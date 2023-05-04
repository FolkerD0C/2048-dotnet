namespace Game2048.Classes;

class Repository
{
    LinkedList<GridInstance> undoChain;

    public LinkedList<GridInstance> UndoChain
    {
        get
        {
            return undoChain;
        }
        private set
        {
            undoChain = value;
            UndoCountChanged?.Invoke(undoChain.Count - 1);
        }
    }

    int lives;

    public int Lives
    {
        get
        {
            return lives;
        }
        private set
        {
            lives = value;
            LivesCountChanged?.Invoke(lives);
        }
    }

    public event Action<int[,], int> UndoHappened;

    public event Action<int, int, int> GridUpdated;

    public event Action<int> ScoreUpdated;

    public event Action<int[,]> Reached2048;

    public event Action<int> UndoCountChanged;

    public event Action<int> LivesCountChanged;

    public Repository()
    {
        undoChain = new LinkedList<GridInstance>();
    }

    public void Initialize()
    {
        GridInstance first = new GridInstance();
        undoChain.AddFirst(first);
        PutTwoOrFour();
        PutTwoOrFour();
    }

    void PutTwoOrFour()
    {
        Random rnd = new Random();
        var emptyTiles = GetEmptyPositions();
        var position = emptyTiles[rnd.Next(0, emptyTiles.Count)];
        var twoOrFour = rnd.NextDouble() < 0.5 ? 2 : 4;
        UndoChain.First.Value.UpdateField(position.Vertical, position.Horizontal, twoOrFour);
        GridUpdated?.Invoke(position.Vertical, position.Horizontal, twoOrFour);
        ScoreUpdated?.Invoke(undoChain.First.Value.Score);
    }

    void GameWon()
    {
        Reached2048?.Invoke(UndoChain.First.Value.Grid);
    }

    List<(int Vertical, int Horizontal)> GetEmptyPositions()
    {
        List<(int Vertical, int Horizontal)> result = new List<(int Vertical, int Horizontal)>();
        for (int i = 0; i < undoChain.First.Value.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < undoChain.First.Value.Grid.GetLength(1); j++)
            {
                if (undoChain.First.Value.Grid[i, j] == 0)
                {
                    result.Add((i, j));
                }
            }
        }
        return result;
    }

    public void Move(MoveDirection? input)
    {
        GridInstance next = UndoChain.First.Value.Move(input, UpdateHappened, GameWon);
        UpdateUndoChain(next);
    }

    void UpdateUndoChain(GridInstance grid)
    {
        UndoChain.AddFirst(grid);
        while (undoChain.Count > 7)
        {
            undoChain.RemoveLast();
        }
    }

    public void AddNewTile()
    {
        PutTwoOrFour();
    }

    public void Undo()
    {
        if (UndoChain.Count > 1)
        {
            UndoChain.RemoveFirst();
        }
        else
        {
            throw new UndoImpossibleException();
        }
        UndoHappened?.Invoke(UndoChain.First.Value.Grid, UndoChain.First.Value.Score);
    }

    void UpdateHappened(Queue<(int Vertical, int Horizontal, int Value)> moveQueue, Queue<int?> scoreQueue)
    {
        while(moveQueue.Count > 0)
        {
            var updateArgs = moveQueue.Dequeue();
            GridUpdated?.Invoke(updateArgs.Vertical, updateArgs.Horizontal, updateArgs.Value);
            if (scoreQueue.TryDequeue(out int? nextScore) && nextScore != null)
            {
                ScoreUpdated?.Invoke((int)nextScore);
            }
            if (updateArgs.Value > 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}
