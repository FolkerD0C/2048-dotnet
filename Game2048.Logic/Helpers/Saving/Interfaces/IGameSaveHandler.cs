using Game2048.Repository;
using Game2048.Shared.Models;

namespace Game2048.Logic.Saving;

public interface IGameSaveHandler : ISerializerHandler<IGamePosition>
{
    IGameRepository GameRepository { get; }
    void Save();

    void Load();
}