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

    bool triggered2048;

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
        UpdateUndoChain(first);
        PutTwoOrFour();
        PutTwoOrFour();
        Lives = 5;
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
        GridInstance next = UndoChain.First.Value.Move(input);
        bool reached2048 = UpdateHappened(next.MoveQueue, next.ScoreQueue);
        UpdateUndoChain(next);
        PutTwoOrFour();
        if (!triggered2048 && reached2048)
        {
            Reached2048?.Invoke(UndoChain.First.Value.Grid);
        }
        try
        {
            UndoChain.First.Value.CheckIfCanMove();
        }
        catch(GridStuckException)
        {
            Lives -= 1;
            if (lives <= 0)
            {
                throw new GameOverException();
            }
            throw;
        }
    }

    void UpdateUndoChain(GridInstance grid)
    {
        UndoChain.AddFirst(grid);
        while (undoChain.Count > 7)
        {
            undoChain.RemoveLast();
        }
        UndoCountChanged?.Invoke(UndoChain.Count - 1);
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
        UndoCountChanged?.Invoke(UndoChain.Count - 1);
    }

    bool UpdateHappened(Queue<(int Vertical, int Horizontal, int Value)> moveQueue, Queue<int?> scoreQueue)
    {
        bool reached2048 = false;
        while(moveQueue.Count > 0)
        {
            var updateArgs = moveQueue.Dequeue();
            GridUpdated?.Invoke(updateArgs.Vertical, updateArgs.Horizontal, updateArgs.Value);
            if (!triggered2048 && updateArgs.Value >= 2048)
            {
                reached2048 = true;
            }
            if (scoreQueue.TryDequeue(out int? nextScore) && nextScore != null)
            {
                ScoreUpdated?.Invoke((int)nextScore);
            }
            if (updateArgs.Value > 0)
            {
                Thread.Sleep(20);
            }
        }
        return reached2048;
    }
}
