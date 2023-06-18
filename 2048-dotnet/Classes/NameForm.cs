using Game2048.Static;

namespace Game2048.Classes;

class Nameform
{
    enum InputType
    {
        Empty,
        Character,
        MoveLeft,
        MoveRight,
        BackSpace,
        Delete,
        Enter,
        Escape
    }

    private class FormInput
    {
        public char CharValue { get; set; }

        public InputType Type { get; set; }
    }

    static Dictionary<ConsoleKey, InputType> acceptedSpecialInputs = new Dictionary<ConsoleKey, InputType>()
    {
        { ConsoleKey.LeftArrow, InputType.MoveLeft },
        { ConsoleKey.RightArrow, InputType.MoveRight },
        { ConsoleKey.Backspace, InputType.BackSpace },
        { ConsoleKey.Delete, InputType.Delete },
        { ConsoleKey.Enter, InputType.Enter },
        { ConsoleKey.Escape, InputType.Escape }
    };

    const int maxLength = 32;
    const int horizontalOffset = 8;
    const int messagePosition = 14;
    const int formPosition = 16;

    public static string Form()
    {
        Display.NewLayout();
        Display.PrintText("Please enter your name below:", horizontalOffset, messagePosition, ConsoleColor.Black, ConsoleColor.Cyan);
        Display.PrintText(new string(' ', maxLength), horizontalOffset, formPosition, ConsoleColor.Black, ConsoleColor.Cyan);
        Display.ToggleCursor();
        Display.SetCursorPos(horizontalOffset, formPosition);

        int position = 0;
        int length = 0;
        bool inputting = true;
        char[] result = new char[maxLength];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = ' ';
        }

        Action<string, int> updateFormValue = (formValue, horizontalDisplayPosition) => {
            Display.PrintText(formValue, horizontalDisplayPosition, formPosition, ConsoleColor.Black, ConsoleColor.Cyan);
        };
        Action deleteBefore = () =>
        {
            if (position > 0)
            {
                for (int i = position; i < result.Length; i++)
                {
                    result[i - 1] = result[i];
                }
                result[^1] = ' ';
                updateFormValue(new string(result.Skip(position - 1).ToArray()), position - 1 + horizontalOffset);
                length -= 1;
                position = Math.Max(position - 1, 0);
                Display.SetCursorPos(position + horizontalOffset, formPosition);
            }
        };
        Action deleteAfter = () =>
        {
            if (position < length - 1)
            {
                for (int i = position + 1; i < result.Length; i++)
                {
                    result[i - 1] = result[i];
                }
                result[^1] = ' ';
                updateFormValue(new string(result.Skip(position).ToArray()), position + horizontalOffset);
                length -= 1;
                Display.SetCursorPos(position + horizontalOffset, formPosition);
            }
        };
        Action<char> shiftCharactersRight = currentChar =>
        {
            for (int i = position; i < length + 1; i++)
            {
                char tmp = result[i];
                result[i] = currentChar;
                currentChar = tmp;
            }
            updateFormValue(new string(result.Skip(position).ToArray()), position + horizontalOffset);
            length += 1;
            position = Math.Min(position + 1, length);
            Display.SetCursorPos(position + horizontalOffset, formPosition);
        };

        while (inputting)
        {
            var input = GetNextInput();
            switch (input.Type)
            {
                case InputType.Character:
                    {
                        if (length < maxLength)
                        {
                            shiftCharactersRight(input.CharValue);
                        }
                        break;
                    }
                case InputType.MoveLeft:
                    {
                        position = Math.Max(position - 1, 0);
                        Display.SetCursorPos(position + horizontalOffset, formPosition);
                        break;
                    }
                case InputType.MoveRight:
                    {
                        position = Math.Min(position + 1, length);
                        Display.SetCursorPos(position + horizontalOffset, formPosition);
                        break;
                    }
                case InputType.BackSpace:
                    {
                        deleteBefore();
                        break;
                    }
                case InputType.Delete:
                    {
                        deleteAfter();
                        break;
                    }
                case InputType.Enter:
                    {
                        inputting = false;
                        break;
                    }
                case InputType.Escape:
                    throw new FormCancelledException();
                default:
                    break;
            }
        }
        Display.ToggleCursor();
        Display.PreviousLayout();
        return new string(result).Trim();
    }

    static FormInput GetNextInput()
    {
        var input = Console.ReadKey(true);
        if (acceptedSpecialInputs.ContainsKey(input.Key))
        {
            return new FormInput()
            {
                Type = acceptedSpecialInputs[input.Key]
            };
        }
        if (!char.IsControl(input.KeyChar) && input.KeyChar != '"')
        {
            return new FormInput()
            {
                Type = InputType.Character,
                CharValue = input.KeyChar
            };
        }
        return new FormInput()
        {
            Type = InputType.Empty
        };
    }
}
