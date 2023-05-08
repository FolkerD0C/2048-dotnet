using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class PromptMenu : NavigationMenu
{
    string[] promptMessage;

    Action action;

    public PromptMenu(string displayName, string[] promptMessage, Action action) :
        base(displayName, new List<IMenu>() { new ReturnMenu(MenuResult.Yes), new ReturnMenu(MenuResult.No) })
    {
        this.promptMessage = promptMessage;
        this.action = action;
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

    public override MenuResult MenuAction()
    {
        DrawDisplayMessage();
        Navigate();
        ClearDisplayMessage();
        var result = SubMenus[CursorPosition].MenuAction();
        if (result == MenuResult.Yes)
        {
            action?.Invoke();
        }
        return result;
    }
}
