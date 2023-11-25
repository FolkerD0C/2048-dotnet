using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.Menu.Enums;
using Game2048.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.App.Resources;

internal static class InputProvider
{
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

    readonly static Dictionary<ConsoleKey, NameFormInputType> acceptedNameFormSpecialInputs = new ()
    {
        { ConsoleKey.LeftArrow, NameFormInputType.MoveLeft },
        { ConsoleKey.RightArrow, NameFormInputType.MoveRight },
        { ConsoleKey.Backspace, NameFormInputType.RemoveBefore },
        { ConsoleKey.Delete, NameFormInputType.RemoveAfter },
        { ConsoleKey.Enter, NameFormInputType.Return },
        { ConsoleKey.Escape, NameFormInputType.Cancel }
    };


    internal static NameFormInput ProvideNameFormInput()
    {
        NameFormInput input = new NameFormInput()
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
