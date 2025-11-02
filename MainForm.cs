using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ElevatorControl
{
    public partial class MainForm : Form
    {
        private readonly DatabaseManager dbManager;
        private readonly BackgroundWorker dbWorker;
        private readonly System.Windows.Forms.Timer movementTimer;
        private readonly System.Windows.Forms.Timer doorTimer;
        private readonly ElevatorContext elevatorContext;

        // Animation variables
        private int currentY;
        private int targetY;
        private const int FLOOR_0_Y = 250; // Bottom position (aligned with Floor 0 doors)
        private const int FLOOR_1_Y = 10;  // Top position (aligned with Floor 1 doors)
        private bool doorsOpen = false;
        private int doorAnimationStep = 0;
        private int currentFloor = 0;
        private bool isMoving = false;

        // Door animation constants
        private const int DOOR_CLOSED_WIDTH = 125;
        private const int DOOR_STEP = 6;

        // Delegates
        public delegate void FloorChangedDelegate(int floor);
        public delegate void DoorStateDelegate(bool isOpen);
        public event FloorChangedDelegate OnFloorChanged;
        public event DoorStateDelegate OnDoorStateChanged;

        public MainForm()
        {
            InitializeComponent();

            dbManager = DatabaseManager.Instance;

            dbWorker = new BackgroundWorker();
            dbWorker.DoWork += DbWorker_DoWork!;
            dbWorker.RunWorkerCompleted += DbWorker_RunWorkerCompleted!;

            elevatorContext = new ElevatorContext(1, 2);
            elevatorContext.StateChanged += ElevatorContext_StateChanged!;

            movementTimer = new System.Windows.Forms.Timer();
            movementTimer.Interval = 20;
            movementTimer.Tick += MovementTimer_Tick!;

            doorTimer = new System.Windows.Forms.Timer();
            doorTimer.Interval = 15;
            doorTimer.Tick += DoorTimer_Tick!;

            OnFloorChanged += UpdateFloorDisplays;
            OnDoorStateChanged += UpdateDoorStatus;

            currentY = FLOOR_0_Y;
            targetY = FLOOR_0_Y;
            pnlElevatorCar.Top = FLOOR_0_Y;

            // Initialize: ALL doors CLOSED at startup
            InitializeDoorPositions();

            LogOperation("System initialized at Floor 0");
        }

        private void InitializeDoorPositions()
        {
            // ALL FLOOR DOORS START CLOSED
            // Floor 0 doors
            pnlFloorDoorLeft_Floor0.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorLeft_Floor0.Left = 30;
            pnlFloorDoorRight_Floor0.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorRight_Floor0.Left = 155;

            // Floor 1 doors
            pnlFloorDoorLeft_Floor1.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorLeft_Floor1.Left = 30;
            pnlFloorDoorRight_Floor1.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorRight_Floor1.Left = 155;

            doorAnimationStep = 0;
            doorsOpen = false;
        }

        #region Event Handlers

        private void BtnRequestFloor0_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Elevator is currently moving. Please wait.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogOperation("Request button pressed for Floor 0");
                MoveToFloor(0);
            }
            catch (Exception ex)
            {
                HandleException("Request Floor 0", ex);
            }
        }

        private void BtnRequestFloor1_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Elevator is currently moving. Please wait.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogOperation("Request button pressed for Floor 1");
                MoveToFloor(1);
            }
            catch (Exception ex)
            {
                HandleException("Request Floor 1", ex);
            }
        }

        private void BtnControlFloor0_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Elevator is currently moving. Please wait.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogOperation("Control panel button pressed for Floor 0");
                MoveToFloor(0);
            }
            catch (Exception ex)
            {
                HandleException("Control Floor 0", ex);
            }
        }

        private void BtnControlFloor1_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Elevator is currently moving. Please wait.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogOperation("Control panel button pressed for Floor 1");
                MoveToFloor(1);
            }
            catch (Exception ex)
            {
                HandleException("Control Floor 1", ex);
            }
        }

        private void BtnOpenDoor_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Cannot open doors while elevator is moving.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LogOperation("Door open button pressed");
                OpenDoors();
            }
            catch (Exception ex)
            {
                HandleException("Open Door", ex);
            }
        }

        private void BtnCloseDoor_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMoving)
                {
                    MessageBox.Show("Elevator is currently moving.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogOperation("Door close button pressed");
                CloseDoors();
            }
            catch (Exception ex)
            {
                HandleException("Close Door", ex);
            }
        }

        #endregion

        #region Movement Logic

        private void MoveToFloor(int targetFloor)
        {
            try
            {
                if (targetFloor == currentFloor)
                {
                    LogOperation($"Already at Floor {targetFloor}");
                    OpenDoors();
                    return;
                }

                elevatorContext.RequestFloor(targetFloor + 1);

                if (doorsOpen)
                {
                    CloseDoors();
                    Task.Delay(1500).ContinueWith(_ =>
                    {
                        if (InvokeRequired)
                            Invoke(new Action(() => StartMovement(targetFloor)));
                        else
                            StartMovement(targetFloor);
                    });
                }
                else
                {
                    StartMovement(targetFloor);
                }
            }
            catch (Exception ex)
            {
                HandleException("Move To Floor", ex);
            }
        }

        private void StartMovement(int targetFloor)
        {
            try
            {
                isMoving = true;
                targetY = targetFloor == 0 ? FLOOR_0_Y : FLOOR_1_Y;

                string direction = targetFloor > currentFloor ? "up" : "down";
                LogOperation($"Elevator moving {direction} to Floor {targetFloor}");

                movementTimer.Start();
            }
            catch (Exception ex)
            {
                HandleException("Start Movement", ex);
            }
        }

        #endregion

        #region Animation Timers

        private void MovementTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                const int speed = 3;

                if (currentY < targetY)
                {
                    currentY = Math.Min(currentY + speed, targetY);
                    pnlElevatorCar.Top = currentY;
                }
                else if (currentY > targetY)
                {
                    currentY = Math.Max(currentY - speed, targetY);
                    pnlElevatorCar.Top = currentY;
                }
                else
                {
                    movementTimer.Stop();
                    isMoving = false;

                    currentFloor = (currentY == FLOOR_0_Y) ? 0 : 1;
                    OnFloorChanged?.Invoke(currentFloor);

                    LogOperation($"Elevator arrived at Floor {currentFloor}");
                    PlayArrivalSound();
                    OpenDoors();
                }
            }
            catch (Exception ex)
            {
                HandleException("Movement Timer", ex);
            }
        }

        private void DoorTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (doorsOpen)
                {
                    // OPENING: Realistic sliding doors - both doors slide outward
                    if (doorAnimationStep < DOOR_CLOSED_WIDTH)
                    {
                        if (currentFloor == 0)
                        {
                            // Open Floor 0 doors - LEFT door slides LEFT, RIGHT door slides RIGHT
                            // Left door: moves left and stays full width
                            pnlFloorDoorLeft_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor0.Left = 30 - doorAnimationStep;

                            // Right door: moves right and stays full width
                            pnlFloorDoorRight_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor0.Left = 155 + doorAnimationStep;

                            // Keep Floor 1 doors CLOSED
                            pnlFloorDoorLeft_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor1.Left = 30;
                            pnlFloorDoorRight_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor1.Left = 155;
                        }
                        else if (currentFloor == 1)
                        {
                            // Open Floor 1 doors - LEFT door slides LEFT, RIGHT door slides RIGHT
                            pnlFloorDoorLeft_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor1.Left = 30 - doorAnimationStep;

                            pnlFloorDoorRight_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor1.Left = 155 + doorAnimationStep;

                            // Keep Floor 0 doors CLOSED
                            pnlFloorDoorLeft_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor0.Left = 30;
                            pnlFloorDoorRight_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor0.Left = 155;
                        }

                        doorAnimationStep += DOOR_STEP;
                    }
                    else
                    {
                        doorTimer.Stop();
                        OnDoorStateChanged?.Invoke(true);

                        // Auto-close after 3 seconds (more realistic timing)
                        Task.Delay(3000).ContinueWith(_ =>
                        {
                            if (InvokeRequired)
                                Invoke(new Action(CloseDoors));
                            else
                                CloseDoors();
                        });
                    }
                }
                else
                {
                    // CLOSING: Realistic sliding doors - both doors slide inward
                    if (doorAnimationStep > 0)
                    {
                        doorAnimationStep -= DOOR_STEP;

                        if (currentFloor == 0)
                        {
                            // Close Floor 0 doors - doors slide back to center
                            pnlFloorDoorLeft_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor0.Left = 30 - doorAnimationStep;

                            pnlFloorDoorRight_Floor0.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor0.Left = 155 + doorAnimationStep;
                        }
                        else if (currentFloor == 1)
                        {
                            // Close Floor 1 doors - doors slide back to center
                            pnlFloorDoorLeft_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorLeft_Floor1.Left = 30 - doorAnimationStep;

                            pnlFloorDoorRight_Floor1.Width = DOOR_CLOSED_WIDTH;
                            pnlFloorDoorRight_Floor1.Left = 155 + doorAnimationStep;
                        }
                    }
                    else
                    {
                        doorTimer.Stop();
                        doorAnimationStep = 0;

                        // Ensure all doors fully closed at exact positions
                        ResetAllDoorsToClosedPosition();

                        OnDoorStateChanged?.Invoke(false);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException("Door Timer", ex);
            }
        }

        private void ResetAllDoorsToClosedPosition()
        {
            // Floor 0 doors
            pnlFloorDoorLeft_Floor0.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorLeft_Floor0.Left = 30;
            pnlFloorDoorRight_Floor0.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorRight_Floor0.Left = 155;

            // Floor 1 doors
            pnlFloorDoorLeft_Floor1.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorLeft_Floor1.Left = 30;
            pnlFloorDoorRight_Floor1.Width = DOOR_CLOSED_WIDTH;
            pnlFloorDoorRight_Floor1.Left = 155;
        }

        private void OpenDoors()
        {
            try
            {
                if (!doorsOpen)
                {
                    doorsOpen = true;
                    doorAnimationStep = 0;
                    doorTimer.Start();
                    PlayDoorSound();
                    LogOperation($"Doors opening at Floor {currentFloor}");
                }
            }
            catch (Exception ex)
            {
                HandleException("Open Doors", ex);
            }
        }

        private void CloseDoors()
        {
            try
            {
                if (doorsOpen)
                {
                    doorsOpen = false;
                    doorTimer.Start();
                    PlayDoorSound();
                    LogOperation($"Doors closing at Floor {currentFloor}");
                }
            }
            catch (Exception ex)
            {
                HandleException("Close Doors", ex);
            }
        }

        #endregion

        #region Display Updates

        private void UpdateFloorDisplays(int floor)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new FloorChangedDelegate(UpdateFloorDisplays), floor);
                    return;
                }

                lblControlDisplay.Text = floor.ToString();
                lblCurrentFloor.Text = $"Current: Floor {floor}";

                if (floor == 0)
                {
                    lblFloor0Status.Text = "Elevator Here";
                    lblFloor0Status.ForeColor = Color.FromArgb(76, 175, 80);
                    lblFloor0Status.BackColor = Color.FromArgb(200, 255, 200);

                    lblFloor1Status.Text = "Elevator Away";
                    lblFloor1Status.ForeColor = Color.Gray;
                    lblFloor1Status.BackColor = Color.FromArgb(240, 240, 240);
                }
                else
                {
                    lblFloor1Status.Text = "Elevator Here";
                    lblFloor1Status.ForeColor = Color.FromArgb(76, 175, 80);
                    lblFloor1Status.BackColor = Color.FromArgb(200, 255, 200);

                    lblFloor0Status.Text = "Elevator Away";
                    lblFloor0Status.ForeColor = Color.Gray;
                    lblFloor0Status.BackColor = Color.FromArgb(240, 240, 240);
                }
            }
            catch (Exception ex)
            {
                HandleException("Update Floor Displays", ex);
            }
        }

        private void UpdateDoorStatus(bool isOpen)
        {
            // Additional updates if needed
        }

        #endregion

        #region Database and State Management

        private void LogOperation(string operation)
        {
            try
            {
                if (!dbWorker.IsBusy)
                {
                    dbWorker.RunWorkerAsync(operation);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logging error: {ex.Message}");
            }
        }

        private void DbWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                string? operation = e.Argument as string;
                if (!string.IsNullOrEmpty(operation))
                {
                    dbManager.AddLog(operation);
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void DbWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception ex)
            {
                HandleException("Database Operation", ex);
            }
        }

        private void BtnShowLog_Click(object sender, EventArgs e)
        {
            try
            {
                LogOperation("Log window opened");
                LogForm logForm = new LogForm();
                logForm.Show();
            }
            catch (Exception ex)
            {
                HandleException("Show Log", ex);
            }
        }

        private void ElevatorContext_StateChanged(object? sender, StateChangedEventArgs e)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => ElevatorContext_StateChanged(sender, e)));
                    return;
                }

                LogOperation($"State changed to: {e.StateName}");
            }
            catch (Exception ex)
            {
                HandleException("State Changed", ex);
            }
        }

        #endregion

        #region Error Handling and Sound

        private void HandleException(string operation, Exception ex)
        {
            try
            {
                string errorMessage = $"Error in {operation}: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                dbManager.AddLog($"ERROR - {errorMessage}");

                MessageBox.Show(
                    $"An error occurred during {operation}.\n\nDetails: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch
            {
                MessageBox.Show("A critical error occurred.", "Critical Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlayArrivalSound()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        Console.Beep(800, 200);
                        System.Threading.Thread.Sleep(50);
                        Console.Beep(600, 300);
                    }
                    catch { }
                });
            }
            catch { }
        }

        private void PlayDoorSound()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        Console.Beep(500, 100);
                    }
                    catch { }
                });
            }
            catch { }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                base.OnFormClosing(e);
                movementTimer?.Stop();
                doorTimer?.Stop();
                LogOperation("System shutdown");

                if (dbWorker.IsBusy)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Shutdown error: {ex.Message}");
            }
        }
    }
}