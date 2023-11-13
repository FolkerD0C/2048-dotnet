namespace Game2048.Repository.Exceptions;

public class CannotMoveException : NotPlayEndingException
{
    public CannotMoveException() : base("You can not move in that direction!")
    { }
}