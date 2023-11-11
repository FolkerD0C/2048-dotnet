using System;

namespace Game2048.Backend.Helpers.Exceptions;

public class NotComparableException : Exception
{
    public NotComparableException(object item1, object item2) : base($"{nameof(item1)} and {nameof(item2)} are not comparable or one of them is null.")
    { }
}