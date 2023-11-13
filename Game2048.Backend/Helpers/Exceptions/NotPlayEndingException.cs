using System;

namespace Game2048.Backend.Helpers.Exceptions;

public class NotPlayEndingException : Exception
{
    public NotPlayEndingException(string? message) : base(message)
    { }
}