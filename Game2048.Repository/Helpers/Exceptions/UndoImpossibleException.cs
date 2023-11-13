namespace Game2048.Repository.Exceptions;

public class UndoImpossibleException : NotPlayEndingException
{
    public UndoImpossibleException() : base("You can not undo, no more steps in undo chain") { }
}