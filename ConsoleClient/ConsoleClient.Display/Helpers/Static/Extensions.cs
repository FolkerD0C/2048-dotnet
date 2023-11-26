using System.Collections.Generic;

namespace ConsoleClient.Display.Helpers;

public static class Extensions
{
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
