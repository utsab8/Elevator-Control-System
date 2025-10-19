namespace ElevatorControl
{
    public class Door
    {
        public bool IsOpen { get; private set; }

        public Door()
        {
            IsOpen = false;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }
    }
}
