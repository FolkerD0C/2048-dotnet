using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Helpers;

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