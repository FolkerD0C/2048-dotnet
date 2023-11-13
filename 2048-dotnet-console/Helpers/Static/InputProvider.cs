using System;
using Game2048.Backend.Helpers.Enums;
using Game2048.ConsoleFrontend.Helpers.Enums;

namespace Game2048.ConsoleFrontend.Helpers;
public static class InputProvider
{
    public static GameInput ProvidePlayInput()
    {
        GameInput input = GameInput.Unknown;
        while (input == GameInput.Unknown)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W: case ConsoleKey.UpArrow:
                {
                    return GameInput.Up;
                }
                case ConsoleKey.S: case ConsoleKey.DownArrow:
                {
                    return GameInput.Down;
                }
                case ConsoleKey.A: case ConsoleKey.LeftArrow:
                {
                    return GameInput.Left;
                }
                case ConsoleKey.D: case ConsoleKey.RightArrow:
                {
                    return GameInput.Right;
                }
                case ConsoleKey.Backspace: case ConsoleKey.U:
                {
                    return GameInput.Undo;
                }
                case ConsoleKey.P: case ConsoleKey.Escape:
                {
                    return GameInput.Pause;
                }
                default:
                    break;
            }
        }
        throw new InvalidOperationException("Invalid input.");
    }

    public static MenuInput ProvideMenuInput()
    {
        MenuInput input = MenuInput.Unknown;
        while (input == MenuInput.Unknown)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W: case ConsoleKey.UpArrow:
                {
                    return MenuInput.Up;
                }
                case ConsoleKey.S: case ConsoleKey.DownArrow:
                {
                    return MenuInput.Down;
                }
                case ConsoleKey.Enter: case ConsoleKey.Spacebar:
                {
                    return MenuInput.Select;
                }
                default:
                    break;
            }
        }
        throw new InvalidOperationException("Invalid input.");
    }
}