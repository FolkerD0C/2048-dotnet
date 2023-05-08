using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class DisplayMenu : NavigationMenu
{
    string[] displayMessage;

    public DisplayMenu(string displayName, string[] displayMessage) :
        base(displayName, new List<IMenu>() { new ReturnMenu(MenuResult.Back) })
        {
            this.displayMessage = displayMessage;
            MenuPosition = displayMessage.Length;
        }

    public override MenuResult MenuAction()
    {
        return Navigate();
    }

    void DrawDisplayMessage()
    {
        Console.SetCursorPosition(0, 0);
        foreach(string line in displayMessage)
        {
            Console.WriteLine(line);
        }
        Console.SetCursorPosition(0, MenuPosition);
    }

    void ClearDisplayMessage()
    {
        Console.SetCursorPosition(0, 0);
        foreach(string line in displayMessage)
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, MenuPosition);
    }

    protected override MenuResult Navigate()
    {
        DrawDisplayMessage();
        DrawMenu(0);
        HandleKeyboardInput();
        ClearDisplayMessage();
        ClearDisplay();
        return MenuResult.OK;
    }

    protected override InputAction HandleKeyboardInput()
    {
        while (true)
        {
            ConsoleKey input = Console.ReadKey(true).Key;
            switch (input)
            {
                case ConsoleKey.Enter: case ConsoleKey.Spacebar: case ConsoleKey.Escape:
                    return InputAction.Activate;
                default:
                    break;
            }
        }
        throw new ArgumentException();
    }
}

