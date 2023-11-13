namespace Game2048.Config;

public static class PlayEnvironment
{
    public static int GridHeight;

    public static int GridWidth;

    public static void LoadWithParameters(int gridHeight, int gridWidth)
    {
        GridHeight = gridHeight;
        GridWidth = gridWidth;
    }
}