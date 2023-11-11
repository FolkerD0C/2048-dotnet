using System;
using System.Collections.Generic;
using Game2048.Backend.Helpers.Enums;
using Game2048.Backend.Models;

namespace Game2048.Backend.Logic;

public interface IGameLogic
{
    public IPlayLogic NewGame();
    public IPlayLogic LoadGame(string saveGameName);
    public IEnumerable<string> GetSavedGames();
    public void SaveCurrentGame();
    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause);
    public IList<IHighscore> ShowHighscores();
    public void AddHighscore(string playerName, int score);
    public string GetGameDescription();
    public string GetGameHelp();
}