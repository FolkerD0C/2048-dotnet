using Game2048.Repository.Enums;
using System;

namespace Game2048.Repository.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a repository event.
/// </summary>
public class GameRepositoryEventHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The type of the repository event.
    /// </summary>
    public GameRepositoryEvent RepositoryEvent { get; }

    /// <summary>
    /// A value needed for each repository event type.
    /// </summary>
    public int NumberArg { get; }

    /// <summary>
    /// Creates a new instance for the <see cref="GameRepositoryEventHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="repositoryEvent">The type of the repository event.</param>
    /// <param name="numberArg">A value needed for each repository event type.</param>
    public GameRepositoryEventHappenedEventArgs(GameRepositoryEvent repositoryEvent, int numberArg)
    {
        RepositoryEvent = repositoryEvent;
        NumberArg = numberArg;
    }
}