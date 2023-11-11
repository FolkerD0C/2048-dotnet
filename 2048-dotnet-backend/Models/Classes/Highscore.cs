using System;
using System.Text.Json;
using Game2048.Backend.Helpers.Exceptions;

namespace Game2048.Backend.Models;

public class Highscore : IHighscore
{
    string playerName;
    public string PlayerName { get => playerName; }

    int playerScore;
    public int PlayerScore { get => playerScore; }

    public Highscore() : this("", -1)
    { }

    public Highscore(string playerName, int playerScore)
    {
        this.playerName = playerName;
        this.playerScore = playerScore;
    }

    public void Deserialize(string deserializee)
    {
        var deserializedData = JsonSerializer.Deserialize<(string, int)>(deserializee);
        playerName = deserializedData.Item1;
        playerScore = deserializedData.Item2;
    }

    public string Serialize()
    {
        var serializedData = JsonSerializer.Serialize<(string, int)>((playerName, playerScore));
        return serializedData;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not null && obj is Highscore other)
        {
            if (playerScore > other.playerScore)
            {
                return -1;
            }
            else if (playerScore < other.playerScore)
            {
                return 1;
            }
            else if (playerName.CompareTo(other.PlayerName) < 0)
            {
                return -1;
            }
            else if (playerName.CompareTo(other.playerName) > 0)
            {
                return 1;
            }
            return 0;
        }
        throw new NotComparableException(this, obj ?? this);
    }
}