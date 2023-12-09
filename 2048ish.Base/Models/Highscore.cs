namespace _2048ish.Base.Models;

/// <summary>
/// A class that stores information about a high score.
/// </summary>
public record Highscore
{
    /// <summary>
    /// The name of the player who achieved the high score.
    /// </summary>
    public string PlayerName { get; set; }

    /// <summary>
    /// The score of the player who achieved the high score.
    /// </summary>
    public int PlayerScore { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="Highscore"/> class.
    /// </summary>
    public Highscore()
    {
        PlayerName = "";
    }
}