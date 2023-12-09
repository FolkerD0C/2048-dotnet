using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Repository.SaveDataObjects;

public class HighscoreSaveData
{
    /// <summary>
    /// A list that contains all the high scores.
    /// </summary>
    public List<Highscore> Highscores { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="HighscoreSaveData"/> class.
    /// </summary>
    public HighscoreSaveData()
    {
        Highscores = new List<Highscore>();
    }

    /// <summary>
    /// Populates the <see cref="HighscoreSaveData"/> object with save data.
    /// </summary>
    /// <param name="highscores">A list that contains all the high scores.</param>
    internal void Populate(List<Highscore> highscores)
    {
        Highscores = highscores;
    }
}
