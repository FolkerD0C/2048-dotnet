using Game2048.Logic.Enums;
using Game2048.Logic.EventHandlers;
using Game2048.Repository;
using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

/// <summary>
/// A class that represents a set of methods for handling an active play and
/// a set of events and properties that contain information about an active play.
/// </summary>
public class PlayLogic : IPlayLogic
{
    public Guid Id => processor.Id;
    public bool IsSaved { get; set; }

    /// <summary>
    /// Contains all <see cref="GameInput"/>s that are <see cref="MoveDirection"/>s.
    /// </summary>
    static readonly GameInput[] movementInputs = new GameInput[]
    {
        GameInput.Up, GameInput.Down, GameInput.Left, GameInput.Right
    };

    /// <summary>
    /// True if the goal has been reached.
    /// </summary>
    bool goalReached;


    readonly IGameRepository processor;
    public IGameRepository Processor => processor;

    public int PlayerScore => processor.CurrentGameState.Score;

    public string PlayerName
    {
        get { return processor.PlayerName; }
        set
        {
            string oldName = processor.PlayerName;
            processor.PlayerName = value;
            preinputEventQueue.Enqueue(new PlayerNameChangedEventArgs(oldName, processor.PlayerName), 0);
            PlayerNameChangedManagerEvent?.Invoke(this, new PlayerNameChangedEventArgs(oldName, processor.PlayerName));
        }
    }

    /// <summary>
    /// An event queue that should be emptied and triggered before an input.
    /// </summary>
    readonly PriorityQueue<EventArgs, int> preinputEventQueue;
    /// <summary>
    /// An event queue that should be emptied and triggered after an input.
    /// </summary>
    readonly PriorityQueue<EventArgs, int> eventQueue;

    public event EventHandler<PlayStartedEventArgs>? PlayStarted;
    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
    public event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChanged;
    public event EventHandler? PlayEnded;

    public event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChangedManagerEvent;

    /// <summary>
    /// Creates a new instance of the <see cref="PlayLogic"/> class.
    /// </summary>
    /// <param name="repository">The lower level manager of the play.</param>
    public PlayLogic(IGameRepository repository)
    {
        this.processor = repository;
        repository.GameRepositoryEventHappened += GameRepositoryEventHappenedDispatcher;
        preinputEventQueue = new PriorityQueue<EventArgs, int>();
        eventQueue = new PriorityQueue<EventArgs, int>();
        goalReached = repository.HighestNumber >= repository.Goal;
    }

    public void Start()
    {
        PlayStarted?.Invoke(this, new PlayStartedEventArgs(
            processor.CurrentGameState, processor.RemainingUndos, processor.RemainingLives,
            processor.HighestNumber, processor.GridHeight, processor.GridWidth, processor.PlayerName
        ));
    }

    public void End()
    {
        PlayEnded?.Invoke(this, new EventArgs());
    }

    public void PreInput()
    {
        while (preinputEventQueue.Count > 0)
        {
            var eventArgs = preinputEventQueue.Dequeue();
            if (eventArgs is PlayerNameChangedEventArgs playerNameChangedEventArgs)
            {
                PlayerNameChanged?.Invoke(this, playerNameChangedEventArgs);
            }
        }
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
                var moveResult = processor.MoveGrid(moveDirection);
                switch (moveResult)
                {
                    case MoveResult.NoError:
                        {
                            eventQueue.Enqueue(new MoveHappenedEventArgs(processor.CurrentGameState, moveDirection), 2);
                            inputResult = InputResult.Continue;
                            break;
                        }
                    case MoveResult.NotGameEndingError:
                        {
                            eventQueue.Enqueue(new MoveHappenedEventArgs(processor.CurrentGameState, moveDirection), 2);
                            eventQueue.Enqueue(new ErrorHappenedEventArgs(processor.MoveResultErrorMessage), 7);
                            inputResult = InputResult.Continue;
                            break;
                        }
                    case MoveResult.GameOverError:
                        {
                            eventQueue.Enqueue(new ErrorHappenedEventArgs(processor.MoveResultErrorMessage), 7);
                            inputResult = InputResult.GameOver;
                            break;
                        }
                    case MoveResult.CannotMoveInthatDirection:
                        {
                            inputResult = InputResult.Continue;
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                // Handle not moving inputs
                switch (input)
                {
                    case GameInput.Undo:
                        {
                            GameState? undoResult = processor.Undo();
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
        if (!goalReached && processor.HighestNumber >= processor.Goal)
        {
            eventQueue.Enqueue(new MiscEventHappenedEventArgs(MiscEvent.GoalReached), 5);
            goalReached = true;
        }

        // After all we need to handle the event queue
        HandleEventQueue();
        return inputResult;
    }

    /// <summary>
    /// Triggers all events in the <see cref="eventQueue"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Handles repository events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
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
