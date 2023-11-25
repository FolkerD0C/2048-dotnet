using Game2048.Config;
using Game2048.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Game2048.Repository;

public class HighscoresRepository : IHighscoresRepository
{
    IList<IHighscore> highScores;
    public IList<IHighscore> HighScores => highScores;

    public HighscoresRepository()
    {
        highScores = new List<IHighscore>();
    }

    void AddHighscore(IHighscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).ToList();
    }

    public void AddNewHighscore(IHighscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).Take(ConfigManager.GetConfigItem<int>("MaxHighscoresListLength")).ToList();
    }

    public string Serialize()
    {
        string jsonResult = "[";
        jsonResult += string.Join(",", highScores.Select(hsdo => hsdo.Serialize()));
        jsonResult += "]";
        return jsonResult;
    }

    public void Deserialize(string deserializee)
    {
        using var jsonDoc = JsonDocument.Parse(deserializee);
        var jsonArray = jsonDoc.RootElement.EnumerateArray();
        foreach (var item in jsonArray)
        {
            IHighscore highscore = new Highscore();
            highscore.Deserialize(item.GetRawText());
            AddHighscore(highscore);
        }
    }
}