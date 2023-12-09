using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.Menu.Enums;
using Game2048.Managers.Enums;
using System;
using System.Collections.Generic;

namespace ConsoleClient.App.Resources;

/// <summary>
/// A static class that has methods for parsing keyboard input into input enum values for different components.
/// </summary>
internal static class InputProvider
{
    /// <summary>
    /// Provides input for the game.
    /// </summary>
    /// <returns>Input for the game.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static GameInput ProvidePlayInput()
    {
        GameInput input = GameInput.Unknown;
        while (input == GameInput.Unknown)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    {
                        return GameInput.Up;
                    }
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    {
                        return GameInput.Down;
                    }
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    {
                        return GameInput.Left;
                    }
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    {
                        return GameInput.Right;
                    }
                case ConsoleKey.Backspace:
                case ConsoleKey.U:
                    {
                        return GameInput.Undo;
                    }
                case ConsoleKey.P:
                case ConsoleKey.Escape:
                    {
                        return GameInput.Pause;
                    }
                default:
                    break;
            }
        }
        throw new InvalidOperationException("Invalid input.");
    }

    /// <summary>
    /// Provides input for a menu navigation.
    /// </summary>
    /// <returns>Input for a menu navigation.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static MenuInput ProvideMenuInput()
    {
        MenuInput input = MenuInput.Unknown;
        while (input == MenuInput.Unknown)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    {
                        return MenuInput.Up;
                    }
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    {
                        return MenuInput.Down;
                    }
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    {
                        return MenuInput.Select;
                    }
                default:
                    break;
            }
        }
        throw new InvalidOperationException("Invalid input.");
    }

    /// <summary>
    /// Stores special input types for the name form.
    /// </summary>
    readonly static Dictionary<ConsoleKey, NameFormInputType> acceptedNameFormSpecialInputs = new()
    {
        { ConsoleKey.LeftArrow, NameFormInputType.MoveLeft },
        { ConsoleKey.RightArrow, NameFormInputType.MoveRight },
        { ConsoleKey.Backspace, NameFormInputType.RemoveBefore },
        { ConsoleKey.Delete, NameFormInputType.RemoveAfter },
        { ConsoleKey.Enter, NameFormInputType.Return },
        { ConsoleKey.Escape, NameFormInputType.Cancel }
    };

    /// <summary>
    /// Provides input for a name form.
    /// </summary>
    /// <returns>Input for a name form.</returns>
    internal static NameFormInput ProvideNameFormInput()
    {
        NameFormInput input = new()
        {
            InputType = NameFormInputType.Unknown,
            InputValue = ' '
        };
        while (input.InputType == NameFormInputType.Unknown)
        {
            var keyInput = Console.ReadKey(true);
            if (acceptedNameFormSpecialInputs.ContainsKey(keyInput.Key))
            {
                input.InputType = acceptedNameFormSpecialInputs[keyInput.Key];
                continue;
            }
            if (!char.IsControl(keyInput.KeyChar))
            {
                input.InputType = NameFormInputType.Character;
                input.InputValue = keyInput.KeyChar;
            }
        }
        return input;
    }
}
