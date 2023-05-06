using Game2048.Classes;

namespace Game2048.Interfaces;

public interface IGridInstance
{
    public int[,] Grid { get; }

    public int Score { get; }

    public Queue<(int Vertical, int Horizontal, int Value)> MoveQueue
    { get; }

    public Queue<int?> ScoreQueue
    { get; }

    public void UpdateField(int vertical, int horizontal, int value);

    public void CheckIfCanMove();

    public IGridInstance Move(MoveDirection? direction);
}
