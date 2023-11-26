using System;
using System.Text.Json;

namespace Game2048.Shared.Models;

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
        var deserializedData = JsonSerializer.Deserialize<Tuple<string, int>>(deserializee) ?? throw new InvalidOperationException($"'{deserializee}' can not be deserialized.");
        playerName = deserializedData.Item1;
        playerScore = deserializedData.Item2;
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(Tuple.Create(playerName, playerScore));
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
        throw new InvalidOperationException("The object '" + (obj is not null ? obj?.ToString() : "") + "' is not comparable with " + this.ToString());
    }
}