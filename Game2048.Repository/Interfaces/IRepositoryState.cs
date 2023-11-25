using Game2048.Shared.Enums;
using System.Collections.Generic;

namespace Game2048.Repository;

public interface IRepositoryState
{
    public bool CanMove { get; }
    IList<(int Vertical, int Horizontal)> GetEmptyTiles();
    public void Move(MoveDirection direction);
    public IRepositoryState Copy();
    public void PlaceTile(int vertical, int horizontal, int tileValue);
}
