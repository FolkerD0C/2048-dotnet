using _2048ish.Base.Models;
using Game2048.Config;
using Game2048.Managers;
using Game2048.Managers.Enums;
using Game2048.Managers.Models;
using Game2048.Managers.Saving;
using Game2048.Processors;
using Game2048.Processors.SaveDataObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Game2048.Test
{
    internal class GameSaveHandlerMocker : GameSaveHandler
    {
        readonly Dictionary<string, string> jsonSaveData = new()
        {
            {
                "Daniel", "{\"Goal\":2048,\"AcceptedSpawnables\":[2,4],\"GridHeight\":4,\"GridWidth\":4,\"MaxUndos\":5,\"PlayerName\":\"Daniel\"," +
                "\"RemainingLives\":5,\"UndoChain\":[{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100}]}"
            },
            {
                "Evelyn", "{\"Goal\":2048,\"AcceptedSpawnables\":[8,16],\"GridHeight\":4,\"GridWidth\":4,\"MaxUndos\":5,\"PlayerName\":\"Evelyn\"," +
                "\"RemainingLives\":1,\"UndoChain\":[{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100}," +
                "{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100}," +
                "{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100}," +
                "{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100}]}"
            },
            {
                "Francis", "{\"Goal\":2048,\"AcceptedSpawnables\":[2,4],\"GridHeight\":4,\"GridWidth\":4,\"MaxUndos\":5,\"PlayerName\":\"Francis\"," +
                "\"RemainingLives\":5,\"UndoChain\":[{\"Grid\":[[4,2,0,2],[2,2,4,2],[0,0,4,0],[2,4,0,8]],\"Score\":100000}]}"
            },
            {
                "Invalid", "{[\""
            }
        };

        internal override IPlayProcessor Load(string saveGameName)
        {
            if (!jsonSaveData.ContainsKey(saveGameName))
            {
                throw new FileNotFoundException();
            }
            var deserializedData = JsonSerializer.Deserialize<GameSaveData>(jsonSaveData[saveGameName]) ?? throw new AssertionException("Failed to deserialize game.");
            return new PlayProcessor(deserializedData);
        }

        internal override SaveResult Save(IPlayProcessor processor)
        {
            SaveResult result = new()
            {
                ResultType = SaveResultType.Unknown,
                Message = string.Empty
            };
            try
            {
                var serializedData = JsonSerializer.Serialize(processor.GetSaveDataObject());
                if (!jsonSaveData.ContainsKey(processor.PlayerName))
                {
                    jsonSaveData.Add(processor.PlayerName, serializedData);
                }
                else
                {
                    jsonSaveData[processor.PlayerName] = serializedData;
                }
                result.ResultType = SaveResultType.Success;
                result.Message = "Game successfully saved.";
            }
            catch (Exception exc)
            {
                result.ResultType = SaveResultType.Failure;
                result.Message = exc.Message;
            }
            return result;
        }

        internal override IEnumerable<string> GetSavedGames()
        {
            return jsonSaveData.Keys;
        }
    }

    internal class HighscoreSaveHandlerMocker : HighscoreSaveHandler
    {
        string jsonHighscoresData = "{\"Highscores\":[{\"PlayerName\":\"Alice\",\"PlayerScore\":3000},{\"PlayerName\":\"Bob\",\"PlayerScore\":2000},{\"PlayerName\":\"Candy\",\"PlayerScore\":1000}]}";

        IHighscoreProcessor highscoreData;
        internal override IHighscoreProcessor HighscoreData => highscoreData;

        internal HighscoreSaveHandlerMocker() : base(true)
        {
            highscoreData = new HighscoreProcessor();
        }

        internal override void Load()
        {
            var deserializedData = JsonSerializer.Deserialize<HighscoreSaveData>(jsonHighscoresData) ?? throw new AssertionException("Failed to deserialize highscores.");
            highscoreData = new HighscoreProcessor(deserializedData);
        }

        internal override void Save()
        {
            jsonHighscoresData = JsonSerializer.Serialize(highscoreData.GetSaveDataObject());
        }

        internal override void AddNewHighscore(string playerName, int score)
        {
            highscoreData.AddNewHighscore(new()
            {
                PlayerName = playerName,
                PlayerScore = score
            });
        }

        internal bool VerifyHighscoreSave(string playerName, int score)
        {
            return Regex.IsMatch(jsonHighscoresData, $"\"{playerName}\":\\s?{score}");
        }
    }

    [TestFixture]
    public class GameManagerTests
    {
#pragma warning disable CS8618
        IGameManager gameManager;
#pragma warning restore CS8618

        [SetUp]
        public void SetUp()
        {
            gameManager = new GameManager(new(), new HighscoreSaveHandlerMocker(), new GameSaveHandlerMocker());
        }

        [Test]
        public void GetDescription()
        {
            string actualDescription = gameManager.GetGameDescription();
            string expectedDescription = GameData.GameDescription;

            Assert.That(actualDescription, Is.EqualTo(expectedDescription));
        }

        [Test]
        public void GetHighscores()
        {
            List<Highscore> actualHighscores = gameManager.GetHighscores();
            List<Highscore> expectedHighscores = new()
            {
                new()
                {
                    PlayerName = "Alice",
                    PlayerScore = 3000
                },
                new()
                {
                    PlayerName = "Bob",
                    PlayerScore = 2000
                },
                new()
                {
                    PlayerName = "Candy",
                    PlayerScore = 1000
                }
            };
            Assert.That(Enumerable.Range(0, 3).All(i => actualHighscores[i].PlayerName.Equals(expectedHighscores[i].PlayerName) && actualHighscores[i].PlayerScore.Equals(expectedHighscores[i].PlayerScore)));
        }

        [Test]
        public void GetSaves()
        {
            List<string> actualSavedGames = gameManager.GetSavedGames().OrderBy(s => s).ToList();
            List<string> expectedSavedGames = new()
            {
                "Daniel", "Evelyn", "Francis", "Invalid"
            };

            Assert.That(Enumerable.Range(0, 4).All(i => actualSavedGames[i].Equals(expectedSavedGames[i])));
        }

        [Test]
        public void NewGame()
        {
            List<int> accepted = new() { 8, 16 };
            NewGameConfiguration gameConfig = new()
            {
                AcceptedSpawnables = accepted,
                Goal = 4096,
                MaxLives = 10,
                MaxUndos = 12,
                GridHeight = 8,
                GridWidth = 8,
                StarterTiles = 4
            };
            IPlayInstanceManager playInstanceManager = gameManager.NewGame(gameConfig);

            playInstanceManager.Start();

            IPlayProcessor processor = playInstanceManager.Processor;
            var notEmptyTiles = processor.CurrentGameState.Grid.Aggregate(new List<int>(), (all, current) => all.Concat(current).ToList()).Where(n => n != 0);

            Assert.That(processor.RemainingLives == 10);
            Assert.That(processor.RemainingUndos == 0);
            Assert.That(processor.GridHeight == 8);
            Assert.That(processor.GridWidth == 8);
            Assert.That(processor.PlayerName == string.Empty);
            Assert.That(notEmptyTiles.Count() == 4);
            Assert.That(notEmptyTiles.All(n => accepted.Contains(n)));
            Assert.That(processor.CurrentGameState.Score == 0);
        }

        [TestCase("Daniel")]
        [TestCase("Evelyn")]
        [TestCase("Francis")]
        [TestCase("Invalid")]
        [TestCase("NonExistent")]
        public void LoadGame(string saveGameName)
        {
            switch (saveGameName)
            {
                case "Daniel":
                    {
                        IPlayInstanceManager playInstanceManager = gameManager.LoadGame(saveGameName);
                        Assert.That(playInstanceManager.PlayerName.Equals(saveGameName));
                        break;
                    }
                case "Evelyn":
                    {
                        IPlayInstanceManager playInstanceManager = gameManager.LoadGame(saveGameName);
                        Assert.That(playInstanceManager.Processor.RemainingUndos == 3);
                        break;
                    }
                case "Francis":
                    {
                        IPlayInstanceManager playInstanceManager = gameManager.LoadGame(saveGameName);
                        Assert.That(playInstanceManager.PlayerScore == 100000);
                        break;
                    }
                case "Invalid":
                    {
                        Assert.Throws<JsonException>(() => gameManager.LoadGame(saveGameName));
                        break;
                    }
                case "NonExistent":
                    {
                        Assert.Throws<FileNotFoundException>(() => gameManager.LoadGame(saveGameName));
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case");
            }
        }

        [TestCase("ValidNewGame")]
        [TestCase("")]
        public void SaveGame(string changedPlayerName)
        {
            IPlayInstanceManager playInstanceManager = gameManager.NewGame(new());
            Guid playId = playInstanceManager.Id;
            switch (changedPlayerName)
            {
                case "ValidNewGame":
                    {
                        playInstanceManager.PlayerName = changedPlayerName;
                        SaveResult result = gameManager.SaveGame(playId);
                        Assert.That(result.ResultType == SaveResultType.Success);
                        break;
                    }
                case "":
                    {
                        playInstanceManager.PlayerName = changedPlayerName;
                        SaveResult result = gameManager.SaveGame(playId);
                        Assert.That(result.ResultType == SaveResultType.Failure);
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid test case");
            }
        }

        [Test]
        public void LoadAndAddHighscore()
        {
            IPlayInstanceManager playInstanceManager = gameManager.LoadGame("Francis");
            gameManager.AddHighscore(playInstanceManager.PlayerName, playInstanceManager.PlayerScore);
            Assert.That(gameManager.GetHighscores().First().PlayerName == "Francis");
        }
    }
}
