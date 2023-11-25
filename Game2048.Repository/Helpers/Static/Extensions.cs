using Game2048.Shared.Models;
using System;

namespace Game2048.Repository.Helpers;

internal static class Extensions
{
    public static IRepositoryState AsRepositoryGameState(this IGameState gameState)
    {
        if (gameState is IRepositoryState repositoryGameState)
        {
            return repositoryGameState;
        }
        throw new InvalidOperationException("Invalid cast.");
    }

    public static IGameState AsPublicGameState(this IRepositoryState repositoryGameState)
    {
        if (repositoryGameState is IGameState gameState)
        {
            return gameState;
        }
        throw new InvalidOperationException("Invalid cast.");
    }
}
