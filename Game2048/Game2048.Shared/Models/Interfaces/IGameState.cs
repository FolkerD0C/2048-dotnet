using System.Collections.Generic;

namespace Game2048.Shared.Models;

/// <summary>
/// Defines two properties that represent a playing grid state.
/// </summary>
public interface IGameState : ISerializable
{
    /// <summary>
    /// The actual playing grid.
    /// </summary>
    public IList<IList<int>> Grid { get; }

    /// <summary>
    /// The score for the current state.
    /// </summary>
    public int Score { get; }
}
