using Game2048.Repository;
using Game2048.Shared.Models;

namespace Game2048.Logic.Saving;

public interface IHighscoreHandler : ISerializerHandler<IHighscore>
{
    IHighscoresRepository HighscoresData { get; }

    void Save();

    void Load();

    void AddNewHighscore(string playerName, int score);
}