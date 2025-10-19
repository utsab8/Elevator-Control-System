using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControl
{
    partial class MainForm
    {
        private void InitializeComponentSimple()
        {
            this.components = new System.ComponentModel.Container();
            
            // Form - Simple Design matching image
            this.Text = "Elevator System";
            this.Size = new Size(650, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Left Panel - Building View
            Panel pnlBuilding = new Panel();
            pnlBuilding.Location = new Point(20, 20);
            pnlBuilding.Size = new Size(350, 380);
            pnlBuilding.BackColor = Color.FromArgb(200, 210, 220);
            pnlBuilding.BorderStyle = BorderStyle.FixedSingle;
            pnlBuilding.Paint += (s, e) => {
                // Draw grid pattern
                using (var pen = new Pen(Color.FromArgb(180, 190, 200), 1))
                {
                    for (int x = 0; x < pnlBuilding.Width; x += 40)
                    {
                        e.Graphics.DrawLine(pen, x, 0, x, pnlBuilding.Height);
                    }
                    for (int y = 0; y < pnlBuilding.Height; y += 40)
                    {
                        e.Graphics.DrawLine(pen, 0, y, pnlBuilding.Width, y);
                    }
                }
                // Draw floor separator
                using (var pen = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    e.Graphics.DrawLine(pen, 0, pnlBuilding.Height / 2, pnlBuilding.Width, pnlBuilding.Height / 2);
                }
            };
            this.Controls.Add(pnlBuilding);

            // Floor 1 Label - TOP SIDE
            Label lblFloor1 = new Label();
            lblFloor1.Text = "Floor 1";
            lblFloor1.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblFloor1.Location = new Point(120, 10);
            lblFloor1.Size = new Size(120, 40);
            lblFloor1.BackColor = Color.Transparent;
            lblFloor1.ForeColor = Color.FromArgb(50, 50, 50);
            lblFloor1.TextAlign = ContentAlignment.MiddleCenter;
            pnlBuilding.Controls.Add(lblFloor1);

            // Floor 0 Label - BOTTOM SIDE
            Label lblFloor0 = new Label();
            lblFloor0.Text = "Floor 0";
            lblFloor0.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblFloor0.Location = new Point(120, 330);
            lblFloor0.Size = new Size(120, 40);
            lblFloor0.BackColor = Color.Transparent;
            lblFloor0.ForeColor = Color.FromArgb(50, 50, 50);
            lblFloor0.TextAlign = ContentAlignment.MiddleCenter;
            pnlBuilding.Controls.Add(lblFloor0);

            // Elevator Shaft
            pnlElevatorShaft = new Panel();
            pnlElevatorShaft.Location = new Point(150, 70);
            pnlElevatorShaft.Size = new Size(90, 240);
            pnlElevatorShaft.BackColor = Color.FromArgb(150, 150, 150);
            pnlElevatorShaft.BorderStyle = BorderStyle.FixedSingle;
            pnlBuilding.Controls.Add(pnlElevatorShaft);

            // Elevator Car
            pnlElevatorCar = new Panel();
            pnlElevatorCar.Location = new Point(5, 125);
            pnlElevatorCar.Size = new Size(80, 110);
            pnlElevatorCar.BackColor = Color.FromArgb(180, 180, 180);
            pnlElevatorShaft.Controls.Add(pnlElevatorCar);

            // Left Door
            pnlDoorLeft = new Panel();
            pnlDoorLeft.Location = new Point(0, 0);
            pnlDoorLeft.Size = new Size(40, 110);
            pnlDoorLeft.BackColor = Color.FromArgb(100, 110, 120);
            pnlDoorLeft.BorderStyle = BorderStyle.FixedSingle;
            pnlElevatorCar.Controls.Add(pnlDoorLeft);

            // Right Door
            pnlDoorRight = new Panel();
            pnlDoorRight.Location = new Point(40, 0);
            pnlDoorRight.Size = new Size(40, 110);
            pnlDoorRight.BackColor = Color.FromArgb(100, 110, 120);
            pnlDoorRight.BorderStyle = BorderStyle.FixedSingle;
            pnlElevatorCar.Controls.Add(pnlDoorRight);

            // Call Lift Button - Floor 1
            btnRequestFloor1 = new Button();
            btnRequestFloor1.Text = "Call\nLift";
            btnRequestFloor1.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            btnRequestFloor1.Location = new Point(30, 100);
            btnRequestFloor1.Size = new Size(65, 60);
            btnRequestFloor1.BackColor = Color.White;
            btnRequestFloor1.ForeColor = Color.Black;
            btnRequestFloor1.FlatStyle = FlatStyle.Flat;
            btnRequestFloor1.Cursor = Cursors.Hand;
            btnRequestFloor1.Click += btnRequestFloor1_Click;
            pnlBuilding.Controls.Add(btnRequestFloor1);

            // Call Lift Button - Floor 0
            btnRequestFloor2 = new Button();
            btnRequestFloor2.Text = "Call\nLift";
            btnRequestFloor2.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            btnRequestFloor2.Location = new Point(30, 270);
            btnRequestFloor2.Size = new Size(65, 60);
            btnRequestFloor2.BackColor = Color.White;
            btnRequestFloor2.ForeColor = Color.Black;
            btnRequestFloor2.FlatStyle = FlatStyle.Flat;
            btnRequestFloor2.Cursor = Cursors.Hand;
            btnRequestFloor2.Click += btnRequestFloor2_Click;
            pnlBuilding.Controls.Add(btnRequestFloor2);

            // Floor 1 Indicator (Green circle)
            pnlFloor1Indicator = new Panel();
            pnlFloor1Indicator.Location = new Point(280, 110);
            pnlFloor1Indicator.Size = new Size(40, 40);
            pnlFloor1Indicator.BackColor = Color.FromArgb(150, 150, 150);
            System.Drawing.Drawing2D.GraphicsPath circle1 = new System.Drawing.Drawing2D.GraphicsPath();
            circle1.AddEllipse(0, 0, 40, 40);
            pnlFloor1Indicator.Region = new Region(circle1);
            pnlBuilding.Controls.Add(pnlFloor1Indicator);

            // Floor 1 Indicator Label
            lblFloor1Indicator = new Label();
            lblFloor1Indicator.Text = "";
            lblFloor1Indicator.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblFloor1Indicator.Location = new Point(240, 155);
            lblFloor1Indicator.Size = new Size(80, 30);
            lblFloor1Indicator.ForeColor = Color.FromArgb(100, 100, 100);
            lblFloor1Indicator.TextAlign = ContentAlignment.MiddleCenter;
            lblFloor1Indicator.BackColor = Color.Transparent;
            pnlBuilding.Controls.Add(lblFloor1Indicator);

            // Floor 0 Indicator (Gray circle)
            pnlFloor0Indicator = new Panel();
            pnlFloor0Indicator.Location = new Point(280, 280);
            pnlFloor0Indicator.Size = new Size(40, 40);
            pnlFloor0Indicator.BackColor = Color.FromArgb(100, 200, 100);
            System.Drawing.Drawing2D.GraphicsPath circle0 = new System.Drawing.Drawing2D.GraphicsPath();
            circle0.AddEllipse(0, 0, 40, 40);
            pnlFloor0Indicator.Region = new Region(circle0);
            pnlBuilding.Controls.Add(pnlFloor0Indicator);

            // Floor 0 Indicator Label
            lblFloor0Indicator = new Label();
            lblFloor0Indicator.Text = "ELEVATOR\nHERE";
            lblFloor0Indicator.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblFloor0Indicator.Location = new Point(240, 325);
            lblFloor0Indicator.Size = new Size(80, 30);
            lblFloor0Indicator.ForeColor = Color.FromArgb(46, 204, 113);
            lblFloor0Indicator.TextAlign = ContentAlignment.MiddleCenter;
            lblFloor0Indicator.BackColor = Color.Transparent;
            pnlBuilding.Controls.Add(lblFloor0Indicator);

            // Right Panel - Control Panel
            pnlControlPanel = new Panel();
            pnlControlPanel.Location = new Point(390, 20);
            pnlControlPanel.Size = new Size(230, 380);
            pnlControlPanel.BackColor = Color.White;
            pnlControlPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlControlPanel);

            // Floor Display
            Label lblFloorTitle = new Label();
            lblFloorTitle.Text = "Floor: ";
            lblFloorTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblFloorTitle.Location = new Point(15, 20);
            lblFloorTitle.Size = new Size(100, 40);
            lblFloorTitle.ForeColor = Color.Black;
            pnlControlPanel.Controls.Add(lblFloorTitle);

            lblControlPanelDisplay = new Label();
            lblControlPanelDisplay.Text = "0";
            lblControlPanelDisplay.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblControlPanelDisplay.Location = new Point(115, 20);
            lblControlPanelDisplay.Size = new Size(100, 40);
            lblControlPanelDisplay.ForeColor = Color.Black;
            pnlControlPanel.Controls.Add(lblControlPanelDisplay);

            // Status Display
            lblElevatorStatus = new Label();
            lblElevatorStatus.Text = "Status: Idle";
            lblElevatorStatus.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblElevatorStatus.Location = new Point(15, 65);
            lblElevatorStatus.Size = new Size(200, 25);
            lblElevatorStatus.ForeColor = Color.Gray;
            pnlControlPanel.Controls.Add(lblElevatorStatus);

            // Open Door Button
            btnOpenDoor = new Button();
            btnOpenDoor.Text = "Open Door";
            btnOpenDoor.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnOpenDoor.Location = new Point(15, 110);
            btnOpenDoor.Size = new Size(200, 55);
            btnOpenDoor.BackColor = Color.FromArgb(100, 200, 100);
            btnOpenDoor.ForeColor = Color.White;
            btnOpenDoor.FlatStyle = FlatStyle.Flat;
            btnOpenDoor.FlatAppearance.BorderSize = 0;
            btnOpenDoor.Cursor = Cursors.Hand;
            btnOpenDoor.Click += btnOpenDoor_Click;
            pnlControlPanel.Controls.Add(btnOpenDoor);

            // Close Door Button
            btnCloseDoor = new Button();
            btnCloseDoor.Text = "Close Door";
            btnCloseDoor.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnCloseDoor.Location = new Point(15, 180);
            btnCloseDoor.Size = new Size(200, 55);
            btnCloseDoor.BackColor = Color.FromArgb(230, 100, 100);
            btnCloseDoor.ForeColor = Color.White;
            btnCloseDoor.FlatStyle = FlatStyle.Flat;
            btnCloseDoor.FlatAppearance.BorderSize = 0;
            btnCloseDoor.Cursor = Cursors.Hand;
            btnCloseDoor.Click += btnCloseDoor_Click;
            pnlControlPanel.Controls.Add(btnCloseDoor);

            // Floor Selection Buttons Label
            Label lblFloorSelection = new Label();
            lblFloorSelection.Text = "Select Floor:";
            lblFloorSelection.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFloorSelection.Location = new Point(15, 250);
            lblFloorSelection.Size = new Size(200, 25);
            lblFloorSelection.ForeColor = Color.Black;
            pnlControlPanel.Controls.Add(lblFloorSelection);

            // Floor 1 Button
            btnFloor1 = new Button();
            btnFloor1.Text = "1";
            btnFloor1.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnFloor1.Location = new Point(15, 280);
            btnFloor1.Size = new Size(95, 50);
            btnFloor1.BackColor = Color.FromArgb(52, 152, 219);
            btnFloor1.ForeColor = Color.White;
            btnFloor1.FlatStyle = FlatStyle.Flat;
            btnFloor1.FlatAppearance.BorderSize = 0;
            btnFloor1.Cursor = Cursors.Hand;
            btnFloor1.Click += btnFloor1_Click;
            pnlControlPanel.Controls.Add(btnFloor1);
            
            // Floor 0 Button
            btnFloor0 = new Button();
            btnFloor0.Text = "0";
            btnFloor0.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnFloor0.Location = new Point(120, 280);
            btnFloor0.Size = new Size(95, 50);
            btnFloor0.BackColor = Color.FromArgb(46, 204, 113);
            btnFloor0.ForeColor = Color.White;
            btnFloor0.FlatStyle = FlatStyle.Flat;
            btnFloor0.FlatAppearance.BorderSize = 0;
            btnFloor0.Cursor = Cursors.Hand;
            btnFloor0.Click += btnFloor0_Click;
            pnlControlPanel.Controls.Add(btnFloor0);

            // Log Button
            btnShowLog = new Button();
            btnShowLog.Text = "View Log";
            btnShowLog.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            btnShowLog.Location = new Point(15, 340);
            btnShowLog.Size = new Size(200, 35);
            btnShowLog.BackColor = Color.FromArgb(100, 100, 100);
            btnShowLog.ForeColor = Color.White;
            btnShowLog.FlatStyle = FlatStyle.Flat;
            btnShowLog.FlatAppearance.BorderSize = 0;
            btnShowLog.Cursor = Cursors.Hand;
            btnShowLog.Click += btnShowLog_Click;
            pnlControlPanel.Controls.Add(btnShowLog);
        }
    }
}
