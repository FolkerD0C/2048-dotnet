using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes.Menus;

public class NotifiableMenu : NavigationMenu
{
    Dictionary<IMenu, Dictionary<MenuResult, string[]>> notifications;

    public NotifiableMenu(string displayName, List<IMenu> subMenus, Dictionary<IMenu, Dictionary<MenuResult, string[]>> notifications) : base(displayName, subMenus)
    {
        this.notifications = notifications;
    }

    public override MenuResult MenuAction()
    {
        Display.NewLayout();
        Navigate();
        Display.PreviousLayout();
        return Result;
    }

    protected override void Navigate()
    {
        MenuResult navigation = MenuResult.OK;
        Func<int, int> moveCursor = x =>
            x < 0 ? SubMenus.Count - 1 :
            x >= SubMenus.Count ? 0 : x;
        int prevPos = 0;
        DrawMenu();
        while (AcceptedResults.Contains(navigation))
        {
            SelectionChanged(prevPos);
            switch(HandleKeyboardInput())
            {
                case InputAction.Up:
                    {
                        prevPos = CursorPosition;
                        CursorPosition = moveCursor(CursorPosition - 1);
                        break;
                    }
                case InputAction.Down:
                    {
                        prevPos = CursorPosition;
                        CursorPosition = moveCursor(CursorPosition + 1);
                        break;
                    }
                case InputAction.Activate:
                    {
                        navigation = SubMenus[CursorPosition].MenuAction();
                        if (notifications.ContainsKey(SubMenus[CursorPosition]) && notifications[SubMenus[CursorPosition]].ContainsKey(navigation))
                        {
                            DisplayNotification(notifications[SubMenus[CursorPosition]][navigation]);
                        }
                        break;
                    }
            }
        }
    }

    void DisplayNotification(string[] notification)
    {
        int notificationOffsetPosition = MenuPosition + SubMenus.Count + 2;
        Display.NewOverLay();
        for (int i = 0; i < notification.Length; i++)
        {
            Display.PrintText(notification[i], 0, notificationOffsetPosition + i, ConsoleColor.Black, ConsoleColor.White);
        }
        Thread.Sleep(2000);
        Display.PreviousLayout();
    }
}
