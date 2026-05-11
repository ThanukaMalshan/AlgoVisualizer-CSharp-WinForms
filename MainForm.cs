using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AlgorithmVisualizer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            BuildModernLauncher();
        }

        private void BuildModernLauncher()
        {
            // --- Form Styling ---
            this.Text = "Nexus Visualizer Hub";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 20); // Deep dark background
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // --- Background Accent (Optional Gradient Effect) ---
            Panel topAccent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(topAccent);

            // --- Header Section ---
            Label lblTitle = new Label
            {
                Text = "Algorithm Visualizer",
                Font = new Font("Segoe UI Semibold", 28),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, 50)
            };

            Label lblSub = new Label
            {
                Text = "UNIVERSITY OF KELANIYA | CSCI 22042",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215), // Blue accent
                AutoSize = true,
                Location = new Point(45, 100)
            };

            // --- Container for Cards ---
            FlowLayoutPanel cardContainer = new FlowLayoutPanel
            {
                Location = new Point(40, 160),
                Size = new Size(720, 250),
                BackColor = Color.Transparent
            };

            // --- Creating the "Cards" (Customized Buttons) ---
            Button btnSorting = CreateCard(
                "Sorting Visualizer",
                "Analyze Bubble, Quick, and Merge algorithms in real-time.",
                Color.FromArgb(40, 40, 60),
                BtnSorting_Click
            );

            Button btnPath = CreateCard(
                "Pathfinding Visualizer",
                "Explore BFS, DFS, and Dijkstra on interactive grids.",
                Color.FromArgb(40, 60, 40),
                BtnPath_Click
            );

            cardContainer.Controls.Add(btnSorting);
            cardContainer.Controls.Add(btnPath);

            // Add everything to form
            this.Controls.Add(cardContainer);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);

            // --- Footer Info ---
            Label lblInfo = new Label
            {
                Text = "v2.0 Beta • Visual Programming Project",
                ForeColor = Color.FromArgb(80, 80, 100),
                AutoSize = true,
                Location = new Point(40, 420),
                Font = new Font("Segoe UI", 8)
            };
            this.Controls.Add(lblInfo);
        }

        // Helper to create a "Card" style button
        private Button CreateCard(string title, string desc, Color hoverColor, EventHandler clickEvent)
        {
            Button card = new Button();
            card.Size = new Size(330, 180);
            card.Margin = new Padding(0, 0, 20, 0);
            card.FlatStyle = FlatStyle.Flat;
            card.FlatAppearance.BorderSize = 0;
            card.BackColor = Color.FromArgb(30, 30, 40);
            card.Cursor = Cursors.Hand;

            // Using a Label inside the button is tricky in WinForms, 
            // so we'll use standard text with NewLine for a "Card" feel
            card.Text = $"{title}\n\n{desc}";
            card.TextAlign = ContentAlignment.MiddleCenter;
            card.Font = new Font("Segoe UI Semibold", 12);
            card.ForeColor = Color.FromArgb(220, 220, 230);

            // Interaction Effects
            card.MouseEnter += (s, e) => {
                card.BackColor = hoverColor;
                card.ForeColor = Color.White;
            };
            card.MouseLeave += (s, e) => {
                card.BackColor = Color.FromArgb(30, 30, 40);
                card.ForeColor = Color.FromArgb(220, 220, 230);
            };

            card.Click += clickEvent;
            return card;
        }

        private void BtnSorting_Click(object sender, EventArgs e)
        {
            // Assuming SortingForm exists in your project
            // SortingForm form = new SortingForm();
            // form.Show();
            MessageBox.Show("Launching Sorting Visualizer...");
        }

        private void BtnPath_Click(object sender, EventArgs e)
        {
            PathfindingForm form = new PathfindingForm();
            form.Show();
        }
    }
}