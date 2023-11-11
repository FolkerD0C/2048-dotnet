using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes.Menus;

public class PromptObjectMenu : NavigationMenu
{
    string[] promptMessage;

    Action<object[]> action;

    object[] args;

    public PromptObjectMenu(string displayName, IEnumerable<string> promptMessage, Action<object[]> action, params object[] args) :
        base(displayName, new List<IMenu>() { new ReturnMenu(MenuResult.Yes), new ReturnMenu(MenuResult.No) })
    {
        this.promptMessage = promptMessage.ToArray();
        this.action = action;
        this.args = args;
        MenuPosition = promptMessage.Count();
    }

    void DrawDisplayMessage()
    {
        for (int i = 0; i < promptMessage.Length; i++)
        {
            Display.PrintText(promptMessage[i], 0, i, ConsoleColor.White, ConsoleColor.Black);
        }
    }

    public override MenuResult MenuAction()
    {
        Display.NewLayout();
        DrawDisplayMessage();
        Navigate();
        var result = SubMenus[CursorPosition].MenuAction();
        if (result == MenuResult.Yes)
        {
            action?.Invoke(args);
        }
        Display.PreviousLayout();
        return result;
    }
}
