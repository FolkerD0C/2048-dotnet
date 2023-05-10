using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes;

class GameDisplay : IGameDisplay
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
    readonly (int Vertical, int Horizontal)[] errorPositions = { (12, 5), (13, 5), (14, 5), (15, 5), (16, 5) };
    readonly (int Vertical, int Horizontal)[] helpPositions = { (1, 35), (3, 35) };
    readonly (int Vertical, int Horizontal) pointsPosition = (10, 10);
    readonly (int Vertical, int Horizontal) scorePosition = (10, 18);
    readonly (int Vertical, int Horizontal) undosPosition = (5, 35);
    readonly (int Vertical, int Horizontal) undosCountPosition = (5, 51);
    readonly (int Vertical, int Horizontal) livesPosition = (7, 35);
    readonly (int Vertical, int Horizontal) livesCountPosition = (7, 52);

    Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet;

    public GameDisplay(Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet)
    {
        this.colorSet = colorSet;
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

    public void ScaleUp(object? o, int[,] grid)
    {
        maxSpaceForTiles = 6;
        for (int i = 0; i < gridPositions.Length; i++)
        {
            Print(gridPositions[i], borderAfter2048[i]);
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                PrintTile(null, (i, j, grid[i, j]));
            }
        }
    }

    public void RedrawGridInstance(object? o, (int[,] grid, int score) args)
    {
        for (int i = 0; i < args.grid.GetLength(0); i++)
        {
            for (int j = 0; j < args.grid.GetLength(1); j++)
            {
                PrintTile(null, (i, j, args.grid[i, j]));
            }
        }
        PrintScore(null, args.score);
    }

    void Print((int Vertical, int Horizontal) position, string displayText,
            ConsoleColor fgColor = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {
        Display.PrintText(displayText, position.Horizontal, position.Vertical,
                fgColor, bgColor);
    }

    public void PrintTile(object? o, (int vertical, int horizontal, int value) args)
    {
        var position = ParsePosition(args.vertical, args.horizontal);
        var displayText = NumberToDisplayWidth(args.value, maxSpaceForTiles);
        Print(position, displayText, colorSet[args.value].Fg, colorSet[args.value].Bg);
    }

    public void PrintScore(object? o, int score)
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

    public void PrintUndosCount(object? o, int undosCount)
    {
        Print(undosCountPosition, NumberToDisplayWidth(undosCount, 1), ConsoleColor.Magenta, ConsoleColor.Yellow);
    }

    public void PrintLivesCount(object? o, int livesCount)
    {
        Print(livesCountPosition, NumberToDisplayWidth(livesCount, 1), ConsoleColor.Magenta, ConsoleColor.Yellow);
    }
}
