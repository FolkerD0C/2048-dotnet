using System;

namespace Game2048.Backend.Models;

public interface IHighscore : ISerializable, IComparable
{
    string PlayerName
    {
        get;
    }

    int PlayerScore
    {
        get;
    }
}