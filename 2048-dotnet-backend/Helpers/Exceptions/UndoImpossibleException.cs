using System;

namespace Game2048.Backend.Helpers.Exceptions;

public class UndoImpossibleException : Exception
{
    public UndoImpossibleException() : base("You can not undo, no more steps in undo chain") {  }
}