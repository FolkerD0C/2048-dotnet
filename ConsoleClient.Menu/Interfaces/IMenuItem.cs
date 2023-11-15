using ConsoleClient.Menu.Enums;

namespace ConsoleClient.Menu;
public interface IMenuItem
{
    string Name { get; }
    IMenuActionRequestedArgs ActionRequestedArgs { get; }
    MenuItemType ItemType { get; }
    MenuItemResult Select();
}