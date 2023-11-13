namespace Game2048.Shared;

public static class PlayEnvironment
{
    internal static int GridHeight;

    internal static int GridWidth;

    public static void LoadWithParameters(int gridHeight, int gridWidth)
    {
        GridHeight = gridHeight;
        GridWidth = gridWidth;
    }
}