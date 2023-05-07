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
        string jsonObject = JsonSerializer.Serialize<SavedGameObject>(gameObject);
        return jsonObject;
    }

    public IList<(string, int)> DeserializeHighScores(string jsonHighScores)
    {
        var highscores = JsonSerializer.Deserialize<List<(string, int)>>(jsonHighScores);
        return highscores;
    }

    public string SerializeHighScores(IList<(string, int)> highscores)
    {
        string jsonHighScores = JsonSerializer.Serialize<List<(string, int)>>((List<(string, int)>)highscores);
        return jsonHighScores;
    }
}
