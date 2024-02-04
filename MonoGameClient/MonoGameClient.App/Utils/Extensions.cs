using System;

namespace MonoGameClient.App.Utils;

internal static class Extensions
{
    internal static T[] SetRange<T>(this T[] self, int index, T[] other)
    {
        Array.Copy(other, 0, self, index, other.Length);
        return self;
    }
}