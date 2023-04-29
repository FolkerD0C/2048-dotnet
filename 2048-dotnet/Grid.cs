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
        throw new ArgumentOutOfRangeException();
    }

    GridInstance SimulateMotion(GridInstance target, int start, int until, int delta, int axis)
    {
        for (int i = 0; i < target.Grid.GetLength(axis); i++)
        {
            for (int j = start; j <= until; j += delta)
            {
                switch (axis)
                {
                    case 0:
                        return MotionLogic(target, i, j + delta, i, j);
                    case 1:
                        return MotionLogic(target, j + delta, i, j, i);
                }
            }
        }
        throw new ArgumentOutOfRangeException();
    }

    GridInstance MotionLogic(GridInstance target, int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        if (target.Grid[nextVerticalPosition, nextHorizontalPosition] == 0 &&
                target.Grid[currentVerticalPosition, currentHorizontalPosition] != 0)
        {
            int numberToMove = target.Grid[currentVerticalPosition, currentHorizontalPosition];
            target.SetField(currentVerticalPosition, currentHorizontalPosition, 0);
            target.SetField(nextVerticalPosition, nextHorizontalPosition, numberToMove);
        }
        else if (target.Grid[nextVerticalPosition, nextHorizontalPosition] ==
                target.Grid[currentVerticalPosition, currentHorizontalPosition])
        {
            target.SetField(currentVerticalPosition, currentHorizontalPosition, 0);
            target.SetField(nextVerticalPosition, nextHorizontalPosition,
                    target.Grid[nextVerticalPosition, nextHorizontalPosition] * 2);
        }
        return target;
    }
}
