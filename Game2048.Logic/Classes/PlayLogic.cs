using Game2048.Logic.Enums;
using Game2048.Repository;
using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.EventHandlers;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

public class PlayLogic : IPlayLogic
{
    static readonly GameInput[] movementInputs = new GameInput[]
    {
        GameInput.Up, GameInput.Down, GameInput.Left, GameInput.Right
    };
    bool goalReached;
    readonly IGameRepository repository;

    public int RemainingLives => repository.RemainingLives;

    public int RemainingUndos => repository.RemainingUndos;

    public int HighestNumber => repository.HighestNumber;

    public int PlayerScore => repository.GetScore();

    public string PlayerName { get { return repository.PlayerName; } set { repository.PlayerName = value; } }

    readonly PriorityQueue<EventArgs, int> eventQueue;

    public event EventHandler<PlayStartedEventArgs>? PlayStarted;
    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
    public event EventHandler? PlayEnded;

    PlayLogic(IGameRepository repository)
    {
        this.repository = repository;
        repository.GameRepositoryEventHappened += GameRepositoryEventHappenedDispatcher;
        eventQueue = new PriorityQueue<EventArgs, int>();
    }

    public PlayLogic() : this(new GameRepository())
    { }

    public static IPlayLogic GetLogicFromSave(IGameRepository repository)
    {
        IPlayLogic logicFromSave = new PlayLogic(repository)
        {
            goalReached = repository.HighestNumber >= repository.Goal
        };
        return logicFromSave;
    }

    public void Start()
    {
        if (repository.UndoChain.First is null)
        {
            throw new InvalidOperationException("First game position can not be null.");
        }
        PlayStarted?.Invoke(this, new PlayStartedEventArgs(
            repository.UndoChain.First.Value, repository.RemainingUndos, repository.RemainingLives,
            repository.HighestNumber, repository.GridHeight, repository.GridWidth
        ));
    }

    public void End()
    {
        PlayEnded?.Invoke(this, new EventArgs());
    }

    public InputResult HandleInput(GameInput input)
    {
        InputResult inputResult = InputResult.Unknown;
        // Handle input
        bool unknownInputDetected = false;
        try
        {
            // Handle movement input
            if (movementInputs.Contains(input))
            {
                MoveDirection moveDirection = MoveDirection.Unknown; ;
                switch (input)
                {
                    case GameInput.Up:
                        {
                            moveDirection = MoveDirection.Up;
                            break;
                        }
                    case GameInput.Down:
                        {
                            moveDirection = MoveDirection.Down;
                            break;
                        }
                    case GameInput.Left:
                        {
                            moveDirection = MoveDirection.Left;
                            break;
                        }
                    case GameInput.Right:
                        {
                            moveDirection = MoveDirection.Right;
                            break;
                        }
                }
                var moveResult = repository.MoveGrid(moveDirection);
                eventQueue.Enqueue(new MoveHappenedEventArgs(repository.CurrentGameState, moveDirection), 2);
                if (moveResult != MoveResult.NoError)
                {
                    eventQueue.Enqueue(new ErrorHappenedEventArgs(repository.MoveResultErrorMessage), 7);
                }
                if (moveResult == MoveResult.GameOverError)
                {
                    inputResult = InputResult.GameOver;
                }
                else
                {
                    inputResult = InputResult.Continue;
                }
            }
            else
            {
                // Handle not moving inputs
                switch (input)
                {
                    case GameInput.Undo:
                        {
                            IGamePosition? undoResult = repository.Undo();
                            if (undoResult is not null)
                            {
                                eventQueue.Enqueue(new UndoHappenedEventArgs(undoResult), 2);
                            }
                            inputResult = InputResult.Continue;
                            break;
                        }
                    case GameInput.Pause:
                        {
                            return InputResult.Pause;
                        }
                    default:
                        {
                            unknownInputDetected = true;
                            break;
                        }
                }
            }
        }
        catch (Exception exc)
        {
            ErrorHappened?.Invoke(this, new ErrorHappenedEventArgs(exc.Message));
            throw;
        }

        if (unknownInputDetected)
        {
            throw new ArgumentOutOfRangeException(nameof(input), "Unknown input detected");
        }

        // If the goal declared in the repository is reached an event should be triggered, but only once
        if (!goalReached && repository.HighestNumber >= repository.Goal)
        {
            eventQueue.Enqueue(new MiscEventHappenedEventArgs(MiscEvent.GoalReached), 5);
        }

        // After all we need to handle the event queue
        HandleEventQueue();
        return inputResult;
    }

    void HandleEventQueue()
    {
        while (eventQueue.Count > 0)
        {
            var eventargs = eventQueue.Dequeue();
            if (eventargs is ErrorHappenedEventArgs errorHappenedEventArgs)
            {
                ErrorHappened?.Invoke(this, errorHappenedEventArgs);
            }
            else if (eventargs is MoveHappenedEventArgs moveHappenedEventArgs)
            {
                MoveHappened?.Invoke(this, moveHappenedEventArgs);
            }
            else if (eventargs is UndoHappenedEventArgs undoHappenedEventArgs)
            {
                UndoHappened?.Invoke(this, undoHappenedEventArgs);
            }
            else if (eventargs is MiscEventHappenedEventArgs miscEventHappenedEventArgs)
            {
                MiscEventHappened?.Invoke(this, miscEventHappenedEventArgs);
            }
            else if (eventargs is GameRepositoryEventHappenedEventArgs gameRepositoryEventHappenedEventArgs)
            {
                MiscEventHappenedEventArgs? convertedRepositoryEventArgs;
                switch (gameRepositoryEventHappenedEventArgs.RepositoryEvent)
                {
                    case GameRepositoryEvent.MaxNumberChanged:
                        {
                            convertedRepositoryEventArgs = new MiscEventHappenedEventArgs(MiscEvent.MaxNumberChanged, gameRepositoryEventHappenedEventArgs.NumberArg);
                            break;
                        }
                    case GameRepositoryEvent.UndoCountChanged:
                        {
                            convertedRepositoryEventArgs = new MiscEventHappenedEventArgs(MiscEvent.UndoCountChanged, gameRepositoryEventHappenedEventArgs.NumberArg);
                            break;
                        }
                    case GameRepositoryEvent.MaxLivesChanged:
                        {
                            convertedRepositoryEventArgs = new MiscEventHappenedEventArgs(MiscEvent.MaxLivesChanged, gameRepositoryEventHappenedEventArgs.NumberArg);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException("Unkown event type detected");
                        }
                }
                MiscEventHappened?.Invoke(this, convertedRepositoryEventArgs);
            }
        }
    }

    internal void GameRepositoryEventHappenedDispatcher(object? sender, GameRepositoryEventHappenedEventArgs args)
    {
        if (args.RepositoryEvent == GameRepositoryEvent.MaxNumberChanged)
        {
            eventQueue.Enqueue(args, 0);
        }
        else
        {
            eventQueue.Enqueue(args, 5);
        }
    }
}
