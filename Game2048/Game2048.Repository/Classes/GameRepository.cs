using Game2048.Config;
using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Repository.Helpers;
using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Game2048.Repository;

/// <summary>
/// A class that represents a lower level manager for an active play.
/// </summary>
public class GameRepository : IGameRepository
{
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

    /*// <summary>
    /// Creates a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    /// <param name="newGame">Tells the constructor that it is a new game or not.</param>
    public GameRepository(bool newGame)
    {
        randomNumberGenerator = new Random();
        undoChain = new LinkedList<GameState>();
        playerName = "";
        acceptedSpawnables = ConfigManager.GetConfigItemValue<List<int>>("DefaultAcceptedSpawnables");
        maxUndos = ConfigManager.GetConfigItemValue<int>("DefaultMaxUndos");
        moveResultErrorMessage = "";
        if (newGame)
        {
            // Setting default values.
            remainingLives = ConfigManager.GetConfigItemValue<int>("DefaultMaxLives");
            gridWidth = ConfigManager.GetConfigItemValue<int>("DefaultGridWidth");
            gridHeight = ConfigManager.GetConfigItemValue<int>("DefaultGridHeight");
            goal = ConfigManager.GetConfigItemValue<int>("DefaultGoal");

            // Setting up undochain and starter GameState object
            undoChain.AddFirst(new GameState());
            int numbersToPlace = ConfigManager.GetConfigItemValue<int>("DefaultStarterTiles");
            for (int i = 0; i < numbersToPlace && i < gridWidth * gridHeight; i++)
            {
                PlaceRandomNumber();
            }
            GetCurrentMaxNumber();
        }
    }*/

    /// <summary>
    /// Creates a new instance of the <see cref="GameRepository"/> class that can be used for a new game.
    /// </summary>
    public GameRepository()
    {
        randomNumberGenerator = new Random();
        moveResultErrorMessage = string.Empty;

        goal = ConfigManager.GetConfigItemValue<int>("DefaultGoal");
        acceptedSpawnables = ConfigManager.GetConfigItemValue<List<int>>("DefaultAcceptedSpawnables");
        gridHeight = ConfigManager.GetConfigItemValue<int>("DefaultGridHeight");
        gridWidth = ConfigManager.GetConfigItemValue<int>("DefaultGridWidth");
        maxUndos = ConfigManager.GetConfigItemValue<int>("DefaultMaxUndos");
        playerName = string.Empty;
        remainingLives = ConfigManager.GetConfigItemValue<int>("DefaultMaxLives");
        
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
        int numbersToPlace = ConfigManager.GetConfigItemValue<int>("DefaultStarterTiles");
        for (int i = 0; i < numbersToPlace && i < gridWidth * gridHeight; i++)
        {
            PlaceRandomNumber();
        }
        GetCurrentMaxNumber();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GameRepository"/> class that can be used for a loaded game.
    /// </summary>
    /// <param name="saveData">The saved data of a game that is about to be loaded.</param>
    public GameRepository(GameSaveData saveData)
    {
        randomNumberGenerator = new Random();
        moveResultErrorMessage = string.Empty;

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

    public int GetScore()
    {
        if (undoChain.First is not null)
        {
            return undoChain.First().Score;
        }
        throw new InvalidOperationException("There are no game state object left in the undo chain.");
    }

    public MoveResult MoveGrid(MoveDirection direction)
    {
        var currentGameState = (undoChain.First
            ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;

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
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.UndoCountChanged, RemainingUndos));
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
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.MaxLivesChanged, RemainingLives));
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
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.UndoCountChanged, RemainingUndos));
            return (undoChain.First ?? throw new NullReferenceException("Game repository can not have empty undo chain.")).Value;
        }
        return null;
    }

    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;

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
            GameRepositoryEventHappened?.Invoke(this,
                new GameRepositoryEventHappenedEventArgs(GameRepositoryEvent.MaxNumberChanged, currentMaxNumber));
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

    /*public string Serialize()
    {
        return "";
        StringBuilder jsonBuilder = new();

        jsonBuilder.Append('{');

        jsonBuilder.Append($"\"remainingLives\":{remainingLives},");

        jsonBuilder.Append($"\"gridWidth\":{gridWidth},");

        jsonBuilder.Append($"\"gridHeight\":{gridHeight},");

        jsonBuilder.Append($"\"goal\":{goal},");

        jsonBuilder.Append($"\"playerName\":\"{playerName}\",");

        jsonBuilder.Append("\"acceptedSpawnables\":[");
        jsonBuilder.AppendJoin(",", acceptedSpawnables);
        jsonBuilder.Append("],");

        jsonBuilder.Append($"\"maxUndos\": {maxUndos},");

        jsonBuilder.Append("\"undoChain\":[");
        jsonBuilder.AppendJoin(",", undoChain.Select(state => state.Serialize()));
        jsonBuilder.Append(']');

        jsonBuilder.Append('}');

        return jsonBuilder.ToString();
    }

    public void Deserialize(string deserializee)
    {
        using var jsonDoc = JsonDocument.Parse(deserializee);
        var jsonRoot = jsonDoc.RootElement;
        remainingLives = jsonRoot.GetProperty("remainingLives").GetInt32();
        gridWidth = jsonRoot.GetProperty("gridWidth").GetInt32();
        gridHeight = jsonRoot.GetProperty("gridHeight").GetInt32();
        playerName = jsonRoot.GetProperty("playerName").GetString()
            ?? throw new InvalidOperationException("Player name can not be null");
        goal = jsonRoot.GetProperty("goal").GetInt32();
        acceptedSpawnables = new List<int>();
        var acceptedEnumerable = jsonRoot.GetProperty("acceptedSpawnables").EnumerateArray();
        foreach (var accepted in acceptedEnumerable)
        {
            acceptedSpawnables.Add(accepted.GetInt32());
        }
        maxUndos = jsonRoot.GetProperty("maxUndos").GetInt32();
        undoChain = new LinkedList<GameState>();
        var chainEnumerable = jsonRoot.GetProperty("undoChain").EnumerateArray();
        foreach (var chainElement in chainEnumerable)
        {
            IGameState gameState = new GameState();
            gameState.Deserialize(chainElement.GetRawText());
            undoChain.AddLast(gameState);
        }
        GetCurrentMaxNumber();
    }*/

    public GameSaveData GetSaveDataObject()
    {
        GameSaveData result = new();
        result.Populate(goal, acceptedSpawnables, gridHeight, gridWidth, maxUndos, playerName, remainingLives, undoChain.ToList());
        return result;
    }
}