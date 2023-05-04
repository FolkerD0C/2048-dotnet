namespace Game2048.Classes;

class GridInstance
{
    int [,] grid;
    public int[,] Grid
    {
        get
        {
            return grid;
        }
        protected set
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
        protected set
        {
            if (value == score)
            {
                ScoreQueue.Enqueue(null);
            }
            else
            {
                ScoreQueue.Enqueue(value);
            }
            score = value;
        }
    }

    Queue<(int Vertical, int Horizontal, int Value)> MoveQueue;

    Queue<int?> ScoreQueue;

    public event Action Reached2048;

    public event Action<Queue<(int Vertical, int Horizontal, int Value)>, Queue<int?>> UpdateHappened;

    public GridInstance()
    {
        grid = new int[4, 4];
        MoveQueue = new Queue<(int Vertical, int Horizontal, int Value)>();
        ScoreQueue = new Queue<int?>();
    }

    public GridInstance(int score) : this()
    {
        this.score = score;
    }

    GridInstance CopyGrid()
    {
        GridInstance copycat = new GridInstance(Score);
        for (int i = 0; i < copycat.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < copycat.Grid.GetLength(1); j++)
            {
                copycat.Grid[i, j] = this.Grid[i, j];
            }
        }
        return copycat;
    }

    public void UpdateField(int vertical, int horizontal, int value)
    {
        Grid[vertical, horizontal] = value;
    }

    void SetField(int vertical, int horizontal, int value)
    {
        Grid[vertical, horizontal] = value;
        MoveQueue.Enqueue((vertical, horizontal, value));
        if (value == 2048)
        {
            Reached2048?.Invoke();
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
                SimulateMotion(arguments[0], arguments[1], arguments[2], arguments[3], false);
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

    public GridInstance Move(MoveDirection? direction,
            Action<Queue<(int Vertical, int Horizontal, int Value)>, Queue<int?>> updateInstance,
            Action? reach2048Func)
    {
        GridInstance copycat = CopyGrid();
        if (reach2048Func != null)
        {
            copycat.Reached2048 += reach2048Func;
        }
        copycat.UpdateHappened += updateInstance;
        int[] arguments = ParseDirection(direction);
        return copycat.SimulateMotion(arguments[0], arguments[1], arguments[2], arguments[3]);
    }

    protected GridInstance SimulateMotion(int start, int until, int delta, int axis, bool updating = true)
    {
        for (int i = 0; i < Grid.GetLength(axis); i++)
        {
            for (int j = start; j * delta <= until * delta; j += delta)
            {
                int k = j + delta;
                AdditionMotionLogic(until, delta, axis, i, ref j, k);
            }
        }
        for (int i = 0; i < Grid.GetLength(axis); i++)
        {

            for (int j = start + delta; j * delta <= (until + delta) * delta; j += delta)
            {
                int k = j - delta;
                while (k * delta < j * delta &&
                        ZeroLogic(axis == 0 ? i : j, axis == 0 ? j : i,
                            axis == 0 ? i : k, axis == 0 ? k : i))
                {
                    k += delta;
                }
                if (k * delta < j * delta)
                {
                    int numberToMove = Grid[axis == 0 ? i : j, axis == 0 ? j : i];
                    SetField(axis == 0 ? i : j, axis == 0 ? j : i, 0);
                    SetField(axis == 0 ? i : k, axis == 0 ? k : i, numberToMove);
                }
            }
        }
        if (MoveQueue.Count == 0)
        {
            throw new CannotMoveException();
        }
        if (updating)
        {
            UpdateHappened?.Invoke(MoveQueue, ScoreQueue);
            MoveQueue = new Queue<(int Vertical, int Horizontal, int Value)>();
            ScoreQueue = new Queue<int?>();
        }
        return this;
    }

    void AdditionMotionLogic(int until, int delta, int axis,
            int outer, ref int current, int varying)
    {
        while (varying * delta < (until + delta) * delta)
        {
            switch (axis)
            {
                case 0:
                    {
                        if (AdditionLogic(outer, varying, outer, current))
                        {
                            int numberToMove = Grid[outer, current];
                            SetField(outer, varying, 0);
                            Score = Score;
                            SetField(outer, current, numberToMove * 2);
                            Score += numberToMove * 2;
                            varying = (until + delta) * delta;
                            current += delta;
                        }
                        else if (!ZeroLogic(outer, varying, outer, current))
                        {
                            varying = (until + delta) * delta;
                        }
                        break;
                    }
                case 1:
                    {
                        if (AdditionLogic(varying, outer, current, outer))
                        {
                            int numberToMove = Grid[current, outer];
                            SetField(varying, outer, 0);
                            Score = Score;
                            SetField(current, outer, numberToMove * 2);
                            Score += numberToMove * 2;
                            varying = (until + delta) * delta;
                            current += delta;
                        }
                        else if (!ZeroLogic(varying, outer, current, outer))
                        {
                            varying = (until + delta) * delta;
                        }
                        break;
                    }
            }
            varying += delta;
        }
    }

    bool AdditionLogic(int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return Grid[currentVerticalPosition, currentHorizontalPosition] != 0 &&
            Grid[nextVerticalPosition, nextHorizontalPosition] ==
            Grid[currentVerticalPosition, currentHorizontalPosition];
    }

    bool ZeroLogic(int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return Grid[nextVerticalPosition, nextHorizontalPosition] == 0 &&
            Grid[currentVerticalPosition, currentHorizontalPosition] != 0;
    }
}
