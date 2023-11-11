using System;
using Game2048.Backend.Helpers.Enums;
using Game2048.Backend.Helpers.EventHandlers;
using Game2048.Backend.Repository;

namespace Game2048.Backend.Logic;

public interface IPlayLogic
{
    public IGameRepository Repository { get; }
    public void HandleInput(GameInput input);

    public void SetPlayerName(string playerName);
    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
}