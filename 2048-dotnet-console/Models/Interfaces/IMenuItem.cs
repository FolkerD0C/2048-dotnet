using Game2048.ConsoleFrontend.Helpers.Enums;

namespace Game2048.ConsoleFrontend.Models;

public interface IMenuItem
{
    IMenuActionRequestedArgs ActionRequestedArgs { get; }
    MenuItemType ItemType { get; }
    MenuItemResult Select();
}