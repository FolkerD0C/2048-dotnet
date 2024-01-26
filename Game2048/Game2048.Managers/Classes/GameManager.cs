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
    readonly GameSaveHandler gameSaveHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="GameManager"/> class.
    /// </summary>
    public GameManager() : this(new(), new(), new())
    {
        GameSaveHandler.CheckOrCreateSaveDirectory();
    }

    /// <summary>
    /// Creates a new <see cref="GameManager"/> object, used for unit testing
    /// </summary>
    internal GameManager(Dictionary<Guid, IPlayInstanceManager> playManagers, HighscoreSaveHandler highscoreSaveHandler, GameSaveHandler gameSaveHandler)
    {
        this.playManagers = playManagers;
        this.highscoreSaveHandler = highscoreSaveHandler;
        this.gameSaveHandler = gameSaveHandler;
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
        return gameSaveHandler.GetSavedGames();
    }

    public IPlayInstanceManager LoadGame(string saveGameName)
    {
        IPlayInstanceManager playManager = new PlayInstanceManager(gameSaveHandler.Load(saveGameName));
        playManager.PlayEnded += OnPlayEnded;
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public IPlayInstanceManager NewGame(NewGameConfiguration gameConfiguration)
    {
        IPlayInstanceManager playManager = new PlayInstanceManager(new PlayProcessor(gameConfiguration));
        playManager.PlayEnded += OnPlayEnded;
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
        var result = gameSaveHandler.Save(playManager.Processor);
        return result;
    }

    public List<Highscore> GetHighscores()
    {
        highscoreSaveHandler.Load();
        return highscoreSaveHandler.HighscoreData.HighScores;
    }

    void OnPlayEnded(object? sender, EventArgs e)
    {
        if (sender is not null && sender is IPlayInstanceManager manager)
        {
            playManagers.Remove(manager.Id);
        }
    }
}