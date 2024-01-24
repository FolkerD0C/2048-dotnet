using _2048ish.Base.Models;
using Game2048.Config;
using Game2048.Managers.Enums;
using Game2048.Managers.Models;
using Game2048.Managers.Saving;
using Game2048.Processors;
using System;
using System.Collections.Generic;

namespace Game2048.Managers;

/// <summary>
/// A class that represents a high level manager for the game.
/// Handles playing, saving, loading, high scores and gets the game description.
/// </summary>
public class GameManager : IGameManager
{
    readonly Dictionary<Guid, IPlayInstanceManager> playManagers;
    readonly HighscoreSaveHandler highscoreSaveHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="GameManager"/> class.
    /// </summary>
    public GameManager()
    {
        playManagers = new();
        highscoreSaveHandler = new();
        GameSaveHandler.CheckOrCreateSaveDirectory();
    }

    public void AddHighscore(string playerName, int score)
    {
        highscoreSaveHandler.AddNewHighscore(playerName, score);
        highscoreSaveHandler.Save();
    }

    public string GetGameDescription()
    {
        return GameData.GameDescription;
    }

    public IEnumerable<string> GetSavedGames()
    {
        return GameSaveHandler.GetSavedGames();
    }

    public IPlayInstanceManager LoadGame(string saveGameName)
    {
        IPlayInstanceManager playManager = new PlayInstanceManager(GameSaveHandler.Load(saveGameName));
        if (playManagers.ContainsKey(playManager.Id))
        {
            playManagers.Remove(playManager.Id);
        }
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public IPlayInstanceManager NewGame(NewGameConfiguration gameConfiguration)
    {
        IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(gameConfiguration));
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public SaveResult SaveGame(Guid playId)
    {
        if (!playManagers.ContainsKey(playId))
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Invalid play ID."
            };
        }
        IPlayInstanceManager playManager = playManagers[playId];
        if (playManager.PlayerName is null || playManager.PlayerName == "")
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Player name can not be empty, aborting save..."
            };
        }
        var result = GameSaveHandler.Save(playManager.Processor);
        return result;
    }

    public IList<Highscore> GetHighscores()
    {
        highscoreSaveHandler.Load();
        return highscoreSaveHandler.HighscoreData.HighScores;
    }
}