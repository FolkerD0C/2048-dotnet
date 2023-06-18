namespace Game2048.Static;

static class Display
{
    static bool cursorVisible;

    static readonly int offsetHorizontal;
    static readonly int offsetVertical;

    static readonly int width;
    public static int Width
    {
        get
        {
            return width;
        }
    }

    static readonly int height;
    public static int Height
    {
        get
        {
            return height;
        }
    }

    class DisplayPos
    {
        public char Value { get; set; }
        public ConsoleColor FgColor { get; set; }
        public ConsoleColor BgColor { get; set; }
    }

    static Stack<DisplayPos[,]> DisplayStack;
    static DisplayPos[,] CurrentDisplay;

    static Display()
    {
        Console.CursorVisible = false;
        cursorVisible = false;
        width = Console.WindowWidth < 70 ? Console.WindowWidth : 70;
        height = Console.WindowHeight < 40 ? Console.WindowHeight : 40;
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        DisplayStack = new Stack<DisplayPos[,]>();
        CurrentDisplay = NewEmptyLayout();
        DrawEntireDisplay();
    }

    static DisplayPos[,] NewEmptyLayout()
    {
        var result = new DisplayPos[height, width];
        for (int i = 0; i < result.GetLength(1); i++)
        {
            for (int j = 0; j < result.GetLength(0); j++)
            {
                result[j, i] = new DisplayPos()
                {
                    Value = ' ',
                    FgColor = ConsoleColor.White,
                    BgColor = ConsoleColor.Black
                };
            }
        }
        return result;
    }

    static void DrawEntireDisplay()
    {
        for (int i = 0; i < CurrentDisplay.GetLength(1); i++)
        {
            for (int j = 0; j < CurrentDisplay.GetLength(0); j++)
            {
                Print(i + offsetHorizontal, j + offsetVertical, CurrentDisplay[j, i]);
            }
        }
    }

    public static void NewLayout()
    {
        DisplayStack.Push(CurrentDisplay);
        CurrentDisplay = NewEmptyLayout();
        DrawEntireDisplay();
    }

    public static void PreviousLayout()
    {
        CurrentDisplay = DisplayStack.Pop();
        DrawEntireDisplay();
    }

    public static void PrintText(string text, int relativeHorizontalPosition, int relativeVerticalPosition,
            ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
    {
        for (int i = 0; i < text.Length; i++)
        {
            DisplayPos val = new DisplayPos()
                            {
                                Value = text[i],
                                FgColor = ForegroundColor,
                                BgColor = BackgroundColor
                            };
            Print(i + relativeHorizontalPosition + offsetHorizontal,
                    relativeVerticalPosition + offsetVertical,
                    val
                    );
            CurrentDisplay[relativeVerticalPosition, relativeHorizontalPosition + i] = val;
        }
    }

    static void Print(int absoluteHorizontalPosition, int absoluteVerticalPosition, DisplayPos toDraw)
    {
        Console.SetCursorPosition(absoluteHorizontalPosition, absoluteVerticalPosition);
        Console.ForegroundColor = toDraw.FgColor;
        Console.BackgroundColor = toDraw.BgColor;
        Console.Write(toDraw.Value);
    }

    public static void ToggleCursor()
    {
        cursorVisible = !cursorVisible;
        Console.CursorVisible = cursorVisible;
    }

    public static void SetCursorPos(int relativeHorizontalPosition, int relativeVerticalPosition)
    {
        Console.SetCursorPosition(offsetHorizontal + relativeHorizontalPosition, offsetVertical + relativeVerticalPosition);
    }
}
