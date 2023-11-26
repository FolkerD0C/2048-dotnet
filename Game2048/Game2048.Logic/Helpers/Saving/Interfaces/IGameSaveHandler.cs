using Game2048.Repository;
using Game2048.Shared.Models;

namespace Game2048.Logic.Saving;

public interface IGameSaveHandler
{
    IGameRepository GameRepository { get; }
    void UpdateFilePath(string filePath);
    SaveResult Save();

    void Load();
}