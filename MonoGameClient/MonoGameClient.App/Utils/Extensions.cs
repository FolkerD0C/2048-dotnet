using System;

namespace MonoGameClient.App.Utils;

internal static class Extensions
{
    internal static T[] SetRange<T>(this T[] self, int destinationIndex, T[] other)
    {
        Array.Copy(other, 0, self, destinationIndex, other.Length);
        return self;
    }
}