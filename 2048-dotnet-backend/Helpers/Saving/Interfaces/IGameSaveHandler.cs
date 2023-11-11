using Game2048.Backend.Models;
using Game2048.Backend.Repository;

namespace Game2048.Backend.Helpers.Saving;

public interface IGameSaveHandler : ISerializerHandler<IGamePosition>
{
    IGameRepository GameRepository { get; }
    void Save();

    void Load();
}