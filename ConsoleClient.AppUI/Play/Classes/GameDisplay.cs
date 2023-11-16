using System.Collections.Generic;
using ConsoleClient.Shared.Models;
using Game2048.Shared.EventHandlers;

namespace ConsoleClient.AppUI.Play;

public class GameDisplay : IGameDisplay
{
    public IDisplayRow this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public IList<IDisplayRow> DisplayRows => throw new System.NotImplementedException();

    public int RowCount => throw new System.NotImplementedException();

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        throw new System.NotImplementedException();
    }

    public void MiscEventHappenedDispatcher(object? sender, MiscEventHappenedEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void OnErrorHappened(object? sender, ErrorHappenedEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void OnMoveHappened(object? sender, MoveHappenedEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayStarted(object? sender, PlayStartedEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void OnUndoHappened(object? sender, UndoHappenedEventArgs args)
    {
        throw new System.NotImplementedException();
    }
}
