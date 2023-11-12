using System;

namespace Game2048.Backend;

public class GameRepositoryEventHappenedEventArgs : EventArgs
{
    public GameRepositoryEvent RepositoryEvent { get; }
    
    public int NumberArg { get; }

    public GameRepositoryEventHappenedEventArgs(GameRepositoryEvent repositoryEvent, int numberArg)
    {
        RepositoryEvent = repositoryEvent;
        NumberArg = numberArg;
    }
}