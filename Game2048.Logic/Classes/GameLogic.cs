using Game2048.Config;
using Game2048.Logic.Enums;
using Game2048.Logic.Saving;
using Game2048.Repository.Exceptions;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

public class GameLogic : IGameLogic
{
    readonly Dictionary<string, string> saveFileInfos;
    readonly IHighscoreHandler highscoreHandler;
    IPlayLogic logic;

#pragma warning disable CS8618
    public GameLogic()
    {
        highscoreHandler = new HighScoreHandler();
        saveFileInfos = new Dictionary<string, string>();
        GameSaveHandler.CheckOrCreateSaveDirectory();
    }
#pragma warning restore CS8618

    public void AddHighscore(string playerName, int score)
    {
        highscoreHandler.AddNewHighscore(playerName, score);
        highscoreHandler.Save();
    }

    public string GetGameDescription()
    {
        return GameData.GameDescription;
    }

    public string GetGameHelp()
    {
        return GameData.GameHelp;
    }

    public IEnumerable<string> GetSavedGames()
    {
        var saveFiles = GameSaveHandler.GetSavedGames();
        foreach (var saveFile in saveFiles)
        {
            saveFileInfos.Add(saveFile.Name, saveFile.Fullpath);
        }
        return saveFiles.Select(saveFile => saveFile.Name);
    }

    public IPlayLogic LoadGame(string saveGameName)
    {
        IGameSaveHandler saveHandler = new GameSaveHandler(saveFileInfos[saveGameName]);
        saveHandler.Load();
        logic = PlayLogic.GetLogicFromSave(saveHandler.GameRepository);
        PlayEnvironment.LoadWithParameters(saveHandler.GameRepository.GridHeight, saveHandler.GameRepository.GridWidth);
        return logic;
    }

    public IPlayLogic NewGame()
    {
        logic = new PlayLogic();
        PlayEnvironment.LoadWithParameters(ConfigManager.GetConfigItem("DefaultGridHeight", default(int)), ConfigManager.GetConfigItem("DefaultGridWidth", default(int)));
        return logic;
    }

    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
        bool inGame = true;
        var endReason = PlayEndedReason.Unknown;
        while (inGame)
        {
            var inputResult = InputResult.Unknown;
            try
            {
                var input = inputMethod();
                logic.HandleInput(input);
            }
            catch (GameOverException)
            {
                inGame = false;
                endReason = PlayEndedReason.GameOver;
            }

            if (inputResult == InputResult.Pause)
            {
                var pauseResult = handlePause();
                if (pauseResult == PauseResult.Save || pauseResult == PauseResult.SaveAndExit)
                {
                    SaveCurrentGame();
                }
                if (pauseResult == PauseResult.SaveAndExit || pauseResult == PauseResult.Exit)
                {
                    inGame = false;
                    endReason = PlayEndedReason.Exit;
                }
            }
        }
        return endReason;
    }

    public void SaveCurrentGame()
    {
        string filePath = GameSaveHandler.GetFullPathFromName(logic.PlayerName);
        IGameSaveHandler saveHandler = new GameSaveHandler(filePath);
        saveHandler.Save();
    }

    public IList<IHighscore> GetHighscores()
    {
        return highscoreHandler.HighscoresData.HighScores;
    }
}