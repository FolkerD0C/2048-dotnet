using System;

namespace Game2048.Backend.Helpers.EventHandlers;

public class ErrorHappenedEventArgs : EventArgs
{
    public string ErrorMessage { get; }
    public ErrorHappenedEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}