using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes.Menus;

public class DisplayMenu : NavigationMenu
{
    string[] displayMessage;

    public DisplayMenu(string displayName, IEnumerable<string> displayMessage) :
        base(displayName, new List<IMenu>() { new ReturnMenu(MenuResult.Back) })
        {
            this.displayMessage = displayMessage.ToArray();
            MenuPosition = displayMessage.Count();
        }

    public override MenuResult MenuAction()
    {
        Display.NewLayout();
        DrawDisplayMessage();
        Navigate();
        ClearDisplayMessage();
        Display.PreviousLayout();
        return MenuResult.OK;
    }

    void DrawDisplayMessage()
    {
        for (int i = 0; i < displayMessage.Length; i++)
        {
            Display.PrintText(displayMessage[i], 0, i, ConsoleColor.White, ConsoleColor.Black);
        }
    }

    void ClearDisplayMessage()
    {
        for (int i = 0; i < displayMessage.Length; i++)
        {
            Display.PrintText(new string(' ', Display.Width), 0, i, ConsoleColor.White, ConsoleColor.Black);
        }
    }
}

