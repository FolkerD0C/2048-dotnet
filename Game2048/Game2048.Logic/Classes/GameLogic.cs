using Game2048.Config;
using Game2048.Logic.Enums;
using Game2048.Logic.Saving;
using Game2048.Repository;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

/// <summary>
/// A class that represents a high level manager for the game.
/// Handles playing, saving, loading, high scores and gets the game description.
/// </summary>
public class GameLogic : IGameLogic
{
    readonly Dictionary<string, string> saveFileInfos;
    readonly IHighscoreSaveHandler highscoreHandler;
    IGameSaveHandler? saveHandler;
    IPlayLogic? playLogic;

    /// <summary>
    /// Creates a new instance of the <see cref="GameLogic"/> class.
    /// </summary>
    public GameLogic()
    {
        highscoreHandler = new HighScoreSaveHandler();
        saveFileInfos = new Dictionary<string, string>();
        GameSaveHandler.CheckOrCreateSaveDirectory();
    }

    public void AddHighscore(string playerName, int score)
    {
        highscoreHandler.AddNewHighscore(playerName, score);
        highscoreHandler.Save();
    }

    public string GetGameDescription()
    {
        return GameData.GameDescription;
    }

    public IEnumerable<string> GetSavedGames()
    {
        var saveFiles = GameSaveHandler.GetSavedGames();
        saveFileInfos.Clear();
        foreach (var saveFile in saveFiles)
        {
            saveFileInfos.Add(saveFile.Name, saveFile.Fullpath);
        }
        return saveFiles.Select(saveFile => saveFile.Name);
    }

    public IPlayInstance LoadGame(string saveGameName)
    {
        saveHandler = new GameSaveHandler(saveFileInfos[saveGameName], default);
        saveHandler.Load();
        //PlayEnvironment.LoadWithParameters(saveHandler.GameRepository.GridHeight, saveHandler.GameRepository.GridWidth);
        playLogic = new PlayLogic(saveHandler.GameRepository);
        return playLogic;
    }

    public IPlayInstance NewGame()
    {
        //PlayEnvironment.LoadWithParameters(ConfigManager.GetConfigItemValue<int>("DefaultGridHeight"), ConfigManager.GetConfigItemValue<int>("DefaultGridWidth"));
        saveHandler = new GameSaveHandler("", new GameRepository());
        playLogic = new PlayLogic(saveHandler.GameRepository);
        return playLogic;
    }

    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
        if (playLogic is null || saveHandler is null)
        {
            return PlayEndedReason.PlayNotInitialized;
        }
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
        return endReason;
    }

    public SaveResult SaveCurrentGame()
    {
        if (playLogic is null || saveHandler is null)
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Play is not initialized"
            };
        }
        if (playLogic.PlayerName is null || playLogic.PlayerName == "")
        {
            return new SaveResult()
            {
                ResultType = SaveResultType.Failure,
                Message = "Player name can not be empty, aborting save..."
            };
        }
        string filePath = GameSaveHandler.GetFullPathFromName(playLogic.PlayerName);
        saveHandler.UpdateFilePath(filePath);
        return saveHandler.Save();
    }

    public IList<Highscore> GetHighscores()
    {
        highscoreHandler.Load();
        return highscoreHandler.HighscoresData.HighScores;
    }
}