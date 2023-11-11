using System.Collections.Generic;
using Game2048.Backend.Repository;

namespace Game2048.Backend;

internal static class PlayEnvironment
{
    internal static int GridHeight;

    internal static int GridWidth;

    internal static void LoadFromConfig()
    {
        GridHeight = GameConfiguration.DefaultGridHeight;
        GridWidth = GameConfiguration.DefaultGridWidth;
    }

    internal static void LoadFromSave(IGameRepository gameRepository)
    {
        GridHeight = gameRepository.UndoChain[0].Grid.Count;
        GridWidth = gameRepository.UndoChain[0].Grid[0].Count;
    }
}