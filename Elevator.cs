using System;
using System.Collections.Generic;

namespace ElevatorControl
{
    public enum ElevatorState
    {
        Idle,
        MovingUp,
        MovingDown,
        DoorsOpening,
        DoorsOpen,
        DoorsClosing
    }

    public class Elevator
    {
        private int currentFloor;
        private ElevatorState state;
        private Door door;
        private List<string> operationLog;
        private DatabaseManager dbManager;

        public event EventHandler<ElevatorEventArgs>? ElevatorStateChanged;
        public event EventHandler<string>? LogEntryAdded;

        public int CurrentFloor 
        { 
            get => currentFloor;
            private set
            {
                currentFloor = value;
                OnElevatorStateChanged();
            }
        }

        public ElevatorState State 
        { 
            get => state;
            private set
            {
                state = value;
                OnElevatorStateChanged();
            }
        }

        public Door Door => door;
        public IReadOnlyList<string> OperationLog => operationLog.AsReadOnly();

        public Elevator()
        {
            currentFloor = 1;
            state = ElevatorState.Idle;
            door = new Door();
            operationLog = new List<string>();
            dbManager = DatabaseManager.Instance;
            AddLog("Elevator initialized at Floor 0");
        }

        public void RequestFloor(int floor)
        {
            if (floor < 1 || floor > 2)
            {
                AddLog($"Invalid floor request: {floor}");
                return;
            }

            if (currentFloor == floor)
            {
                AddLog($"Already at Floor {floor}");
                OpenDoors();
                return;
            }

            AddLog($"Floor {floor} requested from Floor {currentFloor}");
            MoveToFloor(floor);
        }

        private void MoveToFloor(int targetFloor)
        {
            if (door.IsOpen)
            {
                CloseDoors();
            }

            if (targetFloor > currentFloor)
            {
                State = ElevatorState.MovingUp;
                AddLog($"Moving up from Floor {currentFloor} to Floor {targetFloor}");
            }
            else
            {
                State = ElevatorState.MovingDown;
                AddLog($"Moving down from Floor {currentFloor} to Floor {targetFloor}");
            }

            CurrentFloor = targetFloor;
            ArriveAtFloor();
        }

        private void ArriveAtFloor()
        {
            State = ElevatorState.Idle;
            AddLog($"Arrived at Floor {currentFloor}");
            OpenDoors();
        }

        private void OpenDoors()
        {
            State = ElevatorState.DoorsOpening;
            AddLog("Opening doors...");
            door.Open();
            State = ElevatorState.DoorsOpen;
            AddLog("Doors open");
        }

        private void CloseDoors()
        {
            State = ElevatorState.DoorsClosing;
            AddLog("Closing doors...");
            door.Close();
            State = ElevatorState.Idle;
            AddLog("Doors closed");
        }

        private void AddLog(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            operationLog.Add(logEntry);
            
            // Store in database for persistence
            dbManager.AddLog(message, currentFloor, state.ToString());
            
            LogEntryAdded?.Invoke(this, logEntry);
        }

        private void OnElevatorStateChanged()
        {
            ElevatorStateChanged?.Invoke(this, new ElevatorEventArgs(currentFloor, state, door.IsOpen));
        }
    }

    public class ElevatorEventArgs : EventArgs
    {
        public int CurrentFloor { get; }
        public ElevatorState State { get; }
        public bool DoorsOpen { get; }

        public ElevatorEventArgs(int currentFloor, ElevatorState state, bool doorsOpen)
        {
            CurrentFloor = currentFloor;
            State = state;
            DoorsOpen = doorsOpen;
        }
    }
}
