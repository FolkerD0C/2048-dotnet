using Game2048.Config;
using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public IGamePosition CurrentGameState => undoChain.First();

    string moveResultErrorMessage;
    public string MoveResultErrorMessage => moveResultErrorMessage;

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
        acceptedSpawnables = ConfigManager.GetConfigItem("DefaultAcceptedSpawnables", new List<int>()) ?? throw new NullReferenceException("Default accepted spawnables config item can not be null.");
        maxUndos = ConfigManager.GetConfigItem("DefaultMaxUndos", default(int));
        moveResultErrorMessage = "";
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

    public MoveResult MoveGrid(MoveDirection direction)
    {
        // Check if grid can move
        var firstPosition = (undoChain.First
            ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;

        // Perform move on a copy
        IGamePosition gamePositionCopy = firstPosition.Copy();
        gamePositionCopy.Move(direction);

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
        if (!gamePositionCopy.CanMove)
        {
            if (--remainingLives <= 0)
            {
                moveResultErrorMessage = "You have ran out of lives, game is over";
                return MoveResult.GameOverError;
            }
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.MaxLivesChanged, RemainingLives));
            moveResultErrorMessage = "The grid is stuck, you can not " +
                "move, you lose a life. If you run out of lives it is GAME OVER. " +
                "You can undo if you have lives.";
            return MoveResult.NotGameEndingError;
        }
        return MoveResult.NoError;
    }

    public IGamePosition? Undo()
    {
        if (undoChain.Count > 1)
        {
            undoChain.RemoveFirst();
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.UndoCountChanged, RemainingUndos));
            return (undoChain.First ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;
        }
        return null;
    }

    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;

    void PlaceRandomNumber()
    {
        if (undoChain.First is null)
        {
            throw new NullReferenceException("Game repository can not have empty undo chain.");
        }
        var emptyTiles = undoChain.First.Value.GetEmptyTiles();
        if (emptyTiles.Count == 0)
        {
            return;
        }
        var targetTile = emptyTiles[randomNumberGenerator.Next(emptyTiles.Count)];
        int tileValue = acceptedSpawnables[randomNumberGenerator.Next(acceptedSpawnables.Count)];
        undoChain.First.Value
            .PlaceTile(targetTile.Vertical, targetTile.Horizontal, tileValue);
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