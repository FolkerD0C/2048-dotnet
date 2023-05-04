namespace Game2048.Classes;

class Display
{
    readonly string[] borderUntil2048 = new string[]
    {
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+"
    };

    readonly string[] borderAfter2048 = new string[]
    {
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+"
    };

    readonly string help1 = "Use arrow keys or WASD to move";
    readonly string help2 = "Use BACKSPACE to undo";

    readonly string points = "Points:";

    int maxSpaceForTiles = 4;

    int maxSpaceForScore = 9;

    readonly int maxErrorHight = 4;
    readonly int maxErrorWidth = 30;

    bool errorDisplayed;

    //Constants for display positions
    readonly (int Vertical, int Horizontal)[] gridPosition = { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0) };
    readonly (int Vertical, int Horizontal)[] errorPosition = { (5, 5), (6, 5), (7, 5), (8, 5) };
    readonly (int Vertical, int Horizontal) help1Position = (3, 35);
    readonly (int Vertical, int Horizontal) help2Position = (5, 35);
    readonly (int Vertical, int Horizontal) pointsPosition = (10, 10);
    readonly (int Vertical, int Horizontal) scorePosition = (10, 18);

    Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet;

    public Display(Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet)
    {
        this.colorSet = colorSet;
    }

    string NumberToDisplayWidth(int number, int maxSpace)
    {
        if (number == 0)
        {
            return new string(' ', maxSpace);
        }
        int actualWidth = $"{number}".Length;
        string result = new string(' ', maxSpace - actualWidth) + $"{number}";
        return result;
    }

    public void InitializeDisplay()
    {
        Console.Clear();
        Console.CursorVisible = false;
        for (int i = 0; i < gridPosition.Length; i++)
        {
            Print(gridPosition[i], borderUntil2048[i]);
        }
        Print(help1Position, help1);
        Print(help2Position, help2);
        Print(pointsPosition, points);
    }

    public void ScaleUp(int[,] grid)
    {
        maxSpaceForTiles = 6;
        for (int i = 0; i < gridPosition.Length; i++)
        {
            Print(gridPosition[i], borderUntil2048[i]);
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                PrintTile(i, j, grid[i, j]);
            }
        }
    }

    public void RedrawGridInstance(int[,] grid, int score)
    {
        if (errorDisplayed)
        {
            ClearErrorMessage();
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                PrintTile(i, j, grid[i, j]);
            }
        }
        PrintScore(score);
    }

    void Print((int Vertical, int Horizontal) position, string displayText,
            ConsoleColor fgColor = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {
        Console.SetCursorPosition(position.Horizontal, position.Vertical);
        Console.ForegroundColor = fgColor;
        Console.BackgroundColor = bgColor;
        Console.Write(displayText);
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
    }

    public void PrintTile(int vertical, int horizontal, int value)
    {
        if (errorDisplayed)
        {
            ClearErrorMessage();
        }
        var position = ParsePosition(vertical, horizontal);
        var displayText = NumberToDisplayWidth(value, maxSpaceForTiles);
        Print(position, displayText, colorSet[value].Fg, colorSet[value].Bg);
    }

    public void PrintScore(int score)
    {
        if (errorDisplayed)
        {
            ClearErrorMessage();
        }
        string displayText = NumberToDisplayWidth(score, maxSpaceForScore);
        Print(scorePosition, displayText, ConsoleColor.White, ConsoleColor.Red);
    }

    (int Vertical, int Horizontal) ParsePosition(int vertical, int horizontal)
    {
        int ver = 1 + vertical * (maxSpaceForTiles + 1);
        int hor = 1 + horizontal * 2;
        return (ver, hor);
    }

    public void DrawErrorMessage(string error)
    {
        string[] parsedMessage = ParseErrorMessage(error);
        for (int i = 0; i < parsedMessage.Length; i++)
        {
            Print(errorPosition[i], parsedMessage[i], ConsoleColor.White, ConsoleColor.Red);
        }
        errorDisplayed = true;
    }

    void ClearErrorMessage()
    {
        for (int i = 0; i < maxErrorHight; i++)
        {
            Print(errorPosition[i], new string(' ', maxErrorWidth));
        }
    }

    string[] ParseErrorMessage(string error)
    {
        var splittedMessage = new Queue<string>(error.Split(' '));
        var result = new string[maxErrorHight];
        for (int i = 0; i < maxErrorHight; i++)
        {
            string currentBuffer = splittedMessage.Count > 0 ? splittedMessage.Dequeue() : "";
            int currentCount = currentBuffer.Length;
            while (splittedMessage.Count > 0 && currentCount + splittedMessage.Peek().Length + 1 <= maxErrorWidth)
            {
                currentCount += splittedMessage.Peek().Length + 1;
                currentBuffer += " " + splittedMessage.Dequeue();
            }
            currentBuffer += new string(' ', maxErrorWidth - currentBuffer.Length);
            result[i] = currentBuffer;
        }
        return result;
    }
}
