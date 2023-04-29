namespace Game2048;

public enum MoveDirection
{
    Left,
    Down,
    Up,
    Right
}

public class CannotMoveException : Exception
{
    public CannotMoveException() : base() {  }
}

public class GridStuckException : Exception
{
    public GridStuckException() : base() {  }
}
