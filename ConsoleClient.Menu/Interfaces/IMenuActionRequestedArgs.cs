using ConsoleClient.Menu.Enums;
using System;

namespace ConsoleClient.Menu;

public interface IMenuActionRequestedArgs
{
    MenuActionType ActionType { get; }
    IConsoleMenu SubMenu { get; }
    Action Action { get; }
    Action<string> ActionWithStringArg { get; }
    string ActionStringArg { get; }
}