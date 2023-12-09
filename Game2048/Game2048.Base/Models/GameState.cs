using System.Collections.Generic;

namespace Game2048.Base.Models;

/// <summary>
/// A class that represents a state of the game.
/// </summary>
public record GameState
{
    /// <summary>
    /// The actual playing grid.
    /// </summary>
    public List<List<int>> Grid { get; set; } = new List<List<int>>();

    /// <summary>
    /// The score for the current state.
    /// </summary>
    public int Score { get; set; }
}
