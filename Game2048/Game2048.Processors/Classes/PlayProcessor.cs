using _2048ish.Base.Enums;
using _2048ish.Base.Models;
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

    readonly int goal;
    public int Goal => goal;

    readonly LinkedList<GameState> undoChain;

    public GameState CurrentGameState => undoChain.First();

    string moveResultErrorMessage;
    public string MoveResultErrorMessage => moveResultErrorMessage;

    readonly int maxUndos;

    readonly Random randomNumberGenerator;

    readonly bool isNewGame;
    readonly int starterTiles;

    private PlayProcessor(int remainingLives, int gridHeight, int gridWidth, string playerName, List<int> acceptedSpawnables, int goal, int maxUndos, bool isNewGame, int starterTiles)
    {
        this.id = Guid.NewGuid();
        this.remainingLives = remainingLives;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.playerName = playerName;
        this.acceptedSpawnables = acceptedSpawnables;
        this.goal = goal;
        this.maxUndos = maxUndos;
        this.isNewGame = isNewGame;
        this.starterTiles = starterTiles;

        undoChain = new();
        moveResultErrorMessage = string.Empty;
        randomNumberGenerator = new Random();
    }



    /// <summary>
    /// Creates a new instance of the <see cref="PlayProcessor"/> class that can be used for a new game.
    /// </summary>
    /// <param name="gameConfiguration">The configuration to use upon generating and playing the game.</param>
    public PlayProcessor(NewGameConfiguration gameConfiguration) : this(gameConfiguration.MaxLives, gameConfiguration.GridHeight, gameConfiguration.GridWidth, string.Empty, gameConfiguration.AcceptedSpawnables, gameConfiguration.Goal, gameConfiguration.MaxUndos, true, gameConfiguration.StarterTiles)
    {
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
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PlayProcessor"/> class that can be used for a loaded game.
    /// </summary>
    /// <param name="saveData">The saved data of a game that is about to be loaded.</param>
    public PlayProcessor(GameSaveData saveData) : this(saveData.RemainingLives, saveData.GridHeight, saveData.GridWidth, saveData.PlayerName, saveData.AcceptedSpawnables, saveData.Goal, saveData.MaxUndos, false, 0)
    {
        undoChain = new LinkedList<GameState>();
        for (int i = 0; i < saveData.UndoChain.Count; i++)
        {
            undoChain.AddLast(saveData.UndoChain[i]);
        }
        highestNumber = undoChain.First().Grid.Max(row => row.Max());
    }

    public void StartGameActions()
    {
        if (!isNewGame)
        {
            return;
        }
        int numbersToPlace = starterTiles;
        for (int i = 0; i < numbersToPlace && i < gridWidth * gridHeight; i++)
        {
            PlaceRandomNumber();
        }
        highestNumber = undoChain.First().Grid.Max(row => row.Max());
    }

    public bool MoveGrid(MoveDirection direction)
    {
        var currentGameState = undoChain.First();

        // Perform move on a copy
        GameState gameStateCopy = currentGameState.Copy();
        gameStateCopy.Move(direction);

        if (currentGameState.StateEquals(gameStateCopy))
        {
            return false;
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
        return true;
    }

    public PostMoveResult PostMoveActions()
    {
        PlaceRandomNumber();
        GetCurrentMaxNumber();
        if (!GridCanMove(undoChain.First()))
        {
            remainingLives--;
            PlayProcessorEventHappened?.Invoke(this,
                new PlayProcessorEventHappenedEventArgs(PlayProcessorEvent.MaxLivesChanged, RemainingLives));
            if (remainingLives <= 0)
            {
                moveResultErrorMessage = "You have ran out of lives, game is over";
                return PostMoveResult.GameOverError;
            }
            moveResultErrorMessage = "The grid is stuck, you can not " +
                "move, you lose a life. If you run out of lives it is GAME OVER. " +
                "You can undo if you have lives.";
            return PostMoveResult.NotGameEndingError;
        }
        return PostMoveResult.NoError;
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
        undoChain.First().Grid[position.Vertical][position.Horizontal] = tileValue;
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
    bool GridCanMove(GameState state)
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
        result.Populate(goal, acceptedSpawnables, gridHeight, gridWidth, maxUndos, playerName, remainingLives, undoChain.ToList());
        return result;
    }
}