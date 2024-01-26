using _2048ish.Base.Enums;
using _2048ish.Base.Models;
using Game2048.Processors;
using Game2048.Processors.Enums;
using Game2048.Processors.EventHandlers;
using Game2048.Processors.Helpers;
using Game2048.Processors.SaveDataObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Test
{
    internal class PlayProcessorEventHelper
    {
        internal List<PlayProcessorEventHappenedEventArgs> CapturedEvents { get; private set; }

        public PlayProcessorEventHelper()
        {
            CapturedEvents = new List<PlayProcessorEventHappenedEventArgs>();
        }

        internal void EventCapturer(object? sender, PlayProcessorEventHappenedEventArgs args)
        {
            CapturedEvents.Add(args);
        }
    }

    [TestFixture]
    public class PlayProcessorTests
    {
        public enum PlayProcessorCaseHelper
        {
            Empty,
            Some,
            Full,
            Big
        }

#pragma warning disable CS8618
        IPlayProcessor processorWithEmptyState;
        IPlayProcessor processorWithSomeState;
        IPlayProcessor processorWithFullState;
        IPlayProcessor processorWithBigState;

        NewGameConfiguration gameConfig;

        PlayProcessorEventHelper eventHelper;
#pragma warning restore

        [SetUp]
        public void Setup()
        {
            GameState emptyState = new()
            {
                Grid = new()
                {
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0 }
                }
            };
            GameState someState = new()
            {
                Grid = new()
                {
                    new() { 1, 2, 4, 0 },
                    new() { 1, 2, 0, 0 },
                    new() { 1, 0, 0, 0 },
                    new() { 0, 0, 4, 0 }
                }
            };
            GameState fullState = new()
            {
                Grid = new()
                {
                    new() { 1, 2, 4, 8 },
                    new() { 1, 2, 4, 8 },
                    new() { 2, 2, 2, 2 },
                    new() { 4, 4, 4, 4 }
                }
            };
            GameState bigState = new()
            {
                Grid = new()
                {
                    new() { 1, 2, 4, 8, 1, 2, 4, 8, 2, 2, 2, 2, 0, 0, 0, 0 },
                    new() { 1, 2, 4, 8, 1, 2, 4, 8, 2, 2, 2, 0, 0, 0, 0, 4 },
                    new() { 1, 2, 4, 8, 1, 2, 4, 8, 2, 2, 0, 0, 0, 0, 4, 4 },
                    new() { 1, 2, 4, 8, 1, 2, 4, 8, 2, 0, 0, 0, 0, 4, 4, 4 },
                    new() { 1, 2, 4, 8, 1, 2, 4, 8, 0, 0, 0, 0, 4, 4, 4, 4 },
                    new() { 1, 2, 4, 8, 1, 2, 4, 0, 0, 0, 0, 2, 4, 4, 4, 4 },
                    new() { 1, 2, 4, 8, 1, 2, 0, 0, 0, 0, 2, 2, 4, 4, 4, 4 },
                    new() { 1, 2, 4, 8, 1, 0, 0, 0, 0, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 1, 2, 4, 8, 0, 0, 0, 0, 2, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 1, 2, 4, 0, 0, 0, 0, 8, 2, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 1, 2, 0, 0, 0, 0, 4, 8, 2, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 1, 0, 0, 0, 0, 2, 4, 8, 2, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 0, 0, 0, 0, 1, 2, 4, 8, 2, 2, 2, 2, 4, 4, 4, 4 },
                    new() { 0, 0, 0, 8, 1, 2, 4, 8, 2, 2, 2, 2, 4, 4, 4, 0 },
                    new() { 0, 0, 4, 8, 1, 2, 4, 8, 2, 2, 2, 2, 4, 4, 0, 0 },
                    new() { 0, 2, 4, 8, 1, 2, 4, 8, 2, 2, 2, 2, 4, 0, 0, 0 }
                }
            };

            GameSaveData emptySaveData = new();
            emptySaveData.Populate(2048, new() { 2, 4 }, 4, 4, 5, "empty", 5, new() { emptyState });
            GameSaveData someSaveData = new();
            someSaveData.Populate(2048, new() { 2, 4 }, 4, 4, 5, "same", 5, new() { someState });
            GameSaveData fullSaveData = new();
            fullSaveData.Populate(2048, new() { 2, 4 }, 4, 4, 5, "full", 5, new() { fullState });
            GameSaveData bigSaveData = new();
            bigSaveData.Populate(2048, new() { 2, 16 }, 16, 16, 5, "big", 5, new() { bigState });

            processorWithEmptyState = new PlayProcessor(emptySaveData);
            processorWithSomeState = new PlayProcessor(someSaveData);
            processorWithFullState = new PlayProcessor(fullSaveData);
            processorWithBigState = new PlayProcessor(bigSaveData);

            gameConfig = new()
            {
                AcceptedSpawnables = new() { 3, 6 },
                Goal = 100000,
                MaxLives = 3,
                MaxUndos = 3,
                GridHeight = 5,
                GridWidth = 5,
                StarterTiles = 10
            };

            eventHelper = new();
        }

        [Test]
        public void InvalidCreation()
        {
            GameState invalidState = new()
            {
                Grid = new(),
                Score = -1
            };

            GameSaveData invalidSaveData = new();
            invalidSaveData.Populate(-1, new(), 0, 0, 5, "", 5, new() { invalidState });

            Assert.Throws<InvalidOperationException>(() => { IPlayProcessor processorWithInvalidState = new PlayProcessor(invalidSaveData); });
        }

        [TestCase(PlayProcessorCaseHelper.Empty)]
        [TestCase(PlayProcessorCaseHelper.Some)]
        [TestCase(PlayProcessorCaseHelper.Full)]
        [TestCase(PlayProcessorCaseHelper.Big)]
        public void PostMoveActions(PlayProcessorCaseHelper testCase) // Includes placing a random number and getting current max number
        {
            switch (testCase)
            {
                case PlayProcessorCaseHelper.Empty:
                    {
                        var copy = processorWithEmptyState.CurrentGameState.Copy();

                        processorWithEmptyState.PostMoveActions();

                        Assert.That(!processorWithEmptyState.CurrentGameState.StateEquals(copy));
                        break;
                    }
                case PlayProcessorCaseHelper.Some:
                    {
                        var copy = processorWithSomeState.CurrentGameState.Copy();

                        processorWithSomeState.PostMoveActions();

                        Assert.That(!processorWithSomeState.CurrentGameState.StateEquals(copy));
                        break;
                    }
                case PlayProcessorCaseHelper.Full:
                    {
                        var copy = processorWithFullState.CurrentGameState.Copy();

                        processorWithFullState.PostMoveActions();

                        Assert.That(processorWithFullState.CurrentGameState.StateEquals(copy));
                        break;
                    }
                case PlayProcessorCaseHelper.Big:
                    {
                        var copy = processorWithBigState.CurrentGameState.Copy();

                        processorWithBigState.PostMoveActions();

                        Assert.That(!processorWithBigState.CurrentGameState.StateEquals(copy));
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid test case.");
                    }
            }
        }

        [TestCase(PlayProcessorCaseHelper.Empty)]
        [TestCase(PlayProcessorCaseHelper.Some)]
        [TestCase(PlayProcessorCaseHelper.Full)]
        [TestCase(PlayProcessorCaseHelper.Big)]
        public void MoveUp(PlayProcessorCaseHelper testCase)
        {
            bool actualMoveHappened;
            bool expectedMoveHappened;
            GameState actualGameState;
            GameState expectedGameState;
            switch (testCase)
            {
                case PlayProcessorCaseHelper.Empty:
                    {
                        actualMoveHappened = processorWithEmptyState.MoveGrid(MoveDirection.Up);
                        expectedMoveHappened = false;
                        actualGameState = processorWithEmptyState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Some:
                    {
                        actualMoveHappened = processorWithSomeState.MoveGrid(MoveDirection.Up);
                        expectedMoveHappened = true;
                        actualGameState = processorWithSomeState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 2, 4, 8, 0 },
                                new() { 1, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 14
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Full:
                    {
                        actualMoveHappened = processorWithFullState.MoveGrid(MoveDirection.Up);
                        expectedMoveHappened = true;
                        actualGameState = processorWithFullState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 2, 4, 8, 16 },
                                new() { 2, 2, 2, 2 },
                                new() { 4, 4, 4, 4 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 30
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Big:
                    {
                        actualMoveHappened = processorWithBigState.MoveGrid(MoveDirection.Up);
                        expectedMoveHappened = true;
                        actualGameState = processorWithBigState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                            },
                            Score = 648
                        };
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case.");
            }

            Assert.That(expectedMoveHappened == actualMoveHappened);
            Assert.That(expectedGameState.StateEquals(actualGameState));
        }

        [TestCase(PlayProcessorCaseHelper.Empty)]
        [TestCase(PlayProcessorCaseHelper.Some)]
        [TestCase(PlayProcessorCaseHelper.Full)]
        [TestCase(PlayProcessorCaseHelper.Big)]
        public void MoveDown(PlayProcessorCaseHelper testCase)
        {
            bool actualMoveHappened;
            bool expectedMoveHappened;
            GameState actualGameState;
            GameState expectedGameState;
            switch (testCase)
            {
                case PlayProcessorCaseHelper.Empty:
                    {
                        actualMoveHappened = processorWithEmptyState.MoveGrid(MoveDirection.Down);
                        expectedMoveHappened = false;
                        actualGameState = processorWithEmptyState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Some:
                    {
                        actualMoveHappened = processorWithSomeState.MoveGrid(MoveDirection.Down);
                        expectedMoveHappened = true;
                        actualGameState = processorWithSomeState.CurrentGameState;
                        expectedGameState = new()
                        {

                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 1, 0, 0, 0 },
                                new() { 2, 4, 8, 0 }
                            },
                            Score = 14
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Full:
                    {
                        actualMoveHappened = processorWithFullState.MoveGrid(MoveDirection.Down);
                        expectedMoveHappened = true;
                        actualGameState = processorWithFullState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 2, 2, 8, 16 },
                                new() { 2, 4, 2, 2 },
                                new() { 4, 4, 4, 4 }
                            },
                            Score = 30
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Big:
                    {
                        actualMoveHappened = processorWithBigState.MoveGrid(MoveDirection.Down);
                        expectedMoveHappened = true;
                        actualGameState = processorWithBigState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 },
                                new() { 2, 4, 8, 16, 2, 4, 8, 16, 4, 4, 4, 4, 8, 8, 8, 8 }
                            },
                            Score = 648
                        };
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case.");
            }

            Assert.That(expectedMoveHappened == actualMoveHappened);
            Assert.That(expectedGameState.StateEquals(actualGameState));
        }

        [TestCase(PlayProcessorCaseHelper.Empty)]
        [TestCase(PlayProcessorCaseHelper.Some)]
        [TestCase(PlayProcessorCaseHelper.Full)]
        [TestCase(PlayProcessorCaseHelper.Big)]
        public void MoveRight(PlayProcessorCaseHelper testCase)
        {
            bool actualMoveHappened;
            bool expectedMoveHappened;
            GameState actualGameState;
            GameState expectedGameState;
            switch (testCase)
            {
                case PlayProcessorCaseHelper.Empty:
                    {
                        actualMoveHappened = processorWithEmptyState.MoveGrid(MoveDirection.Right);
                        expectedMoveHappened = false;
                        actualGameState = processorWithEmptyState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Some:
                    {
                        actualMoveHappened = processorWithSomeState.MoveGrid(MoveDirection.Right);
                        expectedMoveHappened = true;
                        actualGameState = processorWithSomeState.CurrentGameState;
                        expectedGameState = new()
                        {

                            Grid = new()
                            {
                                new() { 0, 1, 2, 4 },
                                new() { 0, 0, 1, 2 },
                                new() { 0, 0, 0, 1 },
                                new() { 0, 0, 0, 4 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Full:
                    {
                        actualMoveHappened = processorWithFullState.MoveGrid(MoveDirection.Right);
                        expectedMoveHappened = true;
                        actualGameState = processorWithFullState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 1, 2, 4, 8 },
                                new() { 1, 2, 4, 8 },
                                new() { 0, 0, 4, 4 },
                                new() { 0, 0, 8, 8 }
                            },
                            Score = 24
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Big:
                    {
                        actualMoveHappened = processorWithBigState.MoveGrid(MoveDirection.Right);
                        expectedMoveHappened = true;
                        actualGameState = processorWithBigState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 4, 4 },
                                new() { 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 2, 4, 4 },
                                new() { 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 4, 8 },
                                new() { 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 2, 4, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 2, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 1, 2, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 4, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 4, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 4, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 4, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 4, 8, 4, 4, 8, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 8, 1, 2, 4, 8, 4, 4, 4, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 0, 4, 8, 1, 2, 4, 8, 4, 4, 8 },
                                new() { 0, 0, 0, 0, 0, 0, 2, 4, 8, 1, 2, 4, 8, 4, 4, 4 }
                            },
                            Score = 264
                        };
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case.");
            }

            Assert.That(expectedMoveHappened == actualMoveHappened);
            Assert.That(expectedGameState.StateEquals(actualGameState));
        }

        [TestCase(PlayProcessorCaseHelper.Empty)]
        [TestCase(PlayProcessorCaseHelper.Some)]
        [TestCase(PlayProcessorCaseHelper.Full)]
        [TestCase(PlayProcessorCaseHelper.Big)]
        public void MoveLeft(PlayProcessorCaseHelper testCase)
        {
            bool actualMoveHappened;
            bool expectedMoveHappened;
            GameState actualGameState;
            GameState expectedGameState;
            switch (testCase)
            {
                case PlayProcessorCaseHelper.Empty:
                    {
                        actualMoveHappened = processorWithEmptyState.MoveGrid(MoveDirection.Left);
                        expectedMoveHappened = false;
                        actualGameState = processorWithEmptyState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 },
                                new() { 0, 0, 0, 0 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Some:
                    {
                        actualMoveHappened = processorWithSomeState.MoveGrid(MoveDirection.Left);
                        expectedMoveHappened = true;
                        actualGameState = processorWithSomeState.CurrentGameState;
                        expectedGameState = new()
                        {

                            Grid = new()
                            {
                                new() { 1, 2, 4, 0 },
                                new() { 1, 2, 0, 0 },
                                new() { 1, 0, 0, 0 },
                                new() { 4, 0, 0, 0 }
                            },
                            Score = 0
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Full:
                    {
                        actualMoveHappened = processorWithFullState.MoveGrid(MoveDirection.Left);
                        expectedMoveHappened = true;
                        actualGameState = processorWithFullState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 1, 2, 4, 8 },
                                new() { 1, 2, 4, 8 },
                                new() { 4, 4, 0, 0 },
                                new() { 8, 8, 0, 0 }
                            },
                            Score = 24
                        };
                        break;
                    }
                case PlayProcessorCaseHelper.Big:
                    {
                        actualMoveHappened = processorWithBigState.MoveGrid(MoveDirection.Left);
                        expectedMoveHappened = true;
                        actualGameState = processorWithBigState.CurrentGameState;
                        expectedGameState = new()
                        {
                            Grid = new()
                            {
                                new() { 1, 2, 4, 8, 1, 2, 4, 8, 4, 4, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 2, 4, 8, 4, 2, 4, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 2, 4, 8, 4, 8, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 2, 4, 8, 2, 8, 4, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 2, 4, 8, 8, 8, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 2, 4, 2, 8, 8, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 4, 2, 8, 8, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 1, 4, 2, 8, 8, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 4, 4, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 4, 4, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 4, 4, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 4, 4, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 1, 2, 4, 8, 4, 4, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 8, 1, 2, 4, 8, 4, 4, 8, 4, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 4, 8, 1, 2, 4, 8, 4, 4, 8, 0, 0, 0, 0, 0, 0, 0 },
                                new() { 2, 4, 8, 1, 2, 4, 8, 4, 4, 4, 0, 0, 0, 0, 0, 0 }
                            },
                            Score = 264
                        };
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case.");
            }

            Assert.That(expectedMoveHappened == actualMoveHappened);
            Assert.That(expectedGameState.StateEquals(actualGameState));
        }

        [Test]
        public void CreateNewGame()
        {
            IPlayProcessor playProcessor = new PlayProcessor(gameConfig);

            Assert.That(playProcessor.RemainingLives == 3);
            Assert.That(playProcessor.GridWidth == 5);
            Assert.That(playProcessor.GridHeight == 5);
            Assert.That(playProcessor.PlayerName == string.Empty);
            Assert.That(playProcessor.Goal == 100000);

            GameState starterState = playProcessor.CurrentGameState;
            GameState expectedState = new()
            {
                Grid = new()
                {
                    new() { 0, 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0, 0 },
                    new() { 0, 0, 0, 0, 0 }
                },
                Score = 0
            };

            Assert.That(starterState.StateEquals(expectedState));
        }

        [Test]
        public void CreateNewGameAndStart()
        {
            IPlayProcessor playProcessor = new PlayProcessor(gameConfig);
            playProcessor.PlayProcessorEventHappened += eventHelper.EventCapturer;
            playProcessor.StartGameActions();

            IEnumerable<int> nonZeroTiles = playProcessor.CurrentGameState.Grid
                .Aggregate(Enumerable.Empty<int>(), (all, current) => all.Concat(current)).Where(x => x != 0);

            int[] possibleTiles = new int[] { 3, 6 };

            Assert.That(playProcessor.HighestNumber != 0);
            Assert.That(nonZeroTiles.Count() == 10);
            Assert.That(nonZeroTiles.All(x => possibleTiles.Contains(x)));
            Assert.That(eventHelper.CapturedEvents.Count == 0);
        }

        [Test]
        public void Undo()
        {
            GameState current = new()
            {
                Grid = new() { new() { 1, 1, 1 }, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                Score = 100
            };
            GameState next = new()
            {
                Grid = new() { new() { 4, 4, 4 }, new() { 5, 5, 5 }, new() { 6, 6, 6 } },
                Score = 200
            };
            GameSaveData saveData = new();
            saveData.Populate(1000, new() { 1, 2, 3, 4, 5, 6 }, 3, 3, 1, string.Empty, 1, new() { current, next });
            IPlayProcessor playProcessor = new PlayProcessor(saveData);
            playProcessor.PlayProcessorEventHappened += eventHelper.EventCapturer;

            Assert.That(playProcessor.CurrentGameState.StateEquals(current));

            GameState newState = playProcessor.Undo() ?? throw new AssertionException("Expected a GameState object, but got null");

            Assert.That(playProcessor.RemainingUndos == 0);
            Assert.That(newState.StateEquals(next));
            Assert.That(eventHelper.CapturedEvents.Count == 1);

            PlayProcessorEventHappenedEventArgs happenedEvent = eventHelper.CapturedEvents[0];

            Assert.That(happenedEvent.ProcessorEvent == PlayProcessorEvent.UndoCountChanged);
            Assert.That(happenedEvent.NumberArg == 0);
        }

        [Test]
        public void GridStuck()
        {
            GameSaveData saveData = new();
            saveData.Populate(100000, new() { 1 }, 3, 3, 0, string.Empty, 2, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 2, 4, 2 },
                        new() { 4, 2, 4 },
                        new() { 4, 2, 0 }
                    }
                }
            });
            IPlayProcessor playProcessor = new PlayProcessor(saveData);
            playProcessor.PlayProcessorEventHappened += eventHelper.EventCapturer;

            bool moveHappened = playProcessor.MoveGrid(MoveDirection.Right);

            Assert.That(moveHappened);

            PostMoveResult moveResult = playProcessor.PostMoveActions();

            Assert.That(moveResult == PostMoveResult.NotGameEndingError);
            Assert.That(playProcessor.RemainingLives == 1);
            Assert.That(playProcessor.MoveResultErrorMessage != string.Empty);
            Assert.That(eventHelper.CapturedEvents.Count == 1);

            PlayProcessorEventHappenedEventArgs happenedEvent = eventHelper.CapturedEvents[0];

            Assert.That(happenedEvent.ProcessorEvent == PlayProcessorEvent.MaxLivesChanged);
            Assert.That(happenedEvent.NumberArg == 1);
        }

        [Test]
        public void NoMoreLives()
        {
            GameSaveData saveData = new();
            saveData.Populate(100000, new() { 1 }, 3, 3, 0, string.Empty, 1, new()
            {
                new()
                {
                    Grid = new()
                    {
                        new() { 2, 4, 2 },
                        new() { 4, 2, 4 },
                        new() { 4, 2, 0 }
                    }
                }
            });
            IPlayProcessor playProcessor = new PlayProcessor(saveData);

            bool moveHappened = playProcessor.MoveGrid(MoveDirection.Right);

            Assert.That(moveHappened);

            PostMoveResult moveResult = playProcessor.PostMoveActions();

            Assert.That(moveResult == PostMoveResult.GameOverError);
            Assert.That(playProcessor.RemainingLives == 0);
            Assert.That(playProcessor.MoveResultErrorMessage != string.Empty);
        }
    }
}
