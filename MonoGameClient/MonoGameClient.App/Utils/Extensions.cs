using System;

namespace MonoGameClient.App.Utils;

internal static class Extensions
{
    internal static T[] SetRange<T>(this T[] self, T[] other, int destinationIndex)
    {
        Array.Copy(other, 0, self, destinationIndex, other.Length);
        return self;
    }
}