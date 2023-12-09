using Game2048.Config;
using Game2048.Repository;
using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Models;
using System;
using System.IO;
using System.Text.Json;

namespace Game2048.Logic.Saving;

/// <summary>
/// A class that represents a manager for high score saving and loading.
/// </summary>
internal class HighScoreSaveHandler : FileHandler, IHighscoreSaveHandler
{
    IHighscoreRepository highscoresData;

    public IHighscoreRepository HighscoresData => highscoresData;

    /// <summary>
    /// Creates a new instance of the <see cref="HighScoreSaveHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath">The full path of the file that contains the high scores.</param>
    public HighScoreSaveHandler(string saveFilePath) : base(saveFilePath)
    {
        highscoresData = new HighscoreRepository();
        if (!File.Exists(saveFilePath))
        {
            File.Create(saveFilePath).Close();
            File.WriteAllText(saveFilePath, "[]");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HighScoreSaveHandler"/> class.
    /// </summary>
    public HighScoreSaveHandler() : this(GameData.HighscoresFilePath)
    { }

    public void Load()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        string jsonData = Read();
        var deserializedData = JsonSerializer.Deserialize<HighscoreSaveData>(jsonData, serializerOptions) ?? throw new NullReferenceException("Failed to load highscores.");
        highscoresData = new HighscoreRepository(deserializedData);
    }

    public void Save()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        Write(JsonSerializer.Serialize(highscoresData.GetSaveDataObject(), serializerOptions));
    }

    public void AddNewHighscore(string playerName, int score)
    {
        highscoresData.AddNewHighscore(new Highscore()
        {
            PlayerName = playerName,
            PlayerScore = score
        });
    }
}
