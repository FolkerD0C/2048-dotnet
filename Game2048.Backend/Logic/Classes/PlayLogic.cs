using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Shared.Enums;
using Game2048.Backend.Helpers.Enums;
using Game2048.Backend.Helpers.EventHandlers;
using Game2048.Backend.Helpers.Exceptions;
using Game2048.Shared.Models;
using Game2048.Backend.Repository;

namespace Game2048.Backend.Logic;

public class PlayLogic : IPlayLogic
{
    static readonly GameInput[] movementInputs = new GameInput[]
    {
        GameInput.Up, GameInput.Down, GameInput.Left, GameInput.Right
    };
    bool goalReached;
    Queue<GameRepositoryEventHappenedEventArgs> gameRepositoryEventQueue;
    IGameRepository repository;

    public int RemainingLives => repository.RemainingLives;

    public int RemainingUndos => repository.RemainingUndos;

    public int HighestNumber => repository.HighestNumber;

    public string PlayerName { get { return repository.PlayerName; } set { repository.PlayerName = value; } }

    public IGamePosition CurrentPosition => throw new NotImplementedException();

    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;

    PlayLogic(IGameRepository repository)
    {
        this.repository = repository;
        repository.GameRepositoryEventHappened += GameRepositoryEventHappenedDispatcher;
        gameRepositoryEventQueue = new Queue<GameRepositoryEventHappenedEventArgs>();
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

    public void HandleInput(GameInput input)
    {
        // Handle input
        bool unknownInputDetected = false;
        bool pauseRequested = false;
        try
        {
            // Handle movement input
            if (movementInputs.Contains(input))
            {
                MoveDirection moveDirection = MoveDirection.Unknown;;
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
                IGamePosition moveResult = repository.MoveGrid(moveDirection);

                // If an exception is not thrown from the 'repository.MoveGrid' call, then handle event
                MoveHappened?.Invoke(this, new MoveHappenedEventArgs(moveResult, moveDirection));
            }
            else
            {
                // Handle not moving inputs
                switch (input)
                {
                    case GameInput.Undo:
                    {
                        IGamePosition undoResult = repository.Undo();

                        // If an exception is not thrown from the 'repository.Undo' call, then handle event
                        UndoHappened?.Invoke(this, new UndoHappenedEventArgs(undoResult));
                        break;
                    }
                    case GameInput.Pause:
                    {
                        // Handle pause event, TODO: needs a callback
                        pauseRequested = true;
                        break;
                    }
                    default:
                    {
                        unknownInputDetected = true;
                        break;
                    }
                }
            }
        }
        catch (NotPlayEndingException exc)
        {
            // When a 'NotPlayEndingException' is caught, the 'ErrorHappened' event needs to be invoked,
            // but there is no need to throw an exception
            ErrorHappened?.Invoke(this, new ErrorHappenedEventArgs(exc.Message));
        }
        catch (GameOverException exc)
        {
            // When a 'GameOverException' is caught, the 'ErrorHappened' event needs to be invoked,
            ErrorHappened?.Invoke(this, new ErrorHappenedEventArgs(exc.Message));

            // and then another event needs to be invoked because the frontend needs to be informed
            MiscEventHappened?.Invoke(this, new MiscEventHappenedEventArgs(MiscEvent.GameOver));

            // and then the exception must be thrown
            // because the logic above this method needs to be informed about this
            throw;
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

        if (pauseRequested)
        {
            throw new PauseRequestedException();
        }

        // If the goal declared in the repository is reached an event should be triggered, but only once
        if (!goalReached && repository.HighestNumber >= repository.Goal)
        {
            MiscEventHappened?.Invoke(this, new MiscEventHappenedEventArgs(MiscEvent.GoalReached));
        }

        // After all we need to handle game repository events as public 'MiscEventHappened' events
        HandleGameRepositoryEventQueue();
    }

    void HandleGameRepositoryEventQueue()
    {
        while (gameRepositoryEventQueue.Count > 0)
        {
            var eventArgs = gameRepositoryEventQueue.Dequeue();
            MiscEvent eventType = MiscEvent.Unknown;
            switch (eventArgs.RepositoryEvent)
            {
                case GameRepositoryEvent.MaxLivesChanged:
                {
                    eventType = MiscEvent.MaxLivesChanged;
                    break;
                }
                case GameRepositoryEvent.MaxNumberChanged:
                {
                    eventType = MiscEvent.MaxNumberChanged;
                    break;
                }
                case GameRepositoryEvent.UndoCountChanged:
                {
                    eventType = MiscEvent.UndoCountChanged;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException($"{nameof(eventArgs.RepositoryEvent)} is an invalid event type.");
                }
            }
            MiscEventHappened?.Invoke(this, new MiscEventHappenedEventArgs(eventType, eventArgs.NumberArg));
        }
    }

    internal void GameRepositoryEventHappenedDispatcher(object? sender, GameRepositoryEventHappenedEventArgs args)
    {
        gameRepositoryEventQueue.Enqueue(args);
    }
}
