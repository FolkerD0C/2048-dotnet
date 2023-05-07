namespace Game2048.Classes;

public enum MoveDirection
{
    Left,
    Down,
    Up,
    Right
}

public enum MenuResult
{
    Yes,
    No,
    OK,
    Obj,
    Back
}

public class CannotMoveException : Exception
{
    public CannotMoveException() : base("You can not move in that direction!") {  }
}

public class GridStuckException : Exception
{
    public GridStuckException() : base("The grid is stuck, you can not " +
            "move, you lose a life. If you run out of lives it is GAME OVER. " +
            "You can undo if you have lives.") {  }
}

public class UndoImpossibleException : Exception
{
    public UndoImpossibleException() : base("You can not undo, no more steps in undo chain") {  }
}

public class GameOverException : Exception
{
    public GameOverException() : base("You have ran out of lives, game is over") {  }
}

public class SavedGameObject
{
    public bool Reached2048 { get; set; }
    public int RemainingLives { get; set; }
    public List<(int[,] Grid, int Score)> GridUndoChain { get; set; }
}
