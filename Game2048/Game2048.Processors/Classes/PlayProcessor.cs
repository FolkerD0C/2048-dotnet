using _2048ish.Base.Enums;
using _2048ish.Base.Models;
using Game2048.Config;
using Game2048.Processors.Enums;
using Game2048.Processors.EventHandlers;
using Game2048.Processors.Helpers;
using Game2048.Processors.SaveDataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Processors;

/// <summary>
/// A class that represents a lower level manager for an active play.
/// </summary>
public class PlayProcessor : IPlayProcessor
{
    readonly Guid id;
    public Guid Id => id;

    int remainingLives;
    public int RemainingLives => remainingLives;

    public int RemainingUndos => undoChain.Count - 1;

    int highestNumber;
    public int HighestNumber => highestNumber;

    readonly int gridWidth;
    public int GridWidth => gridWidth;

    readonly int gridHeight;
    public int GridHeight => gridHeight;

    string playerName;
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    readonly List<int> acceptedSpawnables;
    public List<int>? AcceptedSpawnables => acceptedSpawnables;

    readonly int goal;
    public int Goal => goal;

    readonly LinkedList<GameState> undoChain;

    public GameState CurrentGameState => undoChain.First();

    string moveResultErrorMessage;
    public string MoveResultErrorMessage => moveResultErrorMessage;

    readonly int maxUndos;

    readonly Random randomNumberGenerator;

