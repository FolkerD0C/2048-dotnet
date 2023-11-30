using Game2048.Shared.Models;
using System;

namespace Game2048.Repository.Helpers;

/// <summary>
/// A static class containing extension methods.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Converts an <see cref="IGameState"/> object to an <see cref="IRepositoryState"/> object.
    /// </summary>
    /// <param name="gameState">The <see cref="IGameState"/> object that needs a conversion.</param>
    /// <returns>The same object as an <see cref="IRepositoryState"/> object.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IRepositoryState AsRepositoryGameState(this IGameState gameState)
    {
        if (gameState is IRepositoryState repositoryGameState)
        {
            return repositoryGameState;
        }
        throw new InvalidOperationException("Invalid cast.");
    }

    /// <summary>
    /// Converts an <see cref="IRepositoryState"/> object to an <see cref="IGameState"/> object.
    /// </summary>
    /// <param name="repositoryGameState">The <see cref="IRepositoryState"/> object that needs a conversion.</param>
    /// <returns>The same object as an <see cref="IGameState"/> object.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IGameState AsPublicGameState(this IRepositoryState repositoryGameState)
    {
        if (repositoryGameState is IGameState gameState)
        {
            return gameState;
        }
        throw new InvalidOperationException("Invalid cast.");
    }
}
