using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace Game2048.Logic;

public interface IGameLogic
{
    public IPlayInstance NewGame();
    public IPlayInstance LoadGame(string saveGameName);
    public IEnumerable<string> GetSavedGames();
    public SaveResult SaveCurrentGame();
    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause);
    public IList<IHighscore> GetHighscores();
    public void AddHighscore(string playerName, int score);
    public string GetGameDescription();
    public string GetGameHelp();
}