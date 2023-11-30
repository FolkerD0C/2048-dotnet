namespace Game2048.Config;

/// <summary>
/// A static class for an actual play, game state objects get the grid size from this.
/// </summary>
public static class PlayEnvironment
{
    static int _gridHeight;
    /// <summary>
    /// Height of the active playing grid.
    /// </summary>
    public static int GridHeight => _gridHeight;

    static int _gridWidth;
    /// <summary>
    /// Width of the active playing grid.
    /// </summary>
    public static int GridWidth => _gridWidth;

    /// <summary>
    /// Sets the <see cref="GridHeight"/> to <paramref name="gridHeight"/> and the <see cref="GridWidth"/> to <paramref name="gridWidth"/>.
    /// </summary>
    /// <param name="gridHeight">New value for the height of the active playing grid.</param>
    /// <param name="gridWidth">New value for the width of the active playing grid.</param>
    public static void LoadWithParameters(int gridHeight, int gridWidth)
    {
        _gridHeight = gridHeight;
        _gridWidth = gridWidth;
    }
}