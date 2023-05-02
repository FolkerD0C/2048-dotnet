namespace Game2048;

class Play
{
    LinkedList<GridInstance> undoChain;

    Display display;

    public Play()
    {
        undoChain = new LinkedList<GridInstance>();
        Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet = new Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)>();
        ConsoleColor[] fgColors = new ConsoleColor[]
        {
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black,
            ConsoleColor.White, ConsoleColor.White, ConsoleColor.White,
            ConsoleColor.White, ConsoleColor.White
        };
        ConsoleColor[] bgColors = new ConsoleColor[]
        {
            ConsoleColor.Gray, ConsoleColor.Gray, ConsoleColor.Gray,
            ConsoleColor.White, ConsoleColor.White, ConsoleColor.White,
            ConsoleColor.DarkGray, ConsoleColor.DarkGray, ConsoleColor.DarkGray,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Red, ConsoleColor.Red
        };
        colorSet.Add(0, (ConsoleColor.White, ConsoleColor.Black));
        for (int i = 0; i < fgColors.Length; i++)
        {
            colorSet.Add((int)Math.Pow(2, i + 1), (fgColors[i], bgColors[i]));
        }
        display = new Display(colorSet);
        display.InitializeDisplay();
        GridInstance first = new GridInstance();
        first.GridUpdated += display.PrintTile;
        first.ScoreUpdated += display.PrintScore;
        first.Reached2048 += display.ScaleUp;
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

    void HandleInput()
    {
        MoveDirection? input = null;
        switch(Console.ReadKey().Key)
        {
            case ConsoleKey.W: case ConsoleKey.UpArrow:
                {
                    input = MoveDirection.Up;
                    break;
                }
            case ConsoleKey.S: case ConsoleKey.DownArrow:
                {
                    input = MoveDirection.Down;
                    break;
                }
            case ConsoleKey.A: case ConsoleKey.LeftArrow:
                {
                    input = MoveDirection.Left;
                    break;
                }
            case ConsoleKey.D: case ConsoleKey.RightArrow:
                {
                    input = MoveDirection.Right;
                    break;
                }
            case ConsoleKey.Backspace:
                {
                    Undo();
                    break;
                }
            case ConsoleKey.Escape:
                {
                    break;
                }
        }
        if (input == null)
        {
            return;
        }
        try
        {
            GridInstance next = undoChain.First.Value.Move(input, display.PrintTile, display.PrintScore, display.ScaleUp);
            UpdateUndoChain(next);
        }
        catch (CannotMoveException)
        {

        }
    }

    void UpdateUndoChain(GridInstance grid)
    {
        undoChain.AddFirst(grid);
        while (undoChain.Count > 7)
        {
            undoChain.RemoveLast();
        }
    }

    void GameOver()
    {

    }

    void GameWon()
    {

    }

    public void Run()
    {

    }

    void Undo()
    {

    }

    void Save()
    {

    }
}
