using System;
using Game2048.Repository.Enums;

namespace Game2048.Repository.EventHandlers;

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