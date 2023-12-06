using Game2048.Config;
using Game2048.Repository;
using Game2048.Shared.Models;
using System.IO;

namespace Game2048.Logic.Saving;

/// <summary>
/// A class that represents a manager for high score saving and loading.
/// </summary>
public class HighScoreHandler : FileHandler, IHighscoreHandler
{
    IHighscoresRepository highscoresData;

    public IHighscoresRepository HighscoresData => highscoresData;

    /// <summary>
    /// Creates a new instance of the <see cref="HighScoreHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath">The full path of the file that contains the high scores.</param>
    public HighScoreHandler(string saveFilePath) : base(saveFilePath)
    {
        highscoresData = new HighscoresRepository();
        if (!File.Exists(saveFilePath))
        {
            File.Create(saveFilePath).Close();
            File.WriteAllText(saveFilePath, "[]");
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HighScoreHandler"/> class.
    /// </summary>
    public HighScoreHandler() : this(GameData.HighscoresFilePath)
    { }

    public void Load()
    {
        string data = Read();
        highscoresData = new HighscoresRepository();
        highscoresData.Deserialize(data);
    }

    protected override string Read()
    {
        var readData = File.ReadAllText(saveFilePath);
        return readData;
    }

    protected override void Write(string fileContent)
    {
        File.WriteAllText(saveFilePath, fileContent);
    }

    public void Save()
    {
        Write(highscoresData.Serialize());
    }

    public void AddNewHighscore(string playerName, int score)
    {
        //highscoresData.AddNewHighscore(new Highscore(playerName, score));
    }
}
