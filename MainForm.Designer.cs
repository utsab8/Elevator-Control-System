using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControl
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Button btnRequestFloor0;
        private Button btnRequestFloor1;
        private Panel pnlControlPanel;
        private Button btnControlFloor0;
        private Button btnControlFloor1;
        private Button btnOpenDoor;
        private Button btnCloseDoor;
        private Label lblControlDisplay;
        private Panel pnlFloor0Display;
        private Panel pnlFloor1Display;
        private Label lblFloor0Status;
        private Label lblFloor1Status;
        private Panel pnlElevatorShaft;
        private GlassPanel pnlElevatorCar;
        private Panel pnlFloorDoorLeft_Floor0;
        private Panel pnlFloorDoorRight_Floor0;
        private Panel pnlFloorDoorLeft_Floor1;
        private Panel pnlFloorDoorRight_Floor1;
        private Button btnShowLog;
        private Label lblTitle;
        private Label lblCurrentFloor;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            this.Text = "Elevator Control System - Realistic Door Design";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 250);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lblTitle = new Label
            {
                Text = "Two-Floor Elevator Control System - Realistic Door Design",
                Location = new Point(20, 15),
                Size = new Size(960, 40),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(lblTitle);

            InitializeElevatorShaft();
            InitializeRequestButtons();
            InitializeControlPanel();
            InitializeFloorDisplays();
            InitializeLogButton();

            this.ResumeLayout(false);
        }

        private void InitializeElevatorShaft()
        {
            // Building frame
            GlassPanel pnlBuilding = new GlassPanel
            {
                Location = new Point(40, 70),
                Size = new Size(350, 500),
                BackColor = Color.Transparent,
                CornerRadius = 0,
                GlassColor = Color.FromArgb(30, 200, 230, 255),
                BorderColor = Color.FromArgb(80, 100, 120)
            };

            pnlBuilding.Paint += (s, e) =>
            {
                try
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Frame sides
                    using (SolidBrush frameBrush = new SolidBrush(Color.FromArgb(70, 75, 80)))
                    using (Pen framePen = new Pen(Color.FromArgb(50, 55, 60), 3))
                    {
                        g.FillRectangle(frameBrush, 0, 0, 18, 500);
                        g.FillRectangle(frameBrush, 332, 0, 18, 500);
                        g.DrawRectangle(framePen, 0, 0, 18, 500);
                        g.DrawRectangle(framePen, 332, 0, 18, 500);
                        g.FillRectangle(frameBrush, 0, 240, 350, 10);
                        g.DrawRectangle(framePen, 0, 240, 350, 10);
                    }

                    // Glass effect
                    using (System.Drawing.Drawing2D.LinearGradientBrush glassBrush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            new Rectangle(0, 0, 350, 500),
                            Color.FromArgb(15, 180, 220, 255),
                            Color.FromArgb(30, 200, 240, 255),
                            45f))
                    {
                        g.FillRectangle(glassBrush, 18, 10, 314, 230);
                        g.FillRectangle(glassBrush, 18, 250, 314, 240);
                    }

                    // Floor labels
                    using (Font floorFont = new Font("Arial", 16F, FontStyle.Bold))
                    using (SolidBrush floorBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                        g.DrawString("FLOOR 1", floorFont, floorBrush, new RectangleF(100, 100, 150, 30), sf);
                        g.DrawString("FLOOR 0", floorFont, floorBrush, new RectangleF(100, 350, 150, 30), sf);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Building paint error: " + ex.Message);
                }
            };

            this.Controls.Add(pnlBuilding);

            // Elevator shaft
            pnlElevatorShaft = new Panel
            {
                Location = new Point(60, 80),
                Size = new Size(310, 480),
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.None
            };

            pnlElevatorShaft.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Background gradient
                using (System.Drawing.Drawing2D.LinearGradientBrush bgBrush =
                    new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Rectangle(0, 0, 310, 480),
                        Color.FromArgb(60, 60, 65),
                        Color.FromArgb(40, 40, 45),
                        System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(bgBrush, 0, 0, 310, 480);
                }

                // Floor separator
                using (Pen darkPen = new Pen(Color.FromArgb(30, 30, 35), 5))
                using (Pen lightPen = new Pen(Color.FromArgb(80, 80, 85), 2))
                {
                    g.DrawLine(darkPen, 0, 240, 310, 240);
                    g.DrawLine(lightPen, 0, 243, 310, 243);
                }

                // Floor indicators
                DrawFloorIndicator(g, new Rectangle(10, 15, 80, 40), "F1");
                DrawFloorIndicator(g, new Rectangle(10, 255, 80, 40), "F0");

                // Rails
                using (Pen railPen = new Pen(Color.FromArgb(80, 80, 85), 4))
                {
                    g.DrawLine(railPen, 8, 0, 8, 480);
                    g.DrawLine(railPen, 302, 0, 302, 480);
                }
            };

            // FLOOR DOORS - These stay at fixed positions
            // Floor 1 doors (upper floor)
            pnlFloorDoorLeft_Floor1 = CreateFloorDoor(true);
            pnlFloorDoorLeft_Floor1.Location = new Point(30, 10);
            pnlFloorDoorLeft_Floor1.Size = new Size(125, 200);

            pnlFloorDoorRight_Floor1 = CreateFloorDoor(false);
            pnlFloorDoorRight_Floor1.Location = new Point(155, 10);
            pnlFloorDoorRight_Floor1.Size = new Size(125, 200);

            // Floor 0 doors (ground floor)
            pnlFloorDoorLeft_Floor0 = CreateFloorDoor(true);
            pnlFloorDoorLeft_Floor0.Location = new Point(30, 250);
            pnlFloorDoorLeft_Floor0.Size = new Size(125, 200);

            pnlFloorDoorRight_Floor0 = CreateFloorDoor(false);
            pnlFloorDoorRight_Floor0.Location = new Point(155, 250);
            pnlFloorDoorRight_Floor0.Size = new Size(125, 200);

            // ELEVATOR CAR - Add FIRST so it's in the BACKGROUND
            pnlElevatorCar = new GlassPanel
            {
                Location = new Point(25, 250), // Start at Floor 0
                Size = new Size(260, 220),
                BackColor = Color.Transparent,
                CornerRadius = 12,
                GlassColor = Color.FromArgb(80, 180, 220, 255),
                BorderColor = Color.FromArgb(140, 200, 220)
            };

            // Elevator car interior visual (no visible doors from outside)
            pnlElevatorCar.Paint += (s, e) =>
            {
                try
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Draw elevator interior
                    using (System.Drawing.Drawing2D.LinearGradientBrush interiorBrush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            new Rectangle(5, 10, 250, 200),
                            Color.FromArgb(100, 105, 110),
                            Color.FromArgb(70, 75, 80),
                            System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(interiorBrush, 5, 10, 250, 200);
                    }

                    // Interior walls
                    using (Pen wallPen = new Pen(Color.FromArgb(60, 65, 70), 2))
                    {
                        g.DrawRectangle(wallPen, 5, 10, 250, 200);
                    }

                    // Floor pattern
                    using (SolidBrush floorBrush = new SolidBrush(Color.FromArgb(50, 55, 60)))
                    {
                        for (int i = 10; i < 210; i += 20)
                        {
                            for (int j = 10; j < 250; j += 20)
                            {
                                if ((i + j) % 40 == 0)
                                    g.FillRectangle(floorBrush, j, i, 18, 18);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Elevator car paint error: " + ex.Message);
                }
            };

            // Add elevator car FIRST (background layer)
            pnlElevatorShaft.Controls.Add(pnlElevatorCar);

            // Add floor doors AFTER (foreground layer) - they will be on top
            pnlElevatorShaft.Controls.Add(pnlFloorDoorLeft_Floor1);
            pnlElevatorShaft.Controls.Add(pnlFloorDoorRight_Floor1);
            pnlElevatorShaft.Controls.Add(pnlFloorDoorLeft_Floor0);
            pnlElevatorShaft.Controls.Add(pnlFloorDoorRight_Floor0);

            // Bring doors to front to ensure they're above elevator car
            pnlFloorDoorLeft_Floor1.BringToFront();
            pnlFloorDoorRight_Floor1.BringToFront();
            pnlFloorDoorLeft_Floor0.BringToFront();
            pnlFloorDoorRight_Floor0.BringToFront();

            this.Controls.Add(pnlElevatorShaft);
        }

        private Panel CreateFloorDoor(bool isLeft)
        {
            Panel door = new Panel
            {
                Size = new Size(125, 200),
                BackColor = Color.Transparent
            };

            door.Paint += (s, e) =>
            {
                try
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Metallic door surface
                    using (System.Drawing.Drawing2D.LinearGradientBrush doorBrush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            new Rectangle(0, 0, 125, 200),
                            Color.FromArgb(160, 160, 165),
                            Color.FromArgb(120, 120, 125),
                            System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(doorBrush, 0, 0, 125, 200);
                    }

                    // Vertical panel lines
                    using (Pen linePen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                    {
                        for (int i = 0; i < 125; i += 25)
                        {
                            g.DrawLine(linePen, i, 0, i, 200);
                        }
                    }

                    // Door handle
                    using (SolidBrush handleBrush = new SolidBrush(Color.FromArgb(180, 160, 140)))
                    using (Pen handlePen = new Pen(Color.FromArgb(140, 120, 100), 1))
                    {
                        int handleX = isLeft ? 110 : 10;
                        Rectangle handleRect = new Rectangle(handleX, 85, 8, 30);
                        g.FillRectangle(handleBrush, handleRect);
                        g.DrawRectangle(handlePen, handleRect);
                    }

                    // Edge highlight
                    using (Pen edgePen = new Pen(Color.FromArgb(60, 60, 65), 2))
                    {
                        int x = isLeft ? 124 : 0;
                        g.DrawLine(edgePen, x, 0, x, 200);
                    }

                    // Metallic shine effect
                    using (System.Drawing.Drawing2D.LinearGradientBrush shineBrush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            new Rectangle(0, 0, 125, 40),
                            Color.FromArgb(40, 255, 255, 255),
                            Color.FromArgb(0, 255, 255, 255),
                            System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(shineBrush, 0, 0, 125, 40);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Floor door paint error: " + ex.Message);
                }
            };

            return door;
        }

        private void DrawFloorIndicator(Graphics g, Rectangle rect, string floorText)
        {
            using (System.Drawing.Drawing2D.LinearGradientBrush floorBrush =
                new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect,
                    Color.FromArgb(26, 35, 126),
                    Color.FromArgb(13, 18, 63),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(floorBrush, rect);
            }
            using (Pen borderPen = new Pen(Color.FromArgb(63, 81, 181), 2))
            {
                g.DrawRectangle(borderPen, rect);
            }
            using (Font font = new Font("Segoe UI", 14F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(floorText, font, textBrush, rect, sf);
            }
        }

        private void InitializeRequestButtons()
        {
            btnRequestFloor1 = new Button
            {
                Text = "UP\nCALL\nFloor 1",
                Location = new Point(420, 120),
                Size = new Size(120, 90),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRequestFloor1.FlatAppearance.BorderSize = 0;
            btnRequestFloor1.Click += BtnRequestFloor1_Click;
            this.Controls.Add(btnRequestFloor1);

            btnRequestFloor0 = new Button
            {
                Text = "DOWN\nCALL\nFloor 0",
                Location = new Point(420, 370),
                Size = new Size(120, 90),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRequestFloor0.FlatAppearance.BorderSize = 0;
            btnRequestFloor0.Click += BtnRequestFloor0_Click;
            this.Controls.Add(btnRequestFloor0);
        }

        private void InitializeControlPanel()
        {
            pnlControlPanel = new Panel
            {
                Location = new Point(570, 90),
                Size = new Size(220, 380),
                BackColor = Color.FromArgb(60, 60, 65),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblPanelTitle = new Label
            {
                Text = "CONTROL PANEL",
                Location = new Point(10, 10),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlControlPanel.Controls.Add(lblPanelTitle);

            lblControlDisplay = new Label
            {
                Text = "0",
                Location = new Point(40, 55),
                Size = new Size(140, 100),
                Font = new Font("DS-Digital", 56F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlControlPanel.Controls.Add(lblControlDisplay);

            btnControlFloor1 = new Button
            {
                Text = "1",
                Location = new Point(25, 175),
                Size = new Size(80, 70),
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 35, 126),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnControlFloor1.FlatAppearance.BorderSize = 0;
            btnControlFloor1.Click += BtnControlFloor1_Click;
            pnlControlPanel.Controls.Add(btnControlFloor1);

            btnControlFloor0 = new Button
            {
                Text = "0",
                Location = new Point(115, 175),
                Size = new Size(80, 70),
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 35, 126),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnControlFloor0.FlatAppearance.BorderSize = 0;
            btnControlFloor0.Click += BtnControlFloor0_Click;
            pnlControlPanel.Controls.Add(btnControlFloor0);

            btnOpenDoor = new Button
            {
                Text = "◄ ►\nOPEN",
                Location = new Point(25, 260),
                Size = new Size(80, 60),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOpenDoor.FlatAppearance.BorderSize = 0;
            btnOpenDoor.Click += BtnOpenDoor_Click;
            pnlControlPanel.Controls.Add(btnOpenDoor);

            btnCloseDoor = new Button
            {
                Text = "► ◄\nCLOSE",
                Location = new Point(115, 260),
                Size = new Size(80, 60),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCloseDoor.FlatAppearance.BorderSize = 0;
            btnCloseDoor.Click += BtnCloseDoor_Click;
            pnlControlPanel.Controls.Add(btnCloseDoor);

            lblCurrentFloor = new Label
            {
                Text = "Current: Floor 0",
                Location = new Point(10, 335),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlControlPanel.Controls.Add(lblCurrentFloor);

            this.Controls.Add(pnlControlPanel);
        }

        private void InitializeFloorDisplays()
        {
            pnlFloor1Display = new Panel
            {
                Location = new Point(810, 120),
                Size = new Size(160, 110),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblFloor1Title = new Label
            {
                Text = "FLOOR 1 STATUS",
                Location = new Point(10, 10),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlFloor1Display.Controls.Add(lblFloor1Title);

            lblFloor1Status = new Label
            {
                Text = "Elevator Away",
                Location = new Point(10, 50),
                Size = new Size(140, 50),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            pnlFloor1Display.Controls.Add(lblFloor1Status);
            this.Controls.Add(pnlFloor1Display);

            pnlFloor0Display = new Panel
            {
                Location = new Point(810, 340),
                Size = new Size(160, 110),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblFloor0Title = new Label
            {
                Text = "FLOOR 0 STATUS",
                Location = new Point(10, 10),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlFloor0Display.Controls.Add(lblFloor0Title);

            lblFloor0Status = new Label
            {
                Text = "Elevator Here",
                Location = new Point(10, 50),
                Size = new Size(140, 50),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(200, 255, 200)
            };
            pnlFloor0Display.Controls.Add(lblFloor0Status);
            this.Controls.Add(pnlFloor0Display);
        }

        private void InitializeLogButton()
        {
            btnShowLog = new Button
            {
                Text = "SHOW LOG",
                Location = new Point(570, 480),
                Size = new Size(220, 70),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnShowLog.FlatAppearance.BorderSize = 0;
            btnShowLog.Click += BtnShowLog_Click;
            this.Controls.Add(btnShowLog);
        }
    }
}