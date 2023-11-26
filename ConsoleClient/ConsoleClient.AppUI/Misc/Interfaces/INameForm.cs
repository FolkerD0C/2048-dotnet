using ConsoleClient.Display;

namespace ConsoleClient.AppUI.Misc;

public interface INameForm : IOverLay
{
    NameFormResult PromptPlayerName(string name);
}
