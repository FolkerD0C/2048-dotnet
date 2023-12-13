using _2048ish.Base.Enums;
using _2048ish.Base.Models;
using Game2048.Managers.Enums;
using Game2048.Managers.EventHandlers;
using Game2048.Processors;
using Game2048.Processors.Enums;
using Game2048.Processors.EventHandlers;
using System;
using System.Linq;

namespace Game2048.Managers;

/// <summary>
/// A class that represents a set of methods for handling an active play and
/// a set of events and properties that contain information about an active play.
/// </summary>
public class PlayInstanceManager : IPlayManager
{
    public Guid Id => processor.Id;

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


    readonly IPlayProcessor processor;
    public IPlayProcessor Processor => processor;

    public int PlayerScore => processor.CurrentGameState.Score;

    public string PlayerName
    {
        get { return processor.PlayerName; }
        set
        {
            string oldName = processor.PlayerName;
            processor.PlayerName = value;
            PlayerNameChanged?.Invoke(this, new PlayerNameChangedEventArgs(oldName, processor.PlayerName));
        }
    }

    public event EventHandler<PlayStartedEventArgs>? PlayStarted;
    public event EventHandler<MoveHappenedEventArgs>? MoveHappened;
    public event EventHandler<UndoHappenedEventArgs>? UndoHappened;
    public event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;
    public event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;
    public event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChanged;
    public event EventHandler? InputProcessed;
    public event EventHandler? PlayEnded;

    /// <summary>
    /// Creates a new instance of the <see cref="PlayInstanceManager"/> class.
    /// </summary>
    /// <param name="processor">The lower level manager of the play.</param>
    public PlayInstanceManager(IPlayProcessor processor)
    {
        this.processor = processor;
        processor.PlayProcessorEventHappened += PlayProcessorEventHappenedDispatcher;
        goalReached = processor.HighestNumber >= processor.Goal;
    }

    public void Start()
    {
        if (processor.CurrentGameState.Grid.All(row => row.All(tile => tile == 0)))
        {
            processor.NewGameActions();
        }
        PlayStarted?.Invoke(this, new PlayStartedEventArgs(
            processor.CurrentGameState, processor.RemainingUndos, processor.RemainingLives,
            processor.HighestNumber, processor.GridHeight, processor.GridWidth, processor.PlayerName
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
                var moveHappened = processor.MoveGrid(moveDirection);
                if (!moveHappened)
                {
                    inputResult = InputResult.Continue;
                }
                else
                {
                    var postMoveResult = processor.PostMoveActions();
                    MoveHappened?.Invoke(this, new MoveHappenedEventArgs(processor.CurrentGameState, moveDirection));
                    if (postMoveResult == PostMoveResult.NotGameEndingError || postMoveResult == PostMoveResult.GameOverError)
                    {
                        ErrorHappened?.Invoke(this, new ErrorHappenedEventArgs(processor.MoveResultErrorMessage));
                    }
                    if (postMoveResult == PostMoveResult.GameOverError)
                    {
                        inputResult = InputResult.GameOver;
                    }
                    else if (postMoveResult == PostMoveResult.NoError || postMoveResult == PostMoveResult.NotGameEndingError)
                    {
                        inputResult = InputResult.Continue;
                    }
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
                                UndoHappened?.Invoke(this, new UndoHappenedEventArgs(undoResult));
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

        // If the goal declared in the processor is reached an event should be triggered, but only once
        if (!goalReached && processor.HighestNumber >= processor.Goal)
        {
            MiscEventHappened?.Invoke(this, new MiscEventHappenedEventArgs(MiscEvent.GoalReached));
            goalReached = true;
        }

        // After all we need to handle the call the event that represents the end of an input cycle
        InputProcessed?.Invoke(this, new EventArgs());
        return inputResult;
    }

    /// <summary>
    /// Handles processor level events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    internal void PlayProcessorEventHappenedDispatcher(object? sender, PlayProcessorEventHappenedEventArgs args)
    {
        MiscEvent eventType = args.ProcessorEvent switch
        {
            PlayProcessorEvent.UndoCountChanged => MiscEvent.UndoCountChanged,
            PlayProcessorEvent.MaxNumberChanged => MiscEvent.MaxNumberChanged,
            PlayProcessorEvent.MaxLivesChanged => MiscEvent.MaxLivesChanged,
            _ => MiscEvent.Unknown
        };
        MiscEventHappened?.Invoke(this, new MiscEventHappenedEventArgs(eventType, args.NumberArg));
    }
}
