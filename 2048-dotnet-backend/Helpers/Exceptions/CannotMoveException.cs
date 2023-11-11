using System;

namespace Game2048.Backend.Helpers.Exceptions;

public class CannotMoveException : Exception
{
    public CannotMoveException() : base("You can not move in that direction!")
    { }
}