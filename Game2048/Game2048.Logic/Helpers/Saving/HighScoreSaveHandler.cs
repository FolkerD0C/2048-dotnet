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
internal class HighscoreSaveHandler
{
    readonly string saveFilePath;

    /// <summary>
    /// The repository to serialize or desiarilize.
    /// </summary>
    public IHighscoreRepository HighscoresData { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="HighscoreSaveHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath">The full path of the file that contains the high scores.</param>
    public HighscoreSaveHandler(string saveFilePath)
    {
        this.saveFilePath = saveFilePath;
        HighscoresData = new HighscoreRepository();
        if (!File.Exists(saveFilePath))
        {
            File.Create(saveFilePath).Close();
            File.WriteAllText(saveFilePath, "[]");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HighscoreSaveHandler"/> class.
    /// </summary>
    public HighscoreSaveHandler() : this(GameData.HighscoresFilePath)
    { }

    /// <summary>
    /// Loads the highscores from a file.
    /// </summary>
    public void Load()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        string jsonData = File.ReadAllText(saveFilePath);
        var deserializedData = JsonSerializer.Deserialize<HighscoreSaveData>(jsonData, serializerOptions) ?? throw new NullReferenceException("Failed to load highscores.");
        HighscoresData = new HighscoreRepository(deserializedData);
    }

    /// <summary>
    /// Saves the highscores to a file.
    /// </summary>
    public void Save()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        File.WriteAllText(saveFilePath, JsonSerializer.Serialize(HighscoresData.GetSaveDataObject(), serializerOptions));
    }

    /// <summary>
    /// Adds a new highscore to <see cref="HighscoresData"/>.
    /// </summary>
    /// <param name="playerName">The name of the player who set a high score.</param>
    /// <param name="score">The score that has been set by <paramref name="playerName"/>.</param>
    public void AddNewHighscore(string playerName, int score)
    {
        HighscoresData.AddNewHighscore(new Highscore()
        {
            PlayerName = playerName,
            PlayerScore = score
        });
    }
}
