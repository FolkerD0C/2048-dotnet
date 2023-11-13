using System;

namespace Game2048.Repository.Exceptions;

public class NotPlayEndingException : Exception
{
    public NotPlayEndingException(string? message) : base(message)
    { }
}