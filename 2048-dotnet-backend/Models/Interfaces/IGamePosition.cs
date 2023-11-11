using System.Collections.Generic;
using Game2048.Backend.Helpers.Enums;

namespace Game2048.Backend.Models;

public interface IGamePosition : ISerializable
{
    public IList<IList<int>> Grid { get; }
    public int Score { get; }
    public bool CanMove { get; }
    public void Move(MoveDirection direction);
    public IGamePosition Copy();
    public void PlaceTile(int vertical, int horizontal, int tileValue);
}