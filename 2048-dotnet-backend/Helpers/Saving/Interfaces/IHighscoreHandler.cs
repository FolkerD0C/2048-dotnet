using Game2048.Backend.Repository;
using Game2048.Backend.Models;

namespace Game2048.Backend.Helpers.Saving;

public interface IHighscoreHandler : ISerializerHandler<IHighscore>
{
    IHighscoresRepository HighscoresData { get; }
    
    void Save();

    void Load();
}