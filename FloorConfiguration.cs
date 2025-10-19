using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorControl
{
    /// <summary>
    /// Floor Configuration - Demonstrates Open-Closed Principle
    /// New floors can be added without modifying existing code
    /// </summary>
    public class FloorConfiguration
    {
        private List<FloorInfo> floors;
        public int MinFloor { get; private set; }
        public int MaxFloor { get; private set; }

        public FloorConfiguration()
        {
            floors = new List<FloorInfo>();
            // Default: 2 floors (0 and 1)
            AddFloor(0, "Ground Floor", 0);
            AddFloor(1, "First Floor", 1);
            UpdateBounds();
        }

        /// <summary>
        /// Add a new floor - OCP: Open for extension
        /// </summary>
        public void AddFloor(int floorNumber, string floorName, int displayOrder)
        {
            if (floors.Any(f => f.FloorNumber == floorNumber))
            {
                throw new InvalidOperationException($"Floor {floorNumber} already exists");
            }

            floors.Add(new FloorInfo
            {
                FloorNumber = floorNumber,
                FloorName = floorName,
                DisplayOrder = displayOrder
            });

            UpdateBounds();
        }

        /// <summary>
        /// Remove a floor - allows dynamic configuration
        /// </summary>
        public void RemoveFloor(int floorNumber)
        {
            floors.RemoveAll(f => f.FloorNumber == floorNumber);
            UpdateBounds();
        }

        /// <summary>
        /// Get floor information by number
        /// </summary>
        public FloorInfo? GetFloor(int floorNumber)
        {
            return floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
        }

        /// <summary>
        /// Get all floors ordered by display order
        /// </summary>
        public IEnumerable<FloorInfo> GetAllFloors()
        {
            return floors.OrderBy(f => f.DisplayOrder);
        }

        /// <summary>
        /// Validate if a floor number is valid
        /// </summary>
        public bool IsValidFloor(int floorNumber)
        {
            return floors.Any(f => f.FloorNumber == floorNumber);
        }

        private void UpdateBounds()
        {
            if (floors.Count > 0)
            {
                MinFloor = floors.Min(f => f.FloorNumber);
                MaxFloor = floors.Max(f => f.FloorNumber);
            }
        }

        /// <summary>
        /// Example: Configure building with 5 floors (extendable)
        /// </summary>
        public static FloorConfiguration CreateMultiFloorBuilding(int numberOfFloors)
        {
            var config = new FloorConfiguration();
            config.floors.Clear(); // Remove default floors

            for (int i = 0; i < numberOfFloors; i++)
            {
                string floorName = i == 0 ? "Ground Floor" : $"Floor {i}";
                config.AddFloor(i, floorName, i);
            }

            return config;
        }

        /// <summary>
        /// Example: Configure building with basement levels
        /// </summary>
        public static FloorConfiguration CreateBuildingWithBasement()
        {
            var config = new FloorConfiguration();
            config.floors.Clear();

            // Basement levels
            config.AddFloor(-2, "Basement 2", 0);
            config.AddFloor(-1, "Basement 1", 1);
            config.AddFloor(0, "Ground Floor", 2);
            config.AddFloor(1, "First Floor", 3);
            config.AddFloor(2, "Second Floor", 4);

            return config;
        }
    }

    /// <summary>
    /// Floor Information - Represents a single floor
    /// </summary>
    public class FloorInfo
    {
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = "";
        public int DisplayOrder { get; set; }

        public override string ToString()
        {
            return $"{FloorName} ({FloorNumber})";
        }
    }

    /// <summary>
    /// Elevator Controller using State Pattern and Floor Configuration
    /// Demonstrates both OCP and State Pattern working together
    /// </summary>
    public class ModernElevatorController
    {
        private ElevatorContext context;
        private FloorConfiguration floorConfig;

        public event EventHandler<string>? StatusChanged;

        public ModernElevatorController(FloorConfiguration configuration)
        {
            floorConfig = configuration;
            context = new ElevatorContext(configuration.MinFloor, configuration.MaxFloor);
            context.StateChanged += Context_StateChanged;
        }

        public int CurrentFloor => context.CurrentFloor;
        public string CurrentState => context.CurrentState.GetStateName();

        /// <summary>
        /// Request elevator to go to a specific floor
        /// </summary>
        public void RequestFloor(int targetFloor)
        {
            if (!floorConfig.IsValidFloor(targetFloor))
            {
                throw new ArgumentException($"Invalid floor: {targetFloor}");
            }

            try
            {
                context.RequestFloor(targetFloor);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get current floor name
        /// </summary>
        public string GetCurrentFloorName()
        {
            var floor = floorConfig.GetFloor(context.CurrentFloor);
            return floor?.FloorName ?? $"Floor {context.CurrentFloor}";
        }

        private void Context_StateChanged(object? sender, StateChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, $"{e.StateName} at {GetCurrentFloorName()}");
        }

        /// <summary>
        /// Demonstrate extensibility - Add new floor dynamically
        /// </summary>
        public void AddNewFloor(int floorNumber, string floorName)
        {
            floorConfig.AddFloor(floorNumber, floorName, floorNumber);
            // Update context bounds
            context = new ElevatorContext(floorConfig.MinFloor, floorConfig.MaxFloor);
        }
    }
}
