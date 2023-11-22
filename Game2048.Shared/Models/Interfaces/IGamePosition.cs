using Game2048.Shared.Enums;
using System.Collections.Generic;

namespace Game2048.Shared.Models;

public interface IGamePosition : ISerializable
{
    // TODO make separate interface for backend and frontend
    // frontend interface should contain Grid and Score
    // backend should contain everything else and inherit from ISerializable
    public IList<IList<int>> Grid { get; }
    public int Score { get; }
    public bool CanMove { get; }
    IList<(int Vertical, int Horizontal)> GetEmptyTiles();
    public void Move(MoveDirection direction);
    public IGamePosition Copy();
    public void PlaceTile(int vertical, int horizontal, int tileValue);
}