using System.Collections.Generic;

namespace Game2048.Shared.Models;

public interface IGameState : ISerializable
{
    public IList<IList<int>> Grid { get; }
    public int Score { get; }
}