    /// <summary>
    /// Creates a new instance of the <see cref="PlayProcessor"/> class that can be used for a new game.
    /// </summary>
    public PlayProcessor()
    {
        id = Guid.NewGuid();
        randomNumberGenerator = new Random();
        moveResultErrorMessage = string.Empty;

        goal = ConfigRepository.GetConfigItemValue<int>("DefaultGoal");
        acceptedSpawnables = ConfigRepository.GetConfigItemValue<List<int>>("DefaultAcceptedSpawnables");
        gridHeight = ConfigRepository.GetConfigItemValue<int>("DefaultGridHeight");
        gridWidth = ConfigRepository.GetConfigItemValue<int>("DefaultGridWidth");
        maxUndos = ConfigRepository.GetConfigItemValue<int>("DefaultMaxUndos");
        playerName = string.Empty;
        remainingLives = ConfigRepository.GetConfigItemValue<int>("DefaultMaxLives");

        // Initializing grid
        undoChain = new LinkedList<GameState>();
        GameState firstState = new();
        for (int i = 0; i < gridHeight; i++)
        {
            firstState.Grid.Add(new List<int>());
            for (int j = 0; j < gridWidth; j++)
            {
                firstState.Grid[i].Add(0);
            }
        }
        undoChain.AddFirst(firstState);
        int numbersToPlace = ConfigRepository.GetConfigItemValue<int>("DefaultStarterTiles");
        for (int i = 0; i < numbersToPlace && i < gridWidth * gridHeight; i++)
        {
            PlaceRandomNumber();
        }
        GetCurrentMaxNumber();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PlayProcessor"/> class that can be used for a loaded game.
    /// </summary>
    /// <param name="saveData">The saved data of a game that is about to be loaded.</param>
    public PlayProcessor(GameSaveData saveData)
    {
        randomNumberGenerator = new Random();
        moveResultErrorMessage = string.Empty;

        id = new Guid(saveData.Id);
        goal = saveData.Goal;
        acceptedSpawnables = saveData.AcceptedSpawnables;
        gridHeight = saveData.GridHeight;
        gridWidth = saveData.GridWidth;
        maxUndos = saveData.MaxUndos;
        playerName = saveData.PlayerName;
        remainingLives = saveData.RemainingLives;
        undoChain = new LinkedList<GameState>();
        for (int i = 0; i < saveData.UndoChain.Count; i++)
        {
            undoChain.AddLast(saveData.UndoChain[i]);
        }
        GetCurrentMaxNumber();
    }

    public MoveResult MoveGrid(MoveDirection direction)
    {
        var currentGameState = undoChain.First();

        // Perform move on a copy
        GameState gameStateCopy = currentGameState.Copy();
        gameStateCopy.Move(direction);

        if (currentGameState.StateEquals(gameStateCopy))
        {
            return MoveResult.CannotMoveInthatDirection;
        }

        // If move happened then add current position to the undochain
        undoChain.AddFirst(gameStateCopy);
        if (undoChain.Count > maxUndos)
        {
            undoChain.RemoveLast();
        }
        else
        {
            PlayProcessorEventHappened?.Invoke(this,
                new PlayProcessorEventHappenedEventArgs(PlayProcessorEvent.UndoCountChanged, RemainingUndos));
        }

        // Perform after-move actions
        PlaceRandomNumber();
        GetCurrentMaxNumber();
        if (!CheckIfGridCanMove(gameStateCopy))
        {
            if (--remainingLives <= 0)
            {
                moveResultErrorMessage = "You have ran out of lives, game is over";
                return MoveResult.GameOverError;
            }
            PlayProcessorEventHappened?.Invoke(this,
                new PlayProcessorEventHappenedEventArgs(PlayProcessorEvent.MaxLivesChanged, RemainingLives));
            moveResultErrorMessage = "The grid is stuck, you can not " +
                "move, you lose a life. If you run out of lives it is GAME OVER. " +
                "You can undo if you have lives.";
            return MoveResult.NotGameEndingError;
        }
        return MoveResult.NoError;
    }

    public GameState? Undo()
    {
        if (undoChain.Count > 1)
        {
            undoChain.RemoveFirst();
            PlayProcessorEventHappened?.Invoke(this,
                new PlayProcessorEventHappenedEventArgs(PlayProcessorEvent.UndoCountChanged, RemainingUndos));
            return undoChain.First();
        }
        return null;
    }

    public event EventHandler<PlayProcessorEventHappenedEventArgs>? PlayProcessorEventHappened;

    /// <summary>
    /// Places a random tile on an empty position of the first <see cref="GameState"/> object in the <see cref="undoChain"/>.
    /// </summary>
    void PlaceRandomNumber()
    {
        var emptyTiles = undoChain.First().GetEmptyTiles();
        if (emptyTiles.Count == 0)
        {
            return;
        }
        GameStateExtensions.GridPosition position = emptyTiles[randomNumberGenerator.Next(emptyTiles.Count)];
        int tileValue = acceptedSpawnables[randomNumberGenerator.Next(acceptedSpawnables.Count)];
        undoChain.First().PlaceTile(position.Vertical, position.Horizontal, tileValue);
    }

    /// <summary>
    /// Gets the current highest number.
    /// </summary>
    void GetCurrentMaxNumber()
    {
        int currentMaxNumber = undoChain.First().Grid.Max(row => row.Max());
        if (currentMaxNumber > highestNumber)
        {
            highestNumber = currentMaxNumber;
            PlayProcessorEventHappened?.Invoke(this,
                new PlayProcessorEventHappenedEventArgs(PlayProcessorEvent.MaxNumberChanged, currentMaxNumber));
        }
    }

    /// <summary>
    /// Checks if a move can be performed on the playing grid.
    /// </summary>
    /// <returns>True if a movement can be performed on the playing grid.</returns>
    bool CheckIfGridCanMove(GameState state)
    {
        // Check if there is any empty tile on the grid
        if (state.Grid.Any(row => row.Contains(0)))
        {
            return true;
        }
        // Check if there are similar tiles horizontally
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth - 1; j++)
            {
                if (state.Grid[i][j] == state.Grid[i][j + 1])
                {
                    return true;
                }
            }
        }
        // Check if there are similar tiles vertically
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight - 1; j++)
            {
                if (state.Grid[j][i] == state.Grid[j + 1][i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public GameSaveData GetSaveDataObject()
    {
        GameSaveData result = new();
        result.Populate(id.ToString(), goal, acceptedSpawnables, gridHeight, gridWidth, maxUndos, playerName, remainingLives, undoChain.ToList());
        return result;
    }
}