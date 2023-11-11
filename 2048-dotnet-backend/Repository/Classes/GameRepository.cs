using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Backend.Helpers.Enums;
using Game2048.Backend.Helpers.Exceptions;
using Game2048.Backend.Models;
using Game2048.Classes.Menus;

namespace Game2048.Backend.Repository;

public class GameRepository : IGameRepository
{
    int remainingLives;
    public int RemainingLives => remainingLives;

    public int RemainingUndos => UndoChain.Count - 1;

    int highestNumber;
    public int HighestNumber => highestNumber;

    int gridWidth;
    public int GridWidth => gridWidth;

    int gridHeight;
    public int GridHeight => gridHeight;

    string playerName;
    public string PlayerName => playerName;

    IList<int> acceptedSpawnables;
    public IList<int>? AcceptedSpawnables => acceptedSpawnables;

    int goal;
    public int Goal => goal;

    LinkedList<IGamePosition> undoChain;
    public LinkedList<IGamePosition> UndoChain => undoChain;

    Random randomNumberGenerator;

    public GameRepository()
    {
        // Setting default values.
        remainingLives = GameConfiguration.DefaultMaxLives;
        gridWidth = GameConfiguration.DefaultGridWidth;
        gridHeight = GameConfiguration.DefaultGridHeight;
        playerName = "";
        acceptedSpawnables = GameConfiguration.DefaultAcceptedSpawnables ?? throw new NullReferenceException("Accepted spawnables list can not be null.");
        goal = GameConfiguration.DefaultGoal;
        randomNumberGenerator = new Random();

        // Setting up undochain and starter GamePosition object
        undoChain = new LinkedList<IGamePosition>();
        undoChain.AddFirst(new GamePosition());
        PlaceRandomNumber();
        PlaceRandomNumber();
        GetCurrentMaxNumber();
    }

    public IGamePosition MoveGrid(MoveDirection direction)
    {
        // Check if grid can move
        var firstPosition = (undoChain.First
            ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;
        if (!firstPosition.CanMove)
        {
            if (--remainingLives <= 0)
            {
                throw new GameOverException();
            }
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.MaxLivesChanged, RemainingLives));
            throw new GridStuckException();
        }

        // Perform move on a copy
        IGamePosition gamePositionCopy = firstPosition.Copy();
        gamePositionCopy.Move(direction);
        if (firstPosition.Equals(gamePositionCopy))
        {
            throw new CannotMoveException();
        }

        // If move happened then add current position to the undochain
        undoChain.AddFirst(gamePositionCopy);
        if (undoChain.Count > GameConfiguration.DefaultMaxUndos)
        {
            undoChain.RemoveLast();
        }
        else
        {
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.UndoCountChanged, RemainingUndos));
        }

        // Perform after-move actions
        PlaceRandomNumber();
        GetCurrentMaxNumber();
        return gamePositionCopy;
    }

    public IGamePosition Undo()
    {
        if (undoChain.Count > 1)
        {
            undoChain.RemoveFirst();
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.UndoCountChanged, RemainingUndos));
            return (undoChain.First ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;
        }
        throw new UndoImpossibleException();
    }

    internal event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;

    void PlaceRandomNumber()
    {
        int vertical = randomNumberGenerator.Next(gridHeight);
        int horizontal = randomNumberGenerator.Next(gridWidth);
        int tileValue = acceptedSpawnables[randomNumberGenerator.Next(acceptedSpawnables.Count)];
        (undoChain.First ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value
            .PlaceTile(vertical, horizontal, tileValue);
    }

    void GetCurrentMaxNumber()
    {
        int currentMaxNumber = (undoChain.First ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value
            .Grid.Max(row => row.Max());
        if (currentMaxNumber > highestNumber)
        {
            highestNumber = currentMaxNumber;
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.MaxNumberChanged, currentMaxNumber));
        }
    }
}