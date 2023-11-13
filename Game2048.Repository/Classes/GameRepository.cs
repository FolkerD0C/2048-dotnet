using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Shared.Enums;
using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Repository.Exceptions;
using Game2048.Shared.Models;
using Game2048.Config;

namespace Game2048.Repository;

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
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    IList<int> acceptedSpawnables;
    public IList<int>? AcceptedSpawnables => acceptedSpawnables;

    int goal;
    public int Goal => goal;

    LinkedList<IGamePosition> undoChain;
    public LinkedList<IGamePosition> UndoChain => undoChain;

    // TODO save me??
    readonly int maxUndos;

    readonly Random randomNumberGenerator;

    public GameRepository() : this(true)
    { }

    GameRepository(bool newGame)
    {
        randomNumberGenerator = new Random();
        undoChain = new LinkedList<IGamePosition>();
        playerName = "";
        acceptedSpawnables = ConfigManager.GetConfigItem("DefaultAcceptedSpawnables", new List<int>());
        maxUndos = ConfigManager.GetConfigItem("DefaultMaxUndos", default(int));
        if (newGame)
        {
            // Setting default values.
            remainingLives = ConfigManager.GetConfigItem("DefaultMaxLives", default(int));
            gridWidth = ConfigManager.GetConfigItem("DefaultGridWidth", default(int));
            gridHeight = ConfigManager.GetConfigItem("DefaultGridHeight", default(int));
            goal = ConfigManager.GetConfigItem("DefaultGoal", default(int));

            // Setting up undochain and starter GamePosition object
            undoChain.AddFirst(new GamePosition());
            PlaceRandomNumber();
            PlaceRandomNumber();
            GetCurrentMaxNumber();
        }
    }

    public static IGameRepository GetRepositoryFromSave(int remainingLives, int gridWidth, int gridHeight, string playerName, int goal, IList<int> acceptedSpawnables, IList<IGamePosition> undoChain)
    {
        var resultRepository = new GameRepository(false)
        {
            remainingLives = remainingLives,
            gridWidth = gridWidth,
            gridHeight = gridHeight,
            playerName = playerName,
            goal = goal,
            acceptedSpawnables = acceptedSpawnables
        };
        foreach (var position in undoChain)
        {
            resultRepository.undoChain.AddLast(position);
        }
        return resultRepository;
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
        if (undoChain.Count > maxUndos)
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

    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;

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