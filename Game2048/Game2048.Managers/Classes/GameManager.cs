using Game2048.Base.Models;
using Game2048.Config;
using Game2048.Managers.Enums;
using Game2048.Managers.EventHandlers;
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
    readonly Dictionary<string, Guid> saveNameToIdMap;
    readonly Dictionary<Guid, IPlayManager> playManagers;
    readonly HighscoreSaveHandler highscoreSaveHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="GameManager"/> class.
    /// </summary>
    public GameManager()
    {
        saveNameToIdMap = new();
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

    public IPlayInstance LoadGame(string saveGameName)
    {
        if (saveNameToIdMap.ContainsKey(saveGameName))
        {
            return playManagers[saveNameToIdMap[saveGameName]];
        }
        IPlayManager playManager = new PlayManager(GameSaveHandler.Load(saveGameName));
        playManager.PlayerNameChangedManagerEvent += OnPlayerNameChanged;
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public IPlayInstance NewGame()
    {
        IPlayManager playManager = new PlayManager(new PlayProcessor());
        playManager.PlayerNameChangedManagerEvent += OnPlayerNameChanged;
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public PlayEndedReason Play(Guid playId, Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
        if (!playManagers.ContainsKey(playId))
        {
            return PlayEndedReason.PlayNotInitialized;
        }
        IPlayManager playManager = playManagers[playId];
        bool inGame = true;
        var endReason = PlayEndedReason.Unknown;
        playManager.Start();
        while (inGame)
        {
            playManager.PreInput();
            var input = inputMethod();
            var inputResult = playManager.HandleInput(input);

            if (inputResult == InputResult.Continue)
            {
                continue;
            }

            if (inputResult == InputResult.Pause)
            {
                var pauseResult = handlePause();
                if (pauseResult == PauseResult.Continue)
                {
                    continue;
                }
                if (pauseResult == PauseResult.EndPlay)
                {
                    inGame = false;
                    endReason = PlayEndedReason.ExitPlay;
                }
                else if (pauseResult == PauseResult.ExitGame)
                {
                    inGame = false;
                    endReason = PlayEndedReason.QuitGame;
                }
            }
            else if (inputResult == InputResult.GameOver)
            {
                inGame = false;
                endReason = PlayEndedReason.GameOver;
            }
        }
        playManager.End();
        if (!playManager.IsSaved)
        {
            playManagers.Remove(playId);
        }
        return endReason;
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
        IPlayManager playManager = playManagers[playId];
        if (playManager.PlayerName is null || playManager.PlayerName == "")
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Player name can not be empty, aborting save..."
            };
        }
        var result = GameSaveHandler.Save(playManager.Processor);
        if (result.ResultType == SaveResultType.Success)
        {
            playManager.IsSaved = true;
            if (!saveNameToIdMap.ContainsKey(playManager.PlayerName))
            {
                saveNameToIdMap.Add(playManager.PlayerName, playId);
            }
        }
        return result;
    }

    public IList<Highscore> GetHighscores()
    {
        highscoreSaveHandler.Load();
        return highscoreSaveHandler.HighscoreData.HighScores;
    }

    /// <summary>
    /// Handles player name changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnPlayerNameChanged(object? sender, PlayerNameChangedEventArgs args)
    {
        if (!saveNameToIdMap.ContainsKey(args.OldName))
        {
            return;
        }
        var playId = saveNameToIdMap[args.OldName];
        saveNameToIdMap.Remove(args.OldName);
        saveNameToIdMap.Add(args.NewName, playId);
    }
}