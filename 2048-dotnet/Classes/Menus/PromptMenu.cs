using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes.Menus;

public class PromptMenu : NavigationMenu
{
    string[] promptMessage;

    Action action;

    public PromptMenu(string displayName, IEnumerable<string> promptMessage, Action action) :
        base(displayName, new List<IMenu>() { new ReturnMenu(MenuResult.Yes), new ReturnMenu(MenuResult.No) })
    {
        this.promptMessage = promptMessage.ToArray();
        this.action = action;
        MenuPosition = promptMessage.Count();
    }

    void DrawDisplayMessage()
    {
        for (int i = 0; i < promptMessage.Length; i++)
        {
            Display.PrintText(promptMessage[i], 0, i, ConsoleColor.White, ConsoleColor.Black);
        }
    }

    void ClearDisplayMessage()
    {
        for (int i = 0; i < promptMessage.Length; i++)
        {
            Display.PrintText(new string(' ', Display.Width), 0, i, ConsoleColor.White, ConsoleColor.Black);
        }
    }

    public override MenuResult MenuAction()
    {
        Display.NewLayout();
        DrawDisplayMessage();
        Navigate();
        ClearDisplayMessage();
        var result = SubMenus[CursorPosition].MenuAction();
        if (result == MenuResult.Yes)
        {
            action?.Invoke();
        }
        Display.PreviousLayout();
        return result;
    }
}
