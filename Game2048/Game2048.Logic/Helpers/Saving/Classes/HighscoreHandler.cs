using Game2048.Config;
using Game2048.Repository;
using Game2048.Shared.Models;
using System.IO;

namespace Game2048.Logic.Saving;

public class HighScoreHandler : FileHandler, IHighscoreHandler
{
    IHighscoresRepository highscoresData;

    public IHighscoresRepository HighscoresData => highscoresData;

    public HighScoreHandler(string saveFilePath) : base(saveFilePath)
    {
        highscoresData = new HighscoresRepository();
        if (!File.Exists(saveFilePath))
        {
            File.Create(saveFilePath).Close();
            File.WriteAllText(saveFilePath, "[]");
        }
    }

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
        highscoresData.AddNewHighscore(new Highscore(playerName, score));
    }
}
