using Game2048.Repository.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace Game2048.Repository;

public interface IGameRepository
{
    public int RemainingLives { get; }
    public int RemainingUndos { get; }
    public int HighestNumber { get; }
    public int GridWidth { get; }
    public int GridHeight { get; }
    public string PlayerName { get; set; }
    public int Goal { get; }
    public IList<int>? AcceptedSpawnables { get; }
    public LinkedList<IGamePosition> UndoChain { get; }
    public IGamePosition MoveGrid(MoveDirection input);
    public IGamePosition Undo();
    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;
}