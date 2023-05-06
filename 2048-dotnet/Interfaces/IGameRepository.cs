using Game2048.Classes;

namespace Game2048.Interfaces;

public interface IGameRepository
{
    LinkedList<IGridInstance> UndoChain { get; }

    int Lives { get; }

    bool Reached2048 { get; }

    event EventHandler<(int[,], int)> UndoHappened;

    event EventHandler<(int, int, int)> GridUpdated;

    event EventHandler<int> ScoreUpdated;

    event EventHandler<int[,]> Reach2048;

    event EventHandler<int> UndoCountChanged;

    event EventHandler<int> LivesCountChanged;

    void Initialize();

    void Move(MoveDirection? input);

    void Undo();
}
