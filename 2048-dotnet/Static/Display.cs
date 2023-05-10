namespace Game2048.Static;

static class Display
{
    static int offsetHorizontal;
    static int offsetVertical;

    static readonly int width = 70;
    static readonly int height = 25;

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
        DisplayStack = new Stack<DisplayPos[,]>();
        CurrentDisplay = NewEmptyOverlay();
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        RedrawDisplay();
    }

    static DisplayPos[,] NewEmptyOverlay()
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

    static void RedrawDisplay()
    {
        for (int i = 0; i < CurrentDisplay.GetLength(1); i++)
        {
            for (int j = 0; j < CurrentDisplay.GetLength(0); j++)
            {
                Print(i + offsetHorizontal, j + offsetVertical, CurrentDisplay[j, i]);
            }
        }
    }

    public static void NewOverlay()
    {
        DisplayStack.Push(CurrentDisplay);
        CurrentDisplay = NewEmptyOverlay();
        RedrawDisplay();
    }

    public static void PreviousOverlay()
    {
        CurrentDisplay = DisplayStack.Pop();
        RedrawDisplay();
    }

    public static void PrintText(string text, int relativeHorizontalPosition, int relativeVerticalPosition,
            ConsoleColor ForegColor, ConsoleColor BackgColor)
    {
        for (int i = 0; i < text.Length; i++)
        {
            DisplayPos val = new DisplayPos()
                            {
                                Value = text[i],
                                FgColor = ForegColor,
                                BgColor = BackgColor
                            };
            Print(i + relativeHorizontalPosition + offsetHorizontal,
                    relativeVerticalPosition + offsetVertical,
                    val
                    );
            CurrentDisplay[relativeVerticalPosition + i, relativeVerticalPosition] = val;
        }
    }

    static void Print(int absoluteHorizontalPosition, int absoluteVerticalPosition, DisplayPos toDraw)
    {
        Console.SetCursorPosition(absoluteHorizontalPosition, absoluteVerticalPosition);
        Console.ForegroundColor = toDraw.FgColor;
        Console.BackgroundColor = toDraw.BgColor;
        Console.Write(toDraw.Value);
    }
}
