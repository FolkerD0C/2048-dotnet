namespace Game2048;

class GridInstance
{
    enum MoveDirection
    {
        Left,
        Down,
        Up,
        Right
    }

    int [,] grid;
    public int[,] Grid
    {
        get
        {
            return grid;
        }
        private set
        {
            grid = value;
        }
    }

    int score;
    public int Score
    {
        get
        {
            return score;
        }
        private set
        {
            score = value;
        }
    }

    public GridInstance()
    {
        Grid = new int[4, 4];
    }

    public void AddPoints(int points)
    {
        Score += points;
    }

    public GridInstance UpdateField(int up, int left, int value)
    {
        GridInstance copy = CopyGrid();
        copy.SetField(up, left, value);
        return copy;
    }

    GridInstance CopyGrid()
    {
        GridInstance copycat = new GridInstance();
        for (int i = 0; i < copycat.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < copycat.Grid.GetLength(1); j++)
            {
                copycat.SetField(i, j, Grid[i, j]);
            }
        }
        copycat.AddPoints(Score);
        return copycat;
    }

    protected void SetField(int up, int left, int value)
    {
        Grid[up, left] = value;
    }
}
