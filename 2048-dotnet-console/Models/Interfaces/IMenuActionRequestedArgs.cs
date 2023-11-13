using Game2048.ConsoleFrontend.Helpers.Enums;
using Game2048.ConsoleFrontend.Resources.Menus;
using System;

namespace Game2048.ConsoleFrontend.Models;

public interface IMenuActionRequestedArgs
{
    MenuActionType ActionType { get; }
    IMenu SubMenu { get; }
    Action Action { get; }
    Action<string> ActionWithStringArg { get; }
    string ActionStringArg { get; }
}