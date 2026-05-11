using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AlgorithmVisualizer.Algorithms;
using AlgorithmVisualizer.Models;

namespace AlgorithmVisualizer
{
    public partial class SortingForm : Form
    {
        // UI Components
        private Panel panelBars;
        private Panel controlDock;
        private ComboBox cmbAlgorithm;
        private Button btnGenerate, btnStart, btnReset, btnSettings;
        private Label lblComparisons, lblStatus, lblTitle;

        // State & Data
        private int[] array;
        private int arraySize = 50;
        private int animationSpeed = 100;
        private int comparisons = 0;
        private int currentStep = 0;
        private List<SortStep> steps;
        private System.Windows.Forms.Timer animTimer;

        // Highlight colors
        private int compareA = -1, compareB = -1;
        private int swapA = -1, swapB = -1;
        private HashSet<int> sortedIndices = new HashSet<int>();

        // Modern Palette
        private Color colorBg = Color.FromArgb(18, 18, 24);
        private Color colorSurface = Color.FromArgb(28, 28, 38);
        private Color colorAccent = Color.FromArgb(0, 120, 215);

        public SortingForm()
        {
            this.DoubleBuffered = true;
            BuildModernUI();
            SetupTimer();
            GenerateArray();
        }

        private void BuildModernUI()
        {
            this.Text = "Spectrum Sorting Visualizer";
            this.Size = new Size(1100, 720);
            this.BackColor = colorBg;
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- 1. Header Section ---
            lblTitle = new Label
            {
                Text = "SORTING ANALYSIS",
                Font = new Font("Segoe UI Semibold", 16),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            lblComparisons = new Label
            {
                Text = "COMPARISONS: 0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 180, 120),
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(250, 30),
                Location = new Point(820, 15)
            };
            this.Controls.Add(lblComparisons);

            // --- 2. Main Visualization Area ---
            panelBars = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1045, 480),
                BackColor = Color.FromArgb(10, 10, 15),
                BorderStyle = BorderStyle.None
            };
            panelBars.Paint += PanelBars_Paint;
            this.Controls.Add(panelBars);

            // --- 3. Bottom Control Dock ---
            controlDock = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = colorSurface,
                Padding = new Padding(20, 10, 20, 10)
            };
            this.Controls.Add(controlDock);

            // Algorithm Dropdown Styling
            cmbAlgorithm = new ComboBox
            {
                Location = new Point(25, 30),
                Size = new Size(180, 35),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            cmbAlgorithm.Items.AddRange(new string[] { "Insertion Sort", "Quick Sort" });
            cmbAlgorithm.SelectedIndex = 0;
            controlDock.Controls.Add(cmbAlgorithm);

            // Buttons
            btnGenerate = CreateStyledButton("🔀 SHUFFLE", 220, Color.FromArgb(70, 70, 90));
            btnStart = CreateStyledButton("▶ START", 340, Color.FromArgb(0, 120, 215));
            btnReset = CreateStyledButton("↺ RESET", 460, Color.FromArgb(180, 60, 60));
            btnSettings = CreateStyledButton("⚙ SETTINGS", 580, Color.FromArgb(50, 50, 65));

            btnReset.Enabled = false;

            // Legend Panel
            AddLegendToDock(720);

            lblStatus = new Label
            {
                Text = "Ready to visualize. Select algorithm and press start.",
                ForeColor = Color.Gray,
                Location = new Point(25, 85),
                AutoSize = true,
                Font = new Font("Segoe UI", 8)
            };
            controlDock.Controls.Add(lblStatus);

            // Wire Events
            btnGenerate.Click += BtnGenerate_Click;
            btnStart.Click += BtnStart_Click;
            btnReset.Click += BtnReset_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        private Button CreateStyledButton(string text, int x, Color bg)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, 25),
                Size = new Size(110, 45),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            controlDock.Controls.Add(btn);
            return btn;
        }

        private void AddLegendToDock(int xStart)
        {
            string[] names = { "Active", "Swap", "Sorted" };
            Color[] colors = { Color.OrangeRed, Color.Orange, Color.LimeGreen };

            for (int i = 0; i < names.Length; i++)
            {
                Panel p = new Panel { BackColor = colors[i], Size = new Size(12, 12), Location = new Point(xStart, 30 + (i * 22)) };
                Label l = new Label { Text = names[i], ForeColor = Color.Silver, Location = new Point(xStart + 20, 28 + (i * 22)), Font = new Font("Segoe UI", 8) };
                controlDock.Controls.Add(p);
                controlDock.Controls.Add(l);
            }
        }

        private void PanelBars_Paint(object sender, PaintEventArgs e)
        {
            if (array == null) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float barWidth = (float)panelBars.Width / array.Length;
            int maxVal = 100;

            for (int i = 0; i < array.Length; i++)
            {
                Color c = GetBarColor(i);
                float h = ((float)array[i] / maxVal) * (panelBars.Height - 20);
                RectangleF rect = new RectangleF(i * barWidth, panelBars.Height - h, barWidth - (barWidth > 3 ? 2 : 0), h);

                using (LinearGradientBrush lgb = new LinearGradientBrush(rect, c, ControlPaint.Light(c, 0.2f), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(lgb, rect);
                }
            }
        }

        private Color GetBarColor(int i)
        {
            if (sortedIndices.Contains(i)) return Color.LimeGreen;
            if (i == compareA || i == compareB) return Color.OrangeRed;
            if (i == swapA || i == swapB) return Color.Orange;
            return Color.FromArgb(60, 100, 180);
        }

        // Logic methods (SetupTimer, GenerateArray, AnimTimer_Tick, etc.) 
        // remain functionally the same as your original code, 
        // just ensure they reference the new UI labels (lblComparisons, lblStatus).

        private void SetupTimer()
        {
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = animationSpeed;
            animTimer.Tick += AnimTimer_Tick;
        }

        private void GenerateArray()
        {
            Random rnd = new Random();
            array = new int[arraySize];
            for (int i = 0; i < arraySize; i++) array[i] = rnd.Next(5, 100);

            sortedIndices.Clear();
            compareA = compareB = swapA = swapB = -1;
            comparisons = 0;
            lblComparisons.Text = "COMPARISONS: 0";
            panelBars.Invalidate();
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (currentStep >= steps.Count)
            {
                animTimer.Stop();
                for (int i = 0; i < array.Length; i++) sortedIndices.Add(i);
                compareA = compareB = swapA = swapB = -1;
                panelBars.Invalidate();

                lblStatus.Text = "Sorting complete.";
                btnStart.Enabled = false;
                btnGenerate.Enabled = btnSettings.Enabled = btnReset.Enabled = true;
                return;
            }

            SortStep step = steps[currentStep];
            array = step.ArrayState;
            compareA = step.CompareA;
            compareB = step.CompareB;
            swapA = step.SwapA;
            swapB = step.SwapB;
            sortedIndices = new HashSet<int>(step.SortedIndices);

            if (step.IsComparison)
            {
                comparisons++;
                lblComparisons.Text = "COMPARISONS: " + comparisons;
            }

            panelBars.Invalidate();
            currentStep++;
        }

        // ... [Rest of your Button Click events here] ...
    }
}