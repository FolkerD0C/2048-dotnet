namespace Game2048;

class GridInstance
{
    public enum MoveDirection
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

    public void UpdateField(int up, int left, int value)
    {
        SetField(up, left, value);
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

    public GridInstance Move(MoveDirection direction)
    {
        GridInstance copycat = CopyGrid(); 
        int start = 0;
        int until = 0;
        int delta = 0;
        switch (direction)
        {
            case MoveDirection.Left: case MoveDirection.Up:
                {
                    start = 0;
                    until = 2;
                    delta = 1;
                    break;
                }
            case MoveDirection.Right: case MoveDirection.Down:
                {
                    start = 3;
                    until = 1;
                    delta = -1;
                    break;
                }
        }
        switch (direction)
        {
            case MoveDirection.Left: case MoveDirection.Right:
                return SimulateMotion(copycat, start, until, delta, 0);
            case MoveDirection.Down: case MoveDirection.Up:
                return SimulateMotion(copycat, start, until, delta, 1);
        }
    }

    GridInstance SimulateMotion(GridInstance target, int start, int until, int delta, int axis)
    {
        for (int i = 0; i < target.Grid.GetLength(axis); i++)
        {
            for (int j = start; j <= until; j += delta)
            {
                
            }
        }
        return target;
    }
}
