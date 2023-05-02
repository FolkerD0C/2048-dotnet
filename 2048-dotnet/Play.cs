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
        undoChain.First.Value.UpdateField(position.Vertical, position.Horizontal, twoOrFour);
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

    bool HandleInput()
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
            return false;
        }
        try
        {
            GridInstance next = undoChain.First.Value.Move(input, display.PrintTile, display.PrintScore, display.ScaleUp);
            UpdateUndoChain(next);
        }
        catch (CannotMoveException)
        {
            return false;
        }
        return true;
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
        while (true)
        {
            if (HandleInput())
            {
                PutTwoOrFour();
            }
        }
    }

    void Undo()
    {

    }

    void Save()
    {

    }
}
