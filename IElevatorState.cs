using System;

namespace ElevatorControl
{
    /// <summary>
    /// State Pattern Interface - Defines behavior for each elevator state
    /// Following Open-Closed Principle: Open for extension, closed for modification
    /// </summary>
    public interface IElevatorState
    {
        void EnterState(ElevatorContext context);
        void ExitState(ElevatorContext context);
        void HandleRequest(ElevatorContext context, int targetFloor);
        string GetStateName();
        bool CanAcceptRequest();
    }

    /// <summary>
    /// Context class that maintains current state and delegates behavior
    /// </summary>
    public class ElevatorContext
    {
        private IElevatorState currentState;
        public int CurrentFloor { get; set; }
        public int MinFloor { get; private set; }
        public int MaxFloor { get; private set; }
        
        public event EventHandler<StateChangedEventArgs>? StateChanged;

        public ElevatorContext(int minFloor = 0, int maxFloor = 1)
        {
            MinFloor = minFloor;
            MaxFloor = maxFloor;
            CurrentFloor = minFloor;
            currentState = new IdleState();
        }

        public IElevatorState CurrentState => currentState;

        public void SetState(IElevatorState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            currentState?.EnterState(this);
            OnStateChanged(newState.GetStateName());
        }

        public void RequestFloor(int targetFloor)
        {
            // Validate floor range (OCP: easily extensible to more floors)
            if (targetFloor < MinFloor || targetFloor > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), 
                    $"Floor must be between {MinFloor} and {MaxFloor}");
            }

            currentState.HandleRequest(this, targetFloor);
        }

        protected virtual void OnStateChanged(string stateName)
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(stateName, CurrentFloor));
        }
    }

    public class StateChangedEventArgs : EventArgs
    {
        public string StateName { get; }
        public int CurrentFloor { get; }

        public StateChangedEventArgs(string stateName, int currentFloor)
        {
            StateName = stateName;
            CurrentFloor = currentFloor;
        }
    }

    // ==================== CONCRETE STATE IMPLEMENTATIONS ====================

    /// <summary>
    /// Idle State - Elevator is waiting, can accept requests
    /// </summary>
    public class IdleState : IElevatorState
    {
        public void EnterState(ElevatorContext context)
        {
            // Elevator becomes idle
        }

        public void ExitState(ElevatorContext context)
        {
            // Leaving idle state
        }

        public void HandleRequest(ElevatorContext context, int targetFloor)
        {
            if (targetFloor == context.CurrentFloor)
            {
                // Already at target floor, open doors
                context.SetState(new DoorsOpeningState());
            }
            else if (targetFloor > context.CurrentFloor)
            {
                // Need to move up
                context.SetState(new MovingUpState(targetFloor));
            }
            else
            {
                // Need to move down
                context.SetState(new MovingDownState(targetFloor));
            }
        }

        public string GetStateName() => "Idle";
        public bool CanAcceptRequest() => true;
    }

    /// <summary>
    /// Moving Up State - Elevator is traveling upward
    /// </summary>
    public class MovingUpState : IElevatorState
    {
        private int targetFloor;

        public MovingUpState(int targetFloor)
        {
            this.targetFloor = targetFloor;
        }

        public void EnterState(ElevatorContext context)
        {
            // Start moving up animation
        }

        public void ExitState(ElevatorContext context)
        {
            // Update current floor
            context.CurrentFloor = targetFloor;
        }

        public void HandleRequest(ElevatorContext context, int newTargetFloor)
        {
            // Already moving, queue request (could be enhanced)
            // For now, ignore additional requests while moving
        }

        public string GetStateName() => "MovingUp";
        public bool CanAcceptRequest() => false;
    }

    /// <summary>
    /// Moving Down State - Elevator is traveling downward
    /// </summary>
    public class MovingDownState : IElevatorState
    {
        private int targetFloor;

        public MovingDownState(int targetFloor)
        {
            this.targetFloor = targetFloor;
        }

        public void EnterState(ElevatorContext context)
        {
            // Start moving down animation
        }

        public void ExitState(ElevatorContext context)
        {
            // Update current floor
            context.CurrentFloor = targetFloor;
        }

        public void HandleRequest(ElevatorContext context, int newTargetFloor)
        {
            // Already moving, ignore for now
        }

        public string GetStateName() => "MovingDown";
        public bool CanAcceptRequest() => false;
    }

    /// <summary>
    /// Doors Opening State - Doors are opening
    /// </summary>
    public class DoorsOpeningState : IElevatorState
    {
        public void EnterState(ElevatorContext context)
        {
            // Start door opening animation
        }

        public void ExitState(ElevatorContext context)
        {
            // Doors fully opened
        }

        public void HandleRequest(ElevatorContext context, int targetFloor)
        {
            // Wait for doors to finish opening
        }

        public string GetStateName() => "DoorsOpening";
        public bool CanAcceptRequest() => false;
    }

    /// <summary>
    /// Doors Open State - Doors are fully open, passengers can enter/exit
    /// </summary>
    public class DoorsOpenState : IElevatorState
    {
        public void EnterState(ElevatorContext context)
        {
            // Doors are open
        }

        public void ExitState(ElevatorContext context)
        {
            // Starting to close doors
        }

        public void HandleRequest(ElevatorContext context, int targetFloor)
        {
            // Close doors and move to requested floor
            context.SetState(new DoorsClosingState(targetFloor));
        }

        public string GetStateName() => "DoorsOpen";
        public bool CanAcceptRequest() => true;
    }

    /// <summary>
    /// Doors Closing State - Doors are closing
    /// </summary>
    public class DoorsClosingState : IElevatorState
    {
        private int nextTargetFloor;

        public DoorsClosingState(int nextTargetFloor)
        {
            this.nextTargetFloor = nextTargetFloor;
        }

        public void EnterState(ElevatorContext context)
        {
            // Start door closing animation
        }

        public void ExitState(ElevatorContext context)
        {
            // Doors fully closed, ready to move
            if (nextTargetFloor != context.CurrentFloor)
            {
                context.RequestFloor(nextTargetFloor);
            }
            else
            {
                context.SetState(new IdleState());
            }
        }

        public void HandleRequest(ElevatorContext context, int targetFloor)
        {
            // Doors closing, queue the request
            nextTargetFloor = targetFloor;
        }

        public string GetStateName() => "DoorsClosing";
        public bool CanAcceptRequest() => false;
    }
}
