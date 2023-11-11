namespace Game2048.Interfaces;

public interface IJSONHandler
{
    IGameRepository DeserializeRepository(string jsonRepository);

    string SerializeRepository(IGameRepository repository);

    IList<(string Name, int Score)> DeserializeHighScores(string jsonHighScores);

    string SerializeHighScores(IList<(string Name, int Score)> highscores);
}
