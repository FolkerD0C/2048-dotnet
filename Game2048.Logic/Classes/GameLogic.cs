using Game2048.Config;
using Game2048.Logic.Enums;
using Game2048.Logic.Saving;
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
    IPlayLogic playLogic;

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

    public IPlayInstance LoadGame(string saveGameName)
    {
        IGameSaveHandler saveHandler = new GameSaveHandler(saveFileInfos[saveGameName]);
        saveHandler.Load();
        PlayEnvironment.LoadWithParameters(saveHandler.GameRepository.GridHeight, saveHandler.GameRepository.GridWidth);
        playLogic = new PlayLogic(saveHandler.GameRepository);
        return playLogic;
    }

    public IPlayInstance NewGame()
    {
        PlayEnvironment.LoadWithParameters(ConfigManager.GetConfigItem<int>("DefaultGridHeight"), ConfigManager.GetConfigItem<int>("DefaultGridWidth"));
        playLogic = new PlayLogic();
        return playLogic;
    }

    public PlayEndedReason Play(Func<GameInput> inputMethod, Func<PauseResult> handlePause)
    {
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

    public void SaveCurrentGame()
    {
        string filePath = GameSaveHandler.GetFullPathFromName(playLogic.PlayerName);
        IGameSaveHandler saveHandler = new GameSaveHandler(filePath);
        saveHandler.Save();
    }

    public IList<IHighscore> GetHighscores()
    {
        return highscoreHandler.HighscoresData.HighScores;
    }
}