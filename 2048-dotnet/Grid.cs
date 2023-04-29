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
        Queue<int[]> motionQueue = new Queue<int[]>();
        for (int i = 0; i < target.Grid.GetLength(axis); i++)
        {

            for (int j = start; j * delta <= until * delta; j += delta)
            {
                int k = j + delta;
                motionQueue = MotionLogic(motionQueue, target, until, delta, axis, i, ref j, k);
            }
        }
        throw new ArgumentOutOfRangeException();
    }

    Queue<int[]> MotionLogic(Queue<int[]> motionQueue, GridInstance target,
            int until, int delta, int axis,
            int outer, ref int current, int varying)
    {
        while (varying * delta < (until + delta) * delta)
        {
            switch (axis)
            {
                case 0:
                    {
                        if (AdditionLogic(target, outer, varying, outer, current))
                        {
                            motionQueue.Enqueue(new int[]{outer, varying, 0});
                            motionQueue.Enqueue(new int[]{outer, current, target.Grid[outer, current] * 2});
                            varying = (until + delta) * delta;
                            current += delta;
                        }
                        else if (!ZeroLogic(target, outer, varying, outer, current))
                        {
                            varying = (until + delta) * delta;
                        }
                        break;
                    }
                case 1:
                    {
                        if (AdditionLogic(target, varying, outer, current, outer))
                        {
                            motionQueue.Enqueue(new int[]{varying, outer, 0});
                            motionQueue.Enqueue(new int[]{current, outer, target.Grid[current, outer] * 2});
                            varying = (until + delta) * delta;
                            current += delta;
                        }
                        else if (!ZeroLogic(target, varying, outer, current, outer))
                        {
                            varying = (until + delta) * delta;
                        }
                        break;
                    }
            }
            varying += delta;
        }
        return motionQueue;
    }

    bool AdditionLogic(GridInstance target, int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return target.Grid[nextVerticalPosition, nextHorizontalPosition] ==
            target.Grid[currentVerticalPosition, currentHorizontalPosition];
    }

    bool ZeroLogic(GridInstance target, int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return target.Grid[nextVerticalPosition, nextHorizontalPosition] == 0 &&
            target.Grid[currentVerticalPosition, currentHorizontalPosition] != 0;
    }
}
