using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class PromptMenu : NavigationMenu
{
    string[] promptMessage;

    public PromptMenu(string displayName, string[] promptMessage, params ReturnMenu[] returnValues) :
        base(displayName, returnValues.Select(r => (IMenu)r).Distinct().ToList())
    {
        this.promptMessage = promptMessage;
        MenuPosition = promptMessage.Length;
    }

    void DrawDisplayMessage()
    {
        Console.SetCursorPosition(0, 0);
        foreach(string line in promptMessage)
        {
            Console.WriteLine(line);
        }
        Console.SetCursorPosition(0, MenuPosition);
    }

    void ClearDisplayMessage()
    {
        Console.SetCursorPosition(0, 0);
        foreach(string line in promptMessage)
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, MenuPosition);
    }

    public MenuResult MenuAction()
    {
        return Navigate();
    }

    protected override MenuResult Navigate()
    {
        MenuResult navigation = MenuResult.OK;
        int cursorPosition = 0;
        Func<int, int> moveCursor = x =>
            x < 0 ? SubMenus.Count - 1 :
            x >= SubMenus.Count ? 0 : x;
        DrawDisplayMessage();
        while (navigation == MenuResult.OK)
        {
            DrawMenu(cursorPosition);
            switch(HandleKeyboardInput())
            {
                case InputAction.Up:
                    {
                        cursorPosition = moveCursor(cursorPosition - 1);
                        break;
                    }
                case InputAction.Down:
                    {
                        cursorPosition = moveCursor(cursorPosition + 1);
                        break;
                    }
                case InputAction.Activate:
                    {
                        navigation = SubMenus[cursorPosition].MenuAction();
                        break;
                    }
            }
        }
        ClearDisplay();
        ClearDisplayMessage();
        return navigation;
    }
}
