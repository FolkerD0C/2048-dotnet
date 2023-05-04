namespace Game2048.Classes;

public enum MoveDirection
{
    Left,
    Down,
    Up,
    Right
}

public class CannotMoveException : Exception
{
    public CannotMoveException() : base("You can not move in that direction!") {  }
}

public class GridStuckException : Exception
{
    public GridStuckException() : base("The grid is stuck, you can not " +
            "move, you lose a life. If you run out of lives it is GAME OVER. " +
            "You can undo until you have lives.") {  }
}

public class UndoImpossibleException : Exception
{
    public UndoImpossibleException() : base("You can not undo, no more steps in undo chain") {  }
}
