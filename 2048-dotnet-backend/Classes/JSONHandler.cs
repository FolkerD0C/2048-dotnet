using Game2048.Interfaces;
using System.Text.Json;

namespace Game2048.Classes;

class JSONHandler : IJSONHandler
{
    public IGameRepository DeserializeRepository(string jsonRepository)
    {
        var gameObject = JsonSerializer.Deserialize<SavedGameObject>(jsonRepository);
        IGameRepository repository = GameRepository.ConvertBack(gameObject);
        return repository;
    }

    public string SerializeRepository(IGameRepository repository)
    {
        var gameObject = GameRepository.Convert(repository);
        var options = new JsonSerializerOptions() { WriteIndented = true };
        string jsonObject = JsonSerializer.Serialize<SavedGameObject>(gameObject, options);
        return jsonObject;
    }

    public IList<(string Name, int Score)> DeserializeHighScores(string jsonHighScores)
    {
        var options = new JsonSerializerOptions() { IncludeFields = true };
        var highscores = JsonSerializer.Deserialize<List<(string Name, int Score)>>(jsonHighScores, options);
        return highscores;
    }

    public string SerializeHighScores(IList<(string Name, int Score)> highscores)
    {
        var options = new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true };
        string jsonHighScores = JsonSerializer.Serialize<List<(string Name, int Score)>>((List<(string Name, int Score)>)highscores, options);
        return jsonHighScores;
    }
}
