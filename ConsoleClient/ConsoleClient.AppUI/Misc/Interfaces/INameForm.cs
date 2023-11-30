using ConsoleClient.Display.Models;

namespace ConsoleClient.AppUI.Misc;

/// <summary>
/// Defines a method for prompting the player for a new name.
/// </summary>
public interface INameForm : IOverLay
{
    /// <summary>
    /// Prompts the player for a new name to set.
    /// </summary>
    /// <param name="name">The previous name of the player.</param>
    /// <returns>The result of this action as a <see cref="NameFormResult"/>.</returns>
    NameFormResult PromptPlayerName(string name);
}
