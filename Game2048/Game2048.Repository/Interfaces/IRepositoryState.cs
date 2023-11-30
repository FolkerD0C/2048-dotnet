using Game2048.Shared.Enums;
using System.Collections.Generic;

namespace Game2048.Repository;

/// <summary>
/// Defines a set of methods needed for the <see cref="GameRepository"/> to operate.
/// </summary>
public interface IRepositoryState
{
    /// <summary>
    /// True if a movement can happen in at least one direction on a playing grid.
    /// </summary>
    public bool CanMove { get; }

    /// <summary>
    /// Gets the empty tiles of a playing grid.
    /// </summary>
    /// <returns></returns>
    IList<(int Vertical, int Horizontal)> GetEmptyTiles();

    /// <summary>
    /// Performs a move on a playing grid.
    /// </summary>
    /// <param name="direction">The direction to move towards.</param>
    public void Move(MoveDirection direction);

    /// <summary>
    /// Performs and returns a copy of an <see cref="IRepositoryState"/> object.
    /// </summary>
    /// <returns>A new <see cref="IRepositoryState"/> object.</returns>
    public IRepositoryState Copy();

    /// <summary>
    /// Sets the tile on the playing grid specified by <paramref name="vertical"/> and <paramref name="horizontal"/> to <paramref name="tileValue"/>.
    /// </summary>
    /// <param name="vertical">The row number on the playing grid to place a new tile to.</param>
    /// <param name="horizontal">The column number on the playing grid to place a new tile to.</param>
    /// <param name="tileValue">The value to place on the playing grid.</param>
    public void PlaceTile(int vertical, int horizontal, int tileValue);
}
