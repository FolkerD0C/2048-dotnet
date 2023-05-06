using Game2048.Interfaces;

namespace Game2048.Classes;

class JSONHandler : IJSONHandler
{
    public IGameRepository DeserializeRepository(string jsonRepository)
    {
        return null;
    }

    public string SerializeRepository(IGameRepository repository)
    {
        return null;
    }

    public IList<(string, int)> DeserializeHighScores(string jsonHighScores)
    {
        return null;
    }

    public string SerializeHighScores(IList<(string, int)> highscores)
    {
        return null;
    }
}
