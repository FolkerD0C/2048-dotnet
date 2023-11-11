using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Backend.Helpers.Enums;
using Game2048.Backend.Helpers.Exceptions;
using Game2048.Backend.Helpers.Saving;
using Game2048.Backend.Models;
using Game2048.Backend.Repository;

namespace Game2048.Backend.Logic;

public class GameLogic : IGameLogic
{
    readonly Dictionary<string, string> saveFileInfos;
    readonly IHighscoreHandler highscoreHandler;
    IPlayLogic logic;
    
    public GameLogic()
    {
        highscoreHandler = new HighScoreHandler();
        saveFileInfos = new Dictionary<string, string>();
        logic = new PlayLogic();
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
        PlayEnvironment.LoadFromSave(logic.Repository);
        return logic;
    }

    public IPlayLogic NewGame()
    {
        PlayEnvironment.LoadFromConfig();
        return logic;
    }

    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
        bool inGame = true;
        var endReason = PlayEndedReason.Unknown;
        while (inGame)
        {
            try
            {
                var input = inputMethod();
                logic.HandleInput(input);
            }
            catch (PauseRequestedException)
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
            catch (GameOverException)
            {
                inGame = false;
                endReason = PlayEndedReason.GameOver;
            }
        }
        return endReason;
    }

    public void SaveCurrentGame()
    {
        string filePath = GameSaveHandler.GetFullPathFromName(logic.Repository.PlayerName);
        IGameSaveHandler saveHandler = new GameSaveHandler(filePath);
        saveHandler.Save();
    }

    public IList<IHighscore> ShowHighscores()
    {
        return highscoreHandler.HighscoresData.HighScores;
    }
}