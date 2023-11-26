using Game2048.Repository;

namespace Game2048.Logic.Saving;

public interface IHighscoreHandler
{
    IHighscoresRepository HighscoresData { get; }

    void Save();

    void Load();

    void AddNewHighscore(string playerName, int score);
}