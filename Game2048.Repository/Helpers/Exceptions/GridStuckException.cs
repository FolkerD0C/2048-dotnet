namespace Game2048.Repository.Exceptions;

public class GridStuckException : NotPlayEndingException
{
    public GridStuckException() : base("The grid is stuck, you can not " +
            "move, you lose a life. If you run out of lives it is GAME OVER. " +
            "You can undo if you have lives.") {  }
}