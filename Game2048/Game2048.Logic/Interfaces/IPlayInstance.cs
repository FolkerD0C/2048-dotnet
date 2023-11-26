using Game2048.Shared.EventHandlers;
using System;

namespace Game2048.Logic;

public interface IPlayInstance
{
    int PlayerScore { get; }
    string PlayerName { get; set; }
    event EventHandler<PlayStartedEventArgs>? PlayStarted;
    event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
    event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChanged;
    event EventHandler? PlayEnded;
}
