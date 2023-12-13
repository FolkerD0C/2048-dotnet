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
    readonly Dictionary<Guid, IPlayManager> playManagers;
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

    public IPlayInstance LoadGame(string saveGameName)
    {
        IPlayManager playManager = new PlayInstanceManager(GameSaveHandler.Load(saveGameName));
        playManagers.Add(playManager.Id, playManager);
        return playManager;
    }

    public IPlayInstance NewGame(NewGameConfiguration gameConfiguration)
    {
        IPlayManager playManager = new PlayInstanceManager(new PlayProcessor(gameConfiguration));
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
        playManagers.Remove(playId);
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
        return result;
    }

    public IList<Highscore> GetHighscores()
    {
        highscoreSaveHandler.Load();
        return highscoreSaveHandler.HighscoreData.HighScores;
    }
}