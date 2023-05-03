namespace Game2048;

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

    public Repository(Display display)
    {
        undoChain = new LinkedList<GridInstance>();
        GridInstance first = new GridInstance();
        first.GridUpdated += display.PrintTile;
        first.ScoreUpdated += display.PrintScore;
        first.Reached2048 += display.ScaleUp;
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

    public void Move(MoveDirection? input, Display display)
    {
        GridInstance next = UndoChain.First.Value.Move(input, display.PrintTile, display.PrintScore, display.ScaleUp);
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

    }
}
