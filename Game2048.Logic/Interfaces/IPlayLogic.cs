using Game2048.Logic.Enums;
using Game2048.Shared.Enums;
using Game2048.Shared.EventHandlers;
using System;

namespace Game2048.Logic;

// TODO add another interface for handling events
public interface IPlayLogic
{
    int RemainingLives { get; }
    int RemainingUndos { get; }
    int HighestNumber { get; }
    int PlayerScore { get; }
    string PlayerName { get; set; }
    void Start();
    void End();
    InputResult HandleInput(GameInput input);
    event EventHandler<PlayStartedEventArgs>? PlayStarted;
    event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
    event EventHandler? PlayEnded;
}