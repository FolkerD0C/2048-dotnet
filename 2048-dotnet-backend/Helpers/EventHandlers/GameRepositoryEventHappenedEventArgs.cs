using System;

namespace Game2048.Backend;

internal class GameRepositoryEventHappenedEventArgs : EventArgs
{
    internal GameRepositoryEvent RepositoryEvent { get; }
    
    internal int NumberArg { get; }

    internal GameRepositoryEventHappenedEventArgs(GameRepositoryEvent repositoryEvent, int numberArg)
    {
        RepositoryEvent = repositoryEvent;
        NumberArg = numberArg;
    }
}