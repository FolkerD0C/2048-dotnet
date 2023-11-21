using System.Collections.Generic;

namespace ConsoleClient.App.Helpers;

internal static class Extensions
{
    // TODO make it slice on words
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
