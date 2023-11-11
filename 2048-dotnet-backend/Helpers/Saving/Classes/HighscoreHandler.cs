using Game2048.Backend.Models;
using Game2048.Backend.Repository;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Game2048.Backend.Helpers.Saving;

public class HighScoreHandler : FileHandler, IHighscoreHandler
{
    IHighscoresRepository highscoresData;

    IHighscoresRepository IHighscoreHandler.HighscoresData => highscoresData;

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

    public string Convert(IHighscore convertee)
    {
        return convertee.Serialize();
    }

    public IHighscore Deconvert(string deconvertee)
    {
        var highScoreObject = new Highscore();
        highScoreObject.Deserialize(deconvertee);
        return highScoreObject;
    }

    public void Load()
    {
        string data = Read();
        using var jsonDoc = JsonDocument.Parse(data);
        var jsonArray = jsonDoc.RootElement.EnumerateArray();
        highscoresData = new HighscoresRepository();
        foreach (var item in jsonArray)
        {
            highscoresData.AddHighscore(Deconvert(item.GetRawText()));
        }
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
        string result = "[";
        result += string.Join(",", highscoresData.HighScores.Select(hsdo => Convert(hsdo)));
        result += "]";
        Write(result);
    }

    public void AddNewHighscore(string playerName, int score)
    {
        highscoresData.AddNewHighscore(new Highscore(playerName, score));
    }
}
