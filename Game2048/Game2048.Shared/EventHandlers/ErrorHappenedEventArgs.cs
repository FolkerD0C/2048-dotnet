using System;

namespace Game2048.Shared.EventHandlers;

public class ErrorHappenedEventArgs : EventArgs
{
    public string ErrorMessage { get; }
    public ErrorHappenedEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}