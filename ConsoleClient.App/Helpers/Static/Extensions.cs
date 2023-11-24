using System.Collections.Generic;

namespace ConsoleClient.App.Helpers;

internal static class Extensions
{
    // TODO make it split on words and move it to ConsoleClient.AppUI.Static
    internal static IList<string> Slice(this string input, int length)
    {
        IList<string> result = new List<string>();
        while (input.Length >= length)
        {
            result.Add(input[..length]);
            input = input[length..];
        }
        result.Add(input);
        return result;
    }
}
