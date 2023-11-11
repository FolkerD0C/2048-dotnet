using Game2048.Backend.Models;
using Game2048.Backend.Repository;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Linq;

namespace Game2048.Backend.Helpers.Saving;

public class HighScoreHandler : FileHandler, IHighscoreHandler
{
    IHighscoresRepository highscoresData;

    IHighscoresRepository IHighscoreHandler.HighscoresData => highscoresData;

    public HighScoreHandler(string saveFilePath) : base(saveFilePath)
    {
        highscoresData = new HighscoresRepository();
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
        var jsonObjects = JsonNode.Parse(data)?.AsArray() ?? throw new NullReferenceException();
        highscoresData = new HighscoresRepository();
        foreach (var item in jsonObjects)
        {
            if (item is null)
            {
                throw new NullReferenceException();
            }
            highscoresData.AddHighscore(Deconvert(item.AsObject().ToString()));
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
}
