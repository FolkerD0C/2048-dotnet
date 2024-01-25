using _2048ish.Base.Models;
using Game2048.Processors;
using Game2048.Processors.SaveDataObjects;
using NUnit.Framework;
using System.Collections.Generic;

namespace Game2048.Test
{
    public class HighscoreProcessorTests
    {
#pragma warning disable CS8618
        IHighscoreProcessor highscoreProcessor;
#pragma warning restore CS8618

        [Test]
        public void AddOne()
        {
            highscoreProcessor = new HighscoreProcessor();
            highscoreProcessor.AddNewHighscore(new()
            {
                PlayerName = "Second",
                PlayerScore = 100
            });

            Assert.That(highscoreProcessor.HighScores.Count == 1);

            Highscore highscore = highscoreProcessor.HighScores[0];

            Assert.That(highscore.PlayerName == "Second");
            Assert.That(highscore.PlayerScore == 100);
        }

        [Test]
        public void AddTwo()
        {
            highscoreProcessor.AddNewHighscore(new()
            {
                PlayerName = "First",
                PlayerScore = 1000
            });
            highscoreProcessor.AddNewHighscore(new()
            {
                PlayerName = "Third",
                PlayerScore = 10
            });

            Assert.That(highscoreProcessor.HighScores.Count == 3);

            Assert.That(highscoreProcessor.HighScores[0].PlayerName == "First");
            Assert.That(highscoreProcessor.HighScores[0].PlayerScore == 1000);

            Assert.That(highscoreProcessor.HighScores[1].PlayerName == "Second");
            Assert.That(highscoreProcessor.HighScores[1].PlayerScore == 100);

            Assert.That(highscoreProcessor.HighScores[2].PlayerName == "Third");
            Assert.That(highscoreProcessor.HighScores[2].PlayerScore == 10);
        }

        [Test]
        public void AddToLoaded()
        {
            List<Highscore> highscores = new()
            {
                new()
                {
                    PlayerName = "First",
                    PlayerScore = 10
                },
                new()
                {
                    PlayerName = "Second",
                    PlayerScore = 9
                },
                new()
                {
                    PlayerName = "Fourth",
                    PlayerScore = 7
                },
                new()
                {
                    PlayerName = "Fifth",
                    PlayerScore = 6
                },
                new()
                {
                    PlayerName = "Sixth",
                    PlayerScore = 5
                }
            };
            HighscoreSaveData saveData = new();
            saveData.Populate(highscores);

            IHighscoreProcessor loadedHighscores = new HighscoreProcessor(saveData);

            Assert.That(loadedHighscores.HighScores.Count == 5);

            loadedHighscores.AddNewHighscore(new()
            {
                PlayerName = "Third",
                PlayerScore = 8
            });

            Assert.That(loadedHighscores.HighScores.Count == 6);
            Assert.That(loadedHighscores.HighScores[2].PlayerName == "Third");
            Assert.That(loadedHighscores.HighScores[2].PlayerScore == 8);
        }
    }
}
