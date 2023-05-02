namespace Game2048;

class GridInstance
{
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

    public event Action<int, int, int> GridUpdated;

    public event Action<int> ScoreUpdated;

    public event Action<int[,]> Reached2048;

    public GridInstance()
    {
        Grid = new int[4, 4];
    }

    public void AddPoints(int points)
    {
        Score += points;
        ScoreUpdated?.Invoke(Score);
    }

    public void UpdateField(int vertical, int horizontal, int value)
    {
        SetField(vertical, horizontal, value, true);
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

    protected void SetField(int vertical, int horizontal, int value, bool updating = false)
    {
        Grid[vertical, horizontal] = value;
        if (updating && value > 0)
        {
            Thread.Sleep(10);
        }
        GridUpdated?.Invoke(vertical, horizontal, value);
        if (value == 2048)
        {
            Reached2048?.Invoke(Grid);
        }
    }

    public void CheckIfCanMove()
    {
        int canMove = 4;
        foreach (MoveDirection direction in Enum.GetValues(typeof(MoveDirection)))
        {
            GridInstance copycat = CopyGrid();
            int[] arguments = ParseDirection(direction);
            try
            {
                SimulateMotion(copycat, arguments[0], arguments[1], arguments[2], arguments[3], false);
            }
            catch (CannotMoveException)
            {
                canMove -= 1;
            }
        }
        if (canMove <= 0)
        {
            throw new GridStuckException();
        }
    }

    int[] ParseDirection(MoveDirection? direction)
    {
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
        int axis = -1;
        switch (direction)
        {
            case MoveDirection.Left: case MoveDirection.Right:
                {
                    axis = 0;
                    break;
                }
            case MoveDirection.Down: case MoveDirection.Up:
                {
                    axis = 1;
                    break;
                }
        }
        return new int[] { start, until, delta, axis };
    }

    public GridInstance Move(MoveDirection? direction, Action<int, int, int> updateGridFunc, Action<int> updateScoreFunc, Action<int[,]> reach2048Func)
    {
        GridInstance copycat = CopyGrid();
        copycat.GridUpdated += updateGridFunc;
        copycat.Reached2048 += reach2048Func;
        copycat.ScoreUpdated += updateScoreFunc;
        int[] arguments = ParseDirection(direction);
        try
        {
            return SimulateMotion(copycat, arguments[0], arguments[1], arguments[2], arguments[3]);
        }
        catch (CannotMoveException)
        {
            throw;
        }
    }

    GridInstance SimulateMotion(GridInstance target, int start, int until, int delta, int axis, bool updating = true)
    {
        int canMove = 1;
        Queue<int[]> motionQueue = new Queue<int[]>();
        for (int i = 0; i < target.Grid.GetLength(axis); i++)
        {

            for (int j = start; j * delta <= until * delta; j += delta)
            {
                int k = j + delta;
                motionQueue = AdditionMotionLogic(motionQueue, target, until, delta, axis, i, ref j, k);
            }
        }
        if (motionQueue.Count == 0)
        {
            canMove -= 1;
        }
        else
        {
            while (motionQueue.Count > 0)
            {
                int[] motion = motionQueue.Dequeue();
                target.SetField(motion[0], motion[1], motion[2], updating);
            }
        }
        bool spaceForMoving = false;
        for (int i = 0; i < target.Grid.GetLength(axis); i++)
        {

            for (int j = start + delta; j * delta <= (until + delta) * delta; j += delta)
            {
                int k = j - delta;
                while (k * delta < j * delta &&
                        ZeroLogic(target, axis == 0 ? i : j, axis == 0 ? j : i,
                            axis == 0 ? i : k, axis == 0 ? k : i))
                {
                    k += delta;
                }
                if (k * delta < j * delta)
                {
                    spaceForMoving = true;
                    int numberToMove = target.Grid[axis == 0 ? i : j, axis == 0 ? j : i];
                    target.SetField(axis == 0 ? i : j, axis == 0 ? j : i, 0, updating);
                    target.SetField(axis == 0 ? i : k, axis == 0 ? k : i, numberToMove, updating);
                }
            }
        }
        if (!spaceForMoving)
        {
            canMove -= 1;
        }
        if (canMove < 0)
        {
            throw new CannotMoveException();
        }
        return target;
    }

    Queue<int[]> AdditionMotionLogic(Queue<int[]> motionQueue, GridInstance target,
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
