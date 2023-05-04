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

    readonly string[] helpMessages =
    {
        "Use arrow keys or WASD to move",
        "Use BACKSPACE to undo"
    };

    readonly string points = "Points:";
    readonly string undos = "Possible undos:";
    readonly string lives = "Remaining lives:";

    int maxSpaceForTiles = 4;

    int maxSpaceForScore = 9;

    readonly int maxErrorHight = 5;
    readonly int maxErrorWidth = 30;

    bool errorDisplayed;

    //Constants for display positions
    readonly (int Vertical, int Horizontal)[] gridPositions = { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0) };
    readonly (int Vertical, int Horizontal)[] errorPositions = { (12, 5), (13, 5), (14, 5), (15, 5) };
    readonly (int Vertical, int Horizontal)[] helpPositions = { (1, 35), (3, 35) };
    readonly (int Vertical, int Horizontal) pointsPosition = (10, 10);
    readonly (int Vertical, int Horizontal) scorePosition = (10, 18);
    readonly (int Vertical, int Horizontal) undosPosition = (5, 35);
    readonly (int Vertical, int Horizontal) undosCountPosition = (5, 51);
    readonly (int Vertical, int Horizontal) livesPosition = (7, 35);
    readonly (int Vertical, int Horizontal) livesCountPosition = (7, 52);

    Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet;

    public Display(Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet)
    {
        this.colorSet = colorSet;
        Console.Clear();
        Console.CursorVisible = false;
        for (int i = 0; i < gridPositions.Length; i++)
        {
            Print(gridPositions[i], borderUntil2048[i]);
        }
        for (int i = 0; i < helpPositions.Length; i++)
        {
            Print(helpPositions[i], helpMessages[i]);
        }
        Print(pointsPosition, points);
        Print(undosPosition, undos);
        Print(livesPosition, lives);
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

    public void ScaleUp(int[,] grid)
    {
        maxSpaceForTiles = 6;
        for (int i = 0; i < gridPositions.Length; i++)
        {
            Print(gridPositions[i], borderUntil2048[i]);
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
        int ver = 1 + vertical * 2;
        int hor = 1 + horizontal * (maxSpaceForTiles + 1);
        return (ver, hor);
    }

    public void PrintErrorMessage(string errorMessage)
    {
        string[] parsedMessage = ParseErrorMessage(errorMessage);
        for (int i = 0; i < parsedMessage.Length; i++)
        {
            Print(errorPositions[i], parsedMessage[i], ConsoleColor.White, ConsoleColor.Red);
        }
        errorDisplayed = true;
    }

    void ClearErrorMessage()
    {
        for (int i = 0; i < maxErrorHight; i++)
        {
            Print(errorPositions[i], new string(' ', maxErrorWidth));
        }
        errorDisplayed = false;
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

    public void PrintUndosCount(int undosCount)
    {
        Print(undosCountPosition, NumberToDisplayWidth(undosCount, 1), ConsoleColor.Magenta, ConsoleColor.Yellow);
    }

    public void PrintLivesCount(int livesCount)
    {
        Print(livesCountPosition, NumberToDisplayWidth(livesCount, 1), ConsoleColor.Magenta, ConsoleColor.Yellow);
    }
}
