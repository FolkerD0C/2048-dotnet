using Game2048.Base.Models;
using System.Collections.Generic;

namespace Game2048.Processors.SaveDataObjects;

public class GameSaveData
{
    /// <summary>
    /// The ID of the saved play.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The goal of the game. If the player reaches it, they win.
    /// </summary>
    public int Goal { get; set; }

    /// <summary>
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    public List<int> AcceptedSpawnables { get; set; }

    /// <summary>
    /// The height of the playing grid.
    /// </summary>
    public int GridHeight { get; set; }

    /// <summary>
    /// The width of the playing grid.
    /// </summary>
    public int GridWidth { get; set; }

    /// <summary>
    /// The maximum number of undos the player can have.
    /// </summary>
    public int MaxUndos { get; set; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string PlayerName { get; set; }

    /// <summary>
    /// The number of remaining lives.
    /// </summary>
    public int RemainingLives { get; set; }

    /// <summary>
    /// A list that is used for undoing. The first element is the current game state.
    /// </summary>
    public List<GameState> UndoChain { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="GameSaveData"/> class.
    /// </summary>
    public GameSaveData()
    {
        Id = string.Empty;
        PlayerName = string.Empty;
        AcceptedSpawnables = new List<int>();
        UndoChain = new List<GameState>();
    }

    /// <summary>
    /// Populates the <see cref="GameSaveData"/> object with save data.
    /// </summary>
    /// <param name="id">The ID of the saved play.</param>
    /// <param name="goal">The goal of the game. If the player reaches it, they win.</param>
    /// <param name="acceptedSpawnables">The list that contains the accepted spawnable tiles/numbers.</param>
    /// <param name="gridHeight">The height of the playing grid.</param>
    /// <param name="gridWidth">The width of the playing grid.</param>
    /// <param name="maxUndos">The maximum number of undos the player can have.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="remainingLives">The number of remaining lives.</param>
    /// <param name="undoChain">A list that is used for undoing. The first element is the current game state.</param>
    internal void Populate(string id, int goal, List<int> acceptedSpawnables, int gridHeight, int gridWidth, int maxUndos, string playerName, int remainingLives, List<GameState> undoChain)
    {
        Id = id;
        Goal = goal;
        AcceptedSpawnables = acceptedSpawnables;
        GridHeight = gridHeight;
        GridWidth = gridWidth;
        MaxUndos = maxUndos;
        PlayerName = playerName;
        RemainingLives = remainingLives;
        UndoChain = undoChain;
    }
}
