using _2048ish.Base.Models;
using Game2048.Managers;
using Game2048.Managers.Enums;
using Game2048.Managers.EventHandlers;
using Game2048.Processors;
using Game2048.Processors.Helpers;
using Game2048.Processors.SaveDataObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Game2048.Test
{
    internal class PlayInstanceManagerEventHelper
    {
        int playStartedPriorityCounter;
        int moveHappenedPriorityCounter;
        int undoHappenedPriorityCounter;
        int errorHappenedPriorityCounter;
        int miscEventHappenedPriorityCounter;
        int playerNameChangedPriorityCounter;

        internal int InputProcessingFinishedCounter { get; private set; }
        internal int PlayEndedCounter { get; private set; }

        internal PriorityQueue<EventArgs, int> CustomEvents { get; private set; }

        public PlayInstanceManagerEventHelper()
        {
            playStartedPriorityCounter = 0;
            moveHappenedPriorityCounter = 100;
            undoHappenedPriorityCounter = 10100;
            errorHappenedPriorityCounter = 11100;
            miscEventHappenedPriorityCounter = 12100;
            playerNameChangedPriorityCounter = 22100;

            InputProcessingFinishedCounter = 0;
            PlayEndedCounter = 0;

            CustomEvents = new();
        }

        internal void OnPlayStarted(object? sender, PlayStartedEventArgs e)
        {
            CustomEvents.Enqueue(e, playStartedPriorityCounter++);
        }

        internal void OnMoveHappened(object? sender, MoveHappenedEventArgs e)
        {
            CustomEvents.Enqueue(e, moveHappenedPriorityCounter++);
        }

        internal void OnUndoHappened(object? sender, UndoHappenedEventArgs e)
        {
            CustomEvents.Enqueue(e, undoHappenedPriorityCounter++);
        }

        internal void OnErrorHappened(object? sender, ErrorHappenedEventArgs e)
        {
            CustomEvents.Enqueue(e, errorHappenedPriorityCounter++);
        }

        internal void OnMiscEventHappened(object? sender, MiscEventHappenedEventArgs e)
        {
            CustomEvents.Enqueue(e, miscEventHappenedPriorityCounter++);
        }

        internal void OnPlayerNameChanged(object? sender, PlayerNameChangedEventArgs e)
        {
            CustomEvents.Enqueue(e, playerNameChangedPriorityCounter++);
        }

        internal void OnInputProcessingFinished(object? sender, EventArgs args)
        {
            InputProcessingFinishedCounter++;
        }

        internal void OnPlayEnded(object? sender, EventArgs e)
        {
            PlayEndedCounter++;
        }
    }

    [TestFixture]
    public class PlayInstanceManagerTests
    {
#pragma warning disable CS8618
        PlayInstanceManagerEventHelper eventHelper;
#pragma warning restore CS8618

        [SetUp]
        public void Setup()
        {
            eventHelper = new();
        }

        [Test]
        public void StartAndEnd()
        {
            GameSaveData saveData = new();
            saveData.Populate(Guid.NewGuid().ToString(), 2048, new() { 1 }, 4, 4, 1, "Empty", 1, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 1024 },
                    },
                    Score = 100
                }
            });
            IPlayInstanceManager mostlyEmptyLoadedManager = new PlayInstanceManager(new PlayProcessor(saveData));
            mostlyEmptyLoadedManager.PlayStarted += eventHelper.OnPlayStarted;
            mostlyEmptyLoadedManager.PlayEnded += eventHelper.OnPlayEnded;
            mostlyEmptyLoadedManager.Start();
            mostlyEmptyLoadedManager.End();

            Assert.That(eventHelper.CustomEvents.Count == 1);
            Assert.That(eventHelper.PlayEndedCounter == 1);

            EventArgs firstEvent = eventHelper.CustomEvents.Dequeue();
            Assert.That(firstEvent is PlayStartedEventArgs);

            PlayStartedEventArgs startedArgs = (PlayStartedEventArgs)firstEvent;
            Assert.That(startedArgs.State.StateEquals(new()
            {
                Grid = new()
                {
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 1024 },
                },
                Score = 100
            }));
            Assert.That(startedArgs.RemainingUndos == 0);
            Assert.That(startedArgs.RemainingLives == 1);
            Assert.That(startedArgs.HighestNumber == 1024);
            Assert.That(startedArgs.GridHeight == 4);
            Assert.That(startedArgs.GridWidth == 4);
            Assert.That(startedArgs.PlayerName == "Empty");
        }

        [Test]
        public void MoveSome()
        {
            NewGameConfiguration gameConfig = new()
            {
                AcceptedSpawnables = new() { 2 },
                Goal = 64,
                MaxLives = 1,
                MaxUndos = 2,
                GridHeight = 3,
                GridWidth = 3,
                StarterTiles = 9
            };
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(gameConfig));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            playManager.HandleInput(GameInput.Right);
            InputResult result = playManager.HandleInput(GameInput.Down);
            playManager.End();

            int playStarted = 0;
            int moveHappened = 0;
            int maxNumberChanged = 0;
            int undoCountChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is MoveHappenedEventArgs)
                {
                    moveHappened++;
                }
                else if (e is not null && e is MiscEventHappenedEventArgs meh)
                {
                    if (meh.Event == MiscEvent.MaxNumberChanged)
                    {
                        maxNumberChanged++;
                    }
                    else if (meh.Event == MiscEvent.UndoCountChanged)
                    {
                        undoCountChanged++;
                    }
                    else
                    {
                        throw new AssertionException("Wrong event triggered.");
                    }
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(moveHappened == 2);
            Assert.That(maxNumberChanged == 2);
            Assert.That(undoCountChanged == 1);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 2);
            Assert.That(eventHelper.PlayEndedCounter == 1);
            Assert.That(result == InputResult.Continue);
        }

        [Test]
        public void MoveAndUndo()
        {
            GameSaveData saveData = new();
            saveData.Populate(Guid.NewGuid().ToString(), 2048, new() { 1 }, 4, 4, 2, "Empty", 1, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 1024 },
                    },
                    Score = 100
                }
            });
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(saveData));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            playManager.HandleInput(GameInput.Up);
            InputResult result = playManager.HandleInput(GameInput.Undo);
            playManager.End();

            int playStarted = 0;
            int moveHappened = 0;
            int undoHappened = 0;
            int undoCountChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is MoveHappenedEventArgs)
                {
                    moveHappened++;
                }
                else if (e is UndoHappenedEventArgs)
                {
                    undoHappened++;
                }
                else if (e is not null && e is MiscEventHappenedEventArgs meh)
                {
                    if (meh.Event == MiscEvent.UndoCountChanged)
                    {
                        undoCountChanged++;
                    }
                    else
                    {
                        throw new AssertionException("Wrong event triggered.");
                    }
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(moveHappened == 1);
            Assert.That(undoHappened == 1);
            Assert.That(undoCountChanged == 2);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 2);
            Assert.That(eventHelper.PlayEndedCounter == 1);
            Assert.That(result == InputResult.Continue);
        }

        [Test]
        public void GoalReached()
        {
            GameSaveData saveData = new();
            saveData.Populate(Guid.NewGuid().ToString(), 128, new() { 2 }, 4, 4, 1, "Empty", 1, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 0, 0 },
                        new() { 0, 0, 64, 64 },
                    },
                    Score = 100
                }
            });
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(saveData));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            playManager.HandleInput(GameInput.Up);
            InputResult result = playManager.HandleInput(GameInput.Right);
            playManager.End();

            int playStarted = 0;
            int moveHappened = 0;
            int goalReached = 0;
            int maxNumberChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is MoveHappenedEventArgs)
                {
                    moveHappened++;
                }
                else if (e is not null && e is MiscEventHappenedEventArgs meh)
                {
                    if (meh.Event == MiscEvent.MaxNumberChanged)
                    {
                        maxNumberChanged++;
                    }
                    else if (meh.Event == MiscEvent.GoalReached)
                    {
                        goalReached++;
                    }
                    else
                    {
                        throw new AssertionException("Wrong event triggered.");
                    }
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(moveHappened == 2);
            Assert.That(goalReached == 1);
            Assert.That(maxNumberChanged == 1);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 2);
            Assert.That(eventHelper.PlayEndedCounter == 1);
            Assert.That(result == InputResult.Continue);
        }

        [Test]
        public void CannotMoveError()
        {
            GameSaveData saveData = new();
            saveData.Populate(Guid.NewGuid().ToString(), 128, new() { 1 }, 4, 4, 1, "Empty", 2, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 2, 4, 2, 4 },
                        new() { 4, 2, 4, 2 },
                        new() { 2, 4, 2, 4 },
                        new() { 2, 4, 2, 0 },
                    },
                    Score = 100
                }
            });
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(saveData));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            InputResult result = playManager.HandleInput(GameInput.Right);
            playManager.End();

            int playStarted = 0;
            int moveHappened = 0;
            int errorHappened = 0;
            int maxLivesChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is MoveHappenedEventArgs)
                {
                    moveHappened++;
                }
                else if (e is ErrorHappenedEventArgs)
                {
                    errorHappened++;
                }
                else if (e is not null && e is MiscEventHappenedEventArgs meh)
                {
                    if (meh.Event == MiscEvent.MaxLivesChanged)
                    {
                        maxLivesChanged++;
                    }
                    else
                    {
                        throw new AssertionException("Wrong event triggered.");
                    }
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(moveHappened == 1);
            Assert.That(errorHappened == 1);
            Assert.That(maxLivesChanged == 1);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 1);
            Assert.That(eventHelper.PlayEndedCounter == 1);
            Assert.That(result == InputResult.Continue);
        }

        [Test]
        public void GameOver()
        {
            GameSaveData saveData = new();
            saveData.Populate(Guid.NewGuid().ToString(), 128, new() { 1 }, 4, 4, 1, "Empty", 1, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 2, 4, 2, 4 },
                        new() { 4, 2, 4, 2 },
                        new() { 2, 4, 2, 4 },
                        new() { 2, 4, 2, 0 },
                    },
                    Score = 100
                }
            });
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(saveData));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            InputResult result = playManager.HandleInput(GameInput.Right);
            playManager.End();

            int playStarted = 0;
            int moveHappened = 0;
            int errorHappened = 0;
            int maxLivesChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is MoveHappenedEventArgs)
                {
                    moveHappened++;
                }
                else if (e is ErrorHappenedEventArgs)
                {
                    errorHappened++;
                }
                else if (e is not null && e is MiscEventHappenedEventArgs meh)
                {
                    if (meh.Event == MiscEvent.MaxLivesChanged)
                    {
                        maxLivesChanged++;
                    }
                    else
                    {
                        throw new AssertionException("Wrong event triggered.");
                    }
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(moveHappened == 1);
            Assert.That(errorHappened == 1);
            Assert.That(maxLivesChanged == 1);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 1);
            Assert.That(eventHelper.PlayEndedCounter == 1);
            Assert.That(result == InputResult.GameOver);
        }

        [Test]
        public void ChangePlayerName()
        {
            NewGameConfiguration gameConfig = new()
            {
                AcceptedSpawnables = new() { 2 },
                Goal = 64,
                MaxLives = 1,
                MaxUndos = 2,
                GridHeight = 3,
                GridWidth = 3,
                StarterTiles = 9
            };
            IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(gameConfig));
            playManager.PlayStarted += eventHelper.OnPlayStarted;
            playManager.MoveHappened += eventHelper.OnMoveHappened;
            playManager.UndoHappened += eventHelper.OnUndoHappened;
            playManager.ErrorHappened += eventHelper.OnErrorHappened;
            playManager.MiscEventHappened += eventHelper.OnMiscEventHappened;
            playManager.PlayerNameChanged += eventHelper.OnPlayerNameChanged;
            playManager.InputProcessingFinished += eventHelper.OnInputProcessingFinished;
            playManager.PlayEnded += eventHelper.OnPlayEnded;

            playManager.Start();
            playManager.PlayerName = "Changed";
            playManager.End();

            int playStarted = 0;
            int playerNameChanged = 0;

            while (eventHelper.CustomEvents.Count > 0)
            {
                EventArgs e = eventHelper.CustomEvents.Dequeue();
                if (e is PlayStartedEventArgs)
                {
                    playStarted++;
                }
                else if (e is PlayerNameChangedEventArgs)
                {
                    playerNameChanged++;
                }
                else
                {
                    throw new AssertionException("Wrong event triggered.");
                }
            }

            Assert.That(playStarted == 1);
            Assert.That(playerNameChanged == 1);
            Assert.That(eventHelper.InputProcessingFinishedCounter == 0);
            Assert.That(eventHelper.PlayEndedCounter == 1);
        }
    }
}
