using System.Collections.Generic;

namespace ConsoleClient.Display.Helpers;

/// <summary>
/// A static class that contains extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// A method that disposes <paramref name="genericList"/>.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="genericList"/></typeparam>
    /// <param name="genericList">The list to be disposed.</param>
    public static void Dispose<T>(this IList<T>? genericList)
    {
        if (genericList is not null)
        {
            genericList.Clear();
            if (genericList is List<T> genericListImplementation)
            {
                genericListImplementation.TrimExcess();
            }
        }
    }
}
