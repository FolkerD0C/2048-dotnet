using Game2048.Shared.Models;
using Game2048.Repository;

namespace Game2048.Logic.Saving;

public interface IGameSaveHandler : ISerializerHandler<IGamePosition>
{
    IGameRepository GameRepository { get; }
    void Save();

    void Load();
}