using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AlgorithmVisualizer.Algorithms;
using AlgorithmVisualizer.Models;

namespace AlgorithmVisualizer
{
    public enum PlacingMode { Start, End, Wall, Erase }

    public partial class PathfindingForm : Form
    {
        // UI Layout Containers
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel panelGrid;

        // Controls
        private Button btnStart, btnClear, btnSettings;
        private Label lblStatus, lblTitle;
        private RadioButton rbPlaceStart, rbPlaceEnd, rbDrawWall, rbErase;

        // Colors - Modern Palette
        private Color colorBg = Color.FromArgb(18, 18, 24);
        private Color colorSide = Color.FromArgb(28, 28, 38);
        private Color colorAccent = Color.FromArgb(0, 120, 215);
        private Color colorText = Color.FromArgb(220, 220, 230);

        // Grid Data
        private TileType[,] grid;
        private int rows = 20;
        private int cols = 25; // Adjusted for new aspect ratio
        private int tileSize = 28;
        private Point startTile = new Point(-1, -1);
        private Point endTile = new Point(-1, -1);
        private PlacingMode placingMode = PlacingMode.Wall;
        private bool mouseIsDown = false;

        // Animation
        private List<PathStep> steps;
        private int currentStep = 0;
        private System.Windows.Forms.Timer animTimer;
        private int animationSpeed = 30;

        public PathfindingForm()
        {
            this.DoubleBuffered = true;
            BuildModernUI();
            SetupTimer();
            InitGrid();
        }

        private void BuildModernUI()
        {
            this.Text = "Nexus Pathfinding Visualizer";
            this.Size = new Size(1150, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = colorBg;
            this.Font = new Font("Segoe UI", 10);

            // --- 1. Header Panel ---
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = colorSide
            };
            lblTitle = new Label
            {
                Text = "ALGORITHM VISUALIZER",
                Font = new Font("Segoe UI Semibold", 18),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // --- 2. Sidebar Panel ---
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = colorSide,
                Padding = new Padding(20)
            };
            this.Controls.Add(sidePanel);

            int currentY = 20;

            // Mode Selection Group
            Label lblGroup1 = CreateSidebarLabel("INPUT MODE", ref currentY);
            rbPlaceStart = CreateStyledRadio("Start Node", Color.LimeGreen, ref currentY);
            rbPlaceEnd = CreateStyledRadio("Target Node", Color.OrangeRed, ref currentY);
            rbDrawWall = CreateStyledRadio("Build Wall", Color.Silver, ref currentY);
            rbErase = CreateStyledRadio("Eraser", Color.Gray, ref currentY);
            rbDrawWall.Checked = true;

            currentY += 20;

            // Actions Group
            CreateSidebarLabel("CONTROLS", ref currentY);
            btnStart = CreateStyledButton("RUN BFS", Color.FromArgb(0, 180, 120), ref currentY);
            btnClear = CreateStyledButton("RESET GRID", Color.FromArgb(200, 60, 60), ref currentY);
            btnSettings = CreateStyledButton("PREFERENCES", Color.FromArgb(70, 70, 90), ref currentY);

            currentY += 20;

            // Legend Group
            CreateSidebarLabel("LEGEND", ref currentY);
            AddLegendItem("Start Node", Color.LimeGreen, ref currentY);
            AddLegendItem("Target Node", Color.OrangeRed, ref currentY);
            AddLegendItem("Visited", Color.FromArgb(50, 150, 255), ref currentY);
            AddLegendItem("Shortest Path", Color.Yellow, ref currentY);

            // --- 3. Grid View Area ---
            panelGrid = new Panel
            {
                Location = new Point(280, 90),
                BackColor = Color.FromArgb(10, 10, 15),
                BorderStyle = BorderStyle.None
            };
            panelGrid.Paint += PanelGrid_Paint;
            panelGrid.MouseDown += PanelGrid_MouseDown;
            panelGrid.MouseMove += PanelGrid_MouseMove;
            panelGrid.MouseUp += (s, e) => mouseIsDown = false;
            this.Controls.Add(panelGrid);

            // --- 4. Footer Status ---
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                ForeColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Ready to visualize. Select a mode and draw on the grid."
            };
            this.Controls.Add(lblStatus);

