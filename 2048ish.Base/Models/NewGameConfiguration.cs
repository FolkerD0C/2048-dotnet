using System.Collections.Generic;

namespace _2048ish.Base.Models;

public class NewGameConfiguration
{
    /// <summary>
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    public List<int> AcceptedSpawnables = new() { 2, 4 };

    /// <summary>
    /// The goal of the game. If a player reaches it, they win.
    /// </summary>
    public int Goal = 2048;

    /// <summary>
    /// Maximum lives a player can get when starting a new game.
    /// </summary>
    public int MaxLives = 5;

    /// <summary>
    /// Maximum length of the undo chain a player can reach in a play.
    /// </summary>
    public int MaxUndos = 6;

    /// <summary>
    /// Width of the playing grid that sets a new games playing grid width.
    /// </summary>
    public int GridHeight = 4;

    /// <summary>
    /// Height of the playing grid that sets a new games playing grid height.
    /// </summary>
    public int GridWidth = 4;

    /// <summary>
    /// The number of starter tiles/numbers a player can get when starting a new game.
    /// </summary>
    public int StarterTiles = 2;
}
