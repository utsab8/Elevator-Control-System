using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControl
{
    public partial class MainForm : Form
    {
        private Elevator elevator;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.Timer movementTimer;
        private System.Windows.Forms.Timer doorTimer;
        private int targetY;
        private int currentY;
        private const int FLOOR_0_Y = 125; // Bottom position (Floor 0) - Ground
        private const int FLOOR_1_Y = 5;    // Top position (Floor 1) - First Floor
        private bool doorsOpen = false;
        private int doorAnimationStep = 0;
        private bool waitingToMove = false; // Flag to start movement after doors close
        private int pendingTargetFloor = 0; // Store target floor for movement

        public MainForm()
        {
            InitializeComponentSimple();
            elevator = new Elevator();
            
            // Subscribe to elevator events
            elevator.ElevatorStateChanged += Elevator_StateChanged;
            elevator.LogEntryAdded += Elevator_LogEntryAdded;

            // Initialize animation timer for button lights
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 2000;
            animationTimer.Tick += AnimationTimer_Tick;

            // Initialize movement timer for smooth elevator animation
            movementTimer = new System.Windows.Forms.Timer();
            movementTimer.Interval = 20; // Smooth, realistic animation
            movementTimer.Tick += MovementTimer_Tick;

            // Initialize door animation timer - SLOW AND GRADUAL
            doorTimer = new System.Windows.Forms.Timer();
            doorTimer.Interval = 15; // Small pixel changes every 15ms
            doorTimer.Tick += DoorTimer_Tick;

            // Set initial position (Floor 0)
            currentY = FLOOR_0_Y;
            targetY = FLOOR_0_Y;
            pnlElevatorCar.Top = FLOOR_0_Y;

            // Initial display update
            UpdateDisplay();
            UpdateStatusLabel();
        }

        private void Elevator_StateChanged(object? sender, ElevatorEventArgs e)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => Elevator_StateChanged(sender, e)));
                    return;
                }

                UpdateDisplay();
                UpdateStatusLabel();
                UpdateFloorIndicators();
            }
            catch (ObjectDisposedException)
            {
                // Form is closing, ignore
            }
            catch (InvalidOperationException ex)
            {
                // Handle invoke errors gracefully
                System.Diagnostics.Debug.WriteLine($"State update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating display: {ex.Message}", "Display Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Elevator_LogEntryAdded(object? sender, string logEntry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Elevator_LogEntryAdded(sender, logEntry)));
                return;
            }
        }

        private void UpdateDisplay()
        {
            // Update control panel display
            int displayFloor = elevator.CurrentFloor - 1;
            lblControlPanelDisplay.Text = displayFloor.ToString();
            
            // Update button colors
            btnFloor1.BackColor = elevator.CurrentFloor == 2 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(52, 152, 219);
            btnFloor0.BackColor = elevator.CurrentFloor == 1 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(52, 152, 219);
        }

        private void UpdateStatusLabel()
        {
            int displayFloor = elevator.CurrentFloor - 1;
            string status = elevator.State switch
            {
                ElevatorState.Idle => $"Idle at Floor {displayFloor}",
                ElevatorState.MovingUp => "Moving Up...",
                ElevatorState.MovingDown => "Moving Down...",
                ElevatorState.DoorsOpening => "Doors Opening",
                ElevatorState.DoorsOpen => $"Arrived at Floor {displayFloor}",
                ElevatorState.DoorsClosing => "Doors Closing",
                _ => "Operating"
            };
            lblElevatorStatus.Text = $"Status: {status}";
        }

        private void UpdateFloorIndicators()
        {
            if (elevator.CurrentFloor == 1) // Ground Floor (0)
            {
                // Floor 0: Elevator is here
                pnlFloor0Indicator.BackColor = Color.FromArgb(100, 200, 100); // Green
                lblFloor0Indicator.Text = "ELEVATOR\nHERE";
                lblFloor0Indicator.ForeColor = Color.FromArgb(46, 204, 113); // Green text
                
                // Floor 1: Elevator away
                pnlFloor1Indicator.BackColor = Color.FromArgb(150, 150, 150); // Gray
                lblFloor1Indicator.Text = "";
                lblFloor1Indicator.ForeColor = Color.FromArgb(100, 100, 100); // Gray text
            }
            else // Floor 1
            {
                // Floor 1: Elevator is here
                pnlFloor1Indicator.BackColor = Color.FromArgb(100, 200, 100); // Green
                lblFloor1Indicator.Text = "ELEVATOR\nHERE";
                lblFloor1Indicator.ForeColor = Color.FromArgb(46, 204, 113); // Green text
                
                // Floor 0: Elevator away
                pnlFloor0Indicator.BackColor = Color.FromArgb(150, 150, 150); // Gray
                lblFloor0Indicator.Text = "";
                lblFloor0Indicator.ForeColor = Color.FromArgb(100, 100, 100); // Gray text
            }
        }

        private void StartElevatorMovement()
        {
            // Update elevator state and start movement
            waitingToMove = false;
            elevator.RequestFloor(pendingTargetFloor);
            
            // Set target position
            targetY = pendingTargetFloor == 2 ? FLOOR_1_Y : FLOOR_0_Y;
            
            // Update status
            if (pendingTargetFloor > elevator.CurrentFloor)
            {
                lblElevatorStatus.Text = "Status: Moving Up...";
            }
            else
            {
                lblElevatorStatus.Text = "Status: Moving Down...";
            }
            
            // Start movement timer
            movementTimer.Start();
        }

        private void MovementTimer_Tick(object? sender, EventArgs e)
        {
            // Smooth, realistic movement: 2 pixels every 20ms
            // Distance: 120 pixels, Speed: 2px/20ms = ~3 seconds total
            int speed = 2;
            
            if (currentY < targetY)
            {
                currentY += speed;
                if (currentY >= targetY)
                {
                    currentY = targetY;
                    movementTimer.Stop();
                    // Movement complete - NOW open doors
                    OpenDoors();
                }
            }
            else if (currentY > targetY)
            {
                currentY -= speed;
                if (currentY <= targetY)
                {
                    currentY = targetY;
                    movementTimer.Stop();
                    // Movement complete - NOW open doors
                    OpenDoors();
                }
            }

            pnlElevatorCar.Top = currentY;
        }

        private void OpenDoors()
        {
            if (doorsOpen) return;
            doorAnimationStep = 0;
            doorsOpen = true;
            lblElevatorStatus.Text = "Status: Doors Opening";
            doorTimer.Start();
        }

        private void CloseDoors()
        {
            if (!doorsOpen)
            {
                // Doors already closed, check if we should start moving
                if (waitingToMove)
                {
                    StartElevatorMovement();
                }
                return;
            }
            doorAnimationStep = 0;
            doorsOpen = false;
            lblElevatorStatus.Text = "Status: Doors Closing";
            doorTimer.Start();
        }

        private void DoorTimer_Tick(object? sender, EventArgs e)
        {
            // SLOW, GRADUAL door movement: 2 pixels every 15ms
            const int maxStep = 35; // Maximum door opening width
            const int doorSpeed = 2; // Slow, realistic speed
            
            if (doorsOpen)
            {
                // Opening animation - doors slide apart SLOWLY
                doorAnimationStep += doorSpeed;
                if (doorAnimationStep >= maxStep)
                {
                    doorAnimationStep = maxStep;
                    doorTimer.Stop();
                    lblElevatorStatus.Text = $"Status: Arrived at Floor {elevator.CurrentFloor - 1}";
                }
                pnlDoorLeft.Left = -doorAnimationStep;
                pnlDoorRight.Left = 40 + doorAnimationStep;
            }
            else
            {
                // Closing animation - doors slide together SLOWLY
                int currentLeft = pnlDoorLeft.Left;
                int currentRight = pnlDoorRight.Left;
                
                if (currentLeft < 0)
                {
                    pnlDoorLeft.Left = Math.Min(0, currentLeft + doorSpeed);
                }
                if (currentRight > 40)
                {
                    pnlDoorRight.Left = Math.Max(40, currentRight - doorSpeed);
                }
                
                if (pnlDoorLeft.Left >= 0 && pnlDoorRight.Left <= 40)
                {
                    // Doors fully closed
                    doorTimer.Stop();
                    lblElevatorStatus.Text = "Status: Doors Closed";
                    
                    // CRITICAL: Start elevator movement ONLY after doors are fully closed
                    if (waitingToMove)
                    {
                        StartElevatorMovement();
                    }
                }
            }
        }

        private void btnRequestFloor1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate elevator is not in error state
                if (elevator == null)
                {
                    MessageBox.Show("Elevator system not initialized!", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Request button pressed - Move elevator to Floor 1
                btnRequestFloor1.BackColor = Color.FromArgb(192, 57, 43);
                animationTimer.Stop();
                animationTimer.Start();
                
                // Check if already at target floor
                if (elevator.CurrentFloor == 2)
                {
                    elevator.RequestFloor(2);
                    return;
                }
                
                // SEQUENCE: Close doors → Move → Open doors
                pendingTargetFloor = 2;
                waitingToMove = true;
                CloseDoors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error requesting floor: {ex.Message}", "Request Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRequestFloor2_Click(object sender, EventArgs e)
        {
            try
            {
                if (elevator == null) return;

                // Request button pressed - Move elevator to Floor 0 (Ground)
                btnRequestFloor2.BackColor = Color.FromArgb(192, 57, 43);
                animationTimer.Stop();
                animationTimer.Start();
                
                // Check if already at target floor
                if (elevator.CurrentFloor == 1)
                {
                    elevator.RequestFloor(1);
                    return;
                }
                
                // SEQUENCE: Close doors → Move → Open doors
                pendingTargetFloor = 1;
                waitingToMove = true;
                CloseDoors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error requesting floor: {ex.Message}", "Request Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFloor1_Click(object sender, EventArgs e)
        {
            // Control panel button pressed - Move to Floor 1
            btnFloor1.BackColor = Color.FromArgb(46, 204, 113);
            animationTimer.Stop();
            animationTimer.Start();
            
            // Check if already at target floor
            if (elevator.CurrentFloor == 2)
            {
                elevator.RequestFloor(2);
                return;
            }
            
            // SEQUENCE: Close doors → Move → Open doors
            pendingTargetFloor = 2;
            waitingToMove = true;
            CloseDoors();
        }

        private void btnFloor0_Click(object sender, EventArgs e)
        {
            // Control panel button pressed - Move to Floor 0 (Ground)
            btnFloor0.BackColor = Color.FromArgb(46, 204, 113);
            animationTimer.Stop();
            animationTimer.Start();
            
            // Check if already at target floor
            if (elevator.CurrentFloor == 1)
            {
                elevator.RequestFloor(1);
                return;
            }
            
            // SEQUENCE: Close doors → Move → Open doors
            pendingTargetFloor = 1;
            waitingToMove = true;
            CloseDoors();
        }

        private void btnShowLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (elevator == null)
                {
                    MessageBox.Show("Elevator system not initialized!", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LogForm logForm = new LogForm(elevator.OperationLog);
                logForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening log: {ex.Message}", "Log Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenDoor_Click(object sender, EventArgs e)
        {
            // Don't allow manual door operation while moving
            if (movementTimer.Enabled)
            {
                MessageBox.Show("Cannot open doors while elevator is moving!", "Operation Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenDoors();
        }

        private void btnCloseDoor_Click(object sender, EventArgs e)
        {
            // Don't allow manual door operation while moving
            if (movementTimer.Enabled)
            {
                MessageBox.Show("Cannot close doors while elevator is moving!", "Operation Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CloseDoors();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            animationTimer.Stop();
            // Reset request button colors to white
            btnRequestFloor1.BackColor = Color.White;
            btnRequestFloor2.BackColor = Color.White;
            UpdateDisplay();
        }
    }
}
