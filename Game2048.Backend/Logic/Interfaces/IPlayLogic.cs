using System;
using Game2048.Backend.Helpers.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;

namespace Game2048.Backend.Logic;

public interface IPlayLogic
{
    public int RemainingLives { get; }
    public int RemainingUndos { get; }
    public int HighestNumber { get; }
    public string PlayerName { get; set; }
    public IGamePosition CurrentPosition { get; }
    public void HandleInput(GameInput input);
    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
}