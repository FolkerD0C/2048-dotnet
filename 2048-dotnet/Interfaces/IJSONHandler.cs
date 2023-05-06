namespace Game2048.Interfaces;

public interface IJSONHandler
{
    IGameRepository DeserializeRepository(string jsonRepository);

    string SerializeRepository(IGameRepository repository);

    IList<(string, int)> DeserializeHighScores(string jsonHighScores);

    string SerializeHighScores(IList<(string, int)> highscores);
}
