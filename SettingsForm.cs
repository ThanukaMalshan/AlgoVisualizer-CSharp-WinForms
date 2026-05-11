using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmVisualizer
{
    public enum SettingsMode { Sorting, Pathfinding }

    public partial class SettingsForm : Form
    {
        public int ArraySize { get; private set; }
        public int AnimationSpeed { get; private set; }
        public int GridRows { get; private set; }
        public int GridCols { get; private set; }

        private SettingsMode mode;
        private TrackBar trkSpeed;
        private Label lblSpeedValue;
        private NumericUpDown numArraySize, numRows, numCols;

        // Modern UI Colors
        private Color colorBg = Color.FromArgb(24, 24, 32);
        private Color colorSurface = Color.FromArgb(35, 35, 45);
        private Color colorAccent = Color.FromArgb(0, 120, 215);
        private Color colorText = Color.FromArgb(220, 220, 230);

        public SettingsForm(SettingsMode mode, int currentArraySize = 50, int currentSpeed = 100, int currentRows = 20, int currentCols = 20)
        {
            this.mode = mode;
            this.ArraySize = currentArraySize;
            this.AnimationSpeed = currentSpeed;
            this.GridRows = currentRows;
            this.GridCols = currentCols;

            InitializeComponent();
            BuildModernUI(currentArraySize, currentSpeed, currentRows, currentCols);
        }

        private void BuildModernUI(int arraySize, int speed, int rows, int cols)
        {
            // --- Form Styling ---
            this.Text = "Configuration";
            this.BackColor = colorBg;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int currentY = 20;

            // --- Section 1: Animation Speed ---
            CreateSectionHeader("EXECUTION SPEED", ref currentY);

            trkSpeed = new TrackBar
            {
                Minimum = 1,
                Maximum = 500,
                Value = Math.Max(1, Math.Min(500, speed)),
                Location = new Point(20, currentY),
                Size = new Size(300, 45),
                TickStyle = TickStyle.None
            };
            this.Controls.Add(trkSpeed);
            currentY += 35;

            lblSpeedValue = new Label
            {
                Text = $"{speed} ms per operation",
                ForeColor = colorAccent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(25, currentY),
                AutoSize = true
            };
            this.Controls.Add(lblSpeedValue);
            currentY += 40;

            trkSpeed.ValueChanged += (s, e) => {
                lblSpeedValue.Text = $"{trkSpeed.Value} ms per operation";
                // Color shift: faster (green) to slower (orange)
                lblSpeedValue.ForeColor = trkSpeed.Value < 100 ? Color.LimeGreen : Color.FromArgb(0, 120, 215);
            };

            // --- Section 2: Mode Specific ---
            if (mode == SettingsMode.Sorting)
            {
                CreateSectionHeader("ARRAY SETTINGS", ref currentY);
                AddLabel("Element Count:", 25, currentY + 5);
                numArraySize = CreateNumeric(10, 200, arraySize, 160, currentY);
                currentY += 60;
            }
            else
            {
                CreateSectionHeader("GRID RESOLUTION", ref currentY);

                AddLabel("Rows:", 25, currentY + 5);
                numRows = CreateNumeric(5, 50, rows, 100, currentY);
                currentY += 40;

                AddLabel("Columns:", 25, currentY + 5);
                numCols = CreateNumeric(5, 50, cols, 100, currentY);
                currentY += 60;
            }

            // --- Divider ---
            Panel pnlLine = new Panel
            {
                Size = new Size(320, 1),
                BackColor = Color.FromArgb(50, 50, 60),
                Location = new Point(20, currentY)
            };
            this.Controls.Add(pnlLine);
            currentY += 20;

            // --- Footer Buttons ---
            Button btnApply = new Button
            {
                Text = "APPLY",
                Size = new Size(180, 42),
                Location = new Point(20, currentY),
                BackColor = colorAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;

            Button btnCancel = new Button
            {
                Text = "CANCEL",
                Size = new Size(110, 42),
                Location = new Point(210, currentY),
                BackColor = Color.FromArgb(50, 50, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(btnApply);
            this.Controls.Add(btnCancel);

            // Final Form Sizing
            this.ClientSize = new Size(350, currentY + 70);
        }

        // --- UI Helpers ---

        private void CreateSectionHeader(string text, ref int y)
        {
            Label lbl = new Label
            {
                Text = text,
                ForeColor = Color.FromArgb(120, 120, 140),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lbl);
            y += 25;
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(x, y)
            };
            this.Controls.Add(lbl);
        }

        private NumericUpDown CreateNumeric(int min, int max, int val, int x, int y)
        {
            NumericUpDown num = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = val,
                Location = new Point(x, y),
                Width = 80,
                BackColor = Color.FromArgb(45, 45, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(num);
            return num;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            AnimationSpeed = trkSpeed.Value;
            if (mode == SettingsMode.Sorting)
                ArraySize = (int)numArraySize.Value;
            else
            {
                GridRows = (int)numRows.Value;
                GridCols = (int)numCols.Value;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}