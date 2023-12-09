using Game2048.Config;
using Game2048.Logic.Enums;
using Game2048.Logic.EventHandlers;
using Game2048.Logic.Models;
using Game2048.Logic.Saving;
using Game2048.Repository;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace Game2048.Logic;

/// <summary>
/// A class that represents a high level manager for the game.
/// Handles playing, saving, loading, high scores and gets the game description.
/// </summary>
public class GameLogic : IGameLogic
{
    readonly Dictionary<string, Guid> saveNameToIdMap;
    readonly Dictionary<Guid, IPlayLogic> playInstances;
    readonly HighscoreSaveHandler highscoreSaveHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="GameLogic"/> class.
    /// </summary>
    public GameLogic()
    {
        saveNameToIdMap = new();
        playInstances = new();
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
            return playInstances[saveNameToIdMap[saveGameName]];
        }
        var playLogic = new PlayLogic(GameSaveHandler.Load(saveGameName));
        playLogic.PlayerNameChangedManagerEvent += OnPlayerNameChanged;
        playInstances.Add(playLogic.Id, playLogic);
        return playLogic;
    }

    public IPlayInstance NewGame()
    {
        var playLogic = new PlayLogic(new GameRepository());
        playLogic.PlayerNameChangedManagerEvent += OnPlayerNameChanged;
        playInstances.Add(playLogic.Id, playLogic);
        return playLogic;
    }

    public PlayEndedReason Play(Guid playId, Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
        if (!playInstances.ContainsKey(playId))
        {
            return PlayEndedReason.PlayNotInitialized;
        }
        var playLogic = playInstances[playId];
        bool inGame = true;
        var endReason = PlayEndedReason.Unknown;
        playLogic.Start();
        while (inGame)
        {
            playLogic.PreInput();
            var input = inputMethod();
            var inputResult = playLogic.HandleInput(input);

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
        playLogic.End();
        if (!playLogic.IsSaved)
        {
            playInstances.Remove(playId);
        }
        return endReason;
    }

    public SaveResult SaveGame(Guid playId)
    {
        if (!playInstances.ContainsKey(playId))
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Invalid play ID."
            };
        }
        var playLogic = playInstances[playId];
        if (playLogic.PlayerName is null || playLogic.PlayerName == "")
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Player name can not be empty, aborting save..."
            };
        }
        var result = GameSaveHandler.Save(playLogic.Processor);
        if (result.ResultType == SaveResultType.Success)
        {
            playLogic.IsSaved = true;
            if (!saveNameToIdMap.ContainsKey(playLogic.PlayerName))
            {
                saveNameToIdMap.Add(playLogic.PlayerName, playId);
            }
        }
        return result;
    }

    public IList<Highscore> GetHighscores()
    {
        highscoreSaveHandler.Load();
        return highscoreSaveHandler.HighscoresData.HighScores;
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