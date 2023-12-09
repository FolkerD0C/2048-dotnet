using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Shared.Models;

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