            // Initialize Grid sizing
            ResizeGridPanel();

            // Wire Events
            rbPlaceStart.CheckedChanged += (s, e) => { if (rbPlaceStart.Checked) placingMode = PlacingMode.Start; };
            rbPlaceEnd.CheckedChanged += (s, e) => { if (rbPlaceEnd.Checked) placingMode = PlacingMode.End; };
            rbDrawWall.CheckedChanged += (s, e) => { if (rbDrawWall.Checked) placingMode = PlacingMode.Wall; };
            rbErase.CheckedChanged += (s, e) => { if (rbErase.Checked) placingMode = PlacingMode.Erase; };
            btnStart.Click += BtnStart_Click;
            btnClear.Click += BtnClear_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        // --- UI Helper Methods ---

        private Label CreateSidebarLabel(string text, ref int y)
        {
            Label lbl = new Label
            {
                Text = text,
                ForeColor = Color.FromArgb(120, 120, 140),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(20, y),
                AutoSize = true
            };
            sidePanel.Controls.Add(lbl);
            y += 25;
            return lbl;
        }

        private RadioButton CreateStyledRadio(string text, Color bulletColor, ref int y)
        {
            RadioButton rb = new RadioButton
            {
                Text = "  " + text,
                Location = new Point(25, y),
                ForeColor = Color.White,
                Width = 200,
                Height = 30,
                Cursor = Cursors.Hand
            };
            sidePanel.Controls.Add(rb);
            y += 30;
            return rb;
        }

        private Button CreateStyledButton(string text, Color bg, ref int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(20, y),
                Size = new Size(220, 45),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            sidePanel.Controls.Add(btn);
            y += 55;
            return btn;
        }

        private void AddLegendItem(string text, Color c, ref int y)
        {
            Panel p = new Panel
            {
                BackColor = c,
                Size = new Size(12, 12),
                Location = new Point(25, y + 4)
            };
            Label l = new Label
            {
                Text = text,
                ForeColor = Color.Silver,
                Location = new Point(45, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8)
            };
            sidePanel.Controls.Add(p);
            sidePanel.Controls.Add(l);
            y += 22;
        }

        private void ResizeGridPanel()
        {
            int maxWidth = this.ClientSize.Width - sidePanel.Width - 40;
            int maxHeight = this.ClientSize.Height - headerPanel.Height - lblStatus.Height - 40;

            tileSize = Math.Min(maxWidth / cols, maxHeight / rows);
            panelGrid.Size = new Size(cols * tileSize, rows * tileSize);
        }

        // Logic remains largely the same, but with updated colors in GetTileColor
        private Color GetTileColor(TileType type)
        {
            switch (type)
            {
                case TileType.Wall: return Color.FromArgb(45, 45, 60);
                case TileType.Start: return Color.LimeGreen;
                case TileType.End: return Color.OrangeRed;
                case TileType.Visited: return Color.FromArgb(0, 122, 204);
                case TileType.Path: return Color.Yellow;
                default: return Color.FromArgb(30, 30, 40);
            }
        }

        private void PanelGrid_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Color tileColor = GetTileColor(grid[r, c]);
                    Rectangle rect = new Rectangle(c * tileSize, r * tileSize, tileSize - 1, tileSize - 1);

                    using (SolidBrush b = new SolidBrush(tileColor))
                    {
                        // Draw tiles as slightly rounded rectangles for a modern look
                        g.FillRectangle(b, rect);
                    }
                }
            }
        }

        /* [Keep your logic methods: SetupTimer, InitGrid, HandleTilePlacement, 
            AnimTimer_Tick, BtnStart_Click, etc. from your original code here] */
    }
}