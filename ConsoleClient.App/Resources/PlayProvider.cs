using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using Game2048.Logic;
using Game2048.Shared.Enums;
using System;
using System.Linq;

namespace ConsoleClient.App.Resources;

internal static class PlayProvider
{
    static IConsoleMenu? mainMenu;
    static IPlayInstance? playLogic;

    internal static void SetMainMenu(IConsoleMenu? consoleMenu)
    {
        mainMenu = consoleMenu;
    }

    internal static void ProvideNewGame()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException("GameLogic is null");
        }
        playLogic = AppEnvironment.GameLogic.NewGame();
        IGameDisplay gameDisplay = new GameDisplay();
        playLogic.PlayStarted += gameDisplay.OnPlayStarted;
        playLogic.MoveHappened += gameDisplay.OnMoveHappened;
        playLogic.UndoHappened += gameDisplay.OnUndoHappened;
        playLogic.ErrorHappened += gameDisplay.OnErrorHappened;
        playLogic.MiscEventHappened += gameDisplay.MiscEventHappenedDispatcher;
        playLogic.PlayerNameChanged += gameDisplay.OnPlayerNameChanged;
        playLogic.PlayEnded += gameDisplay.OnPlayEnded;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
        HandlePlayEnded(endedReason);
    }

    internal static void ProvideLoadedGame(string saveGameName)
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException("GameLogic is null");
        }
        playLogic = AppEnvironment.GameLogic.LoadGame(saveGameName);
        IGameDisplay gameDisplay = new GameDisplay();
        playLogic.PlayStarted += gameDisplay.OnPlayStarted;
        playLogic.MoveHappened += gameDisplay.OnMoveHappened;
        playLogic.UndoHappened += gameDisplay.OnUndoHappened;
        playLogic.ErrorHappened += gameDisplay.OnErrorHappened;
        playLogic.MiscEventHappened += gameDisplay.MiscEventHappenedDispatcher;
        playLogic.PlayerNameChanged += gameDisplay.OnPlayerNameChanged;
        playLogic.PlayEnded += gameDisplay.OnPlayEnded;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
        HandlePlayEnded(endedReason);
    }

    static void HandlePlayEnded(PlayEndedReason endedReason)
    {
        if (AppEnvironment.GameLogic is null || playLogic is null)
        {
            throw new NullReferenceException("Logic can not be null.");
        }
        if (endedReason == PlayEndedReason.QuitGame)
        {
            mainMenu?.EndNavigation();
            return;
        }
        if (endedReason == PlayEndedReason.ExitPlay)
        {
            return;
        }
        if (endedReason != PlayEndedReason.GameOver)
        {
            throw new InvalidOperationException("Invalid return from playing new game.");
        }
        int lowestHighscore = AppEnvironment.GameLogic.GetHighscores().Select(hs => hs.PlayerScore).OrderBy(ps => ps).First();
        if (lowestHighscore >= playLogic.PlayerScore)
        {
            return;
        }
        if (playLogic.PlayerName is null || playLogic.PlayerName == "")
        {
            var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(playLogic.PlayerName ?? "");
            if (nameFormResult.ResultType == NameFormResultType.Cancelled)
            {
                return;
            }
            if (nameFormResult.ResultType != NameFormResultType.Success)
            {
                throw new InvalidOperationException("Invalid return from name form.");
            }
            playLogic.PlayerName = nameFormResult.Name;
        }
        AppEnvironment.GameLogic.AddHighscore(playLogic.PlayerName, playLogic.PlayerScore);
    }

    static PauseResult Pause()
    {
        return PauseMenuProvider.ProvidePauseMenuAction(ProvideChangePlayerNameAction, ProvideSaveGameAction);
    }

    internal static void ProvideChangePlayerNameAction()
    {
        if (playLogic is null)
        {
            throw new NullReferenceException("Logic can not be null.");
        }
        var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(playLogic.PlayerName ?? "");
        if (nameFormResult.ResultType == NameFormResultType.Cancelled)
        {
            return;
        }
        if (nameFormResult.ResultType != NameFormResultType.Success)
        {
            throw new InvalidOperationException("Invalid return from name form.");
        }
        playLogic.PlayerName = nameFormResult.Name;
    }

    static void ProvideSaveGameAction()
    {
        if (AppEnvironment.GameLogic is null || playLogic is null)
        {
            throw new NullReferenceException("Logic can not be null.");
        }
        if (playLogic.PlayerName is null || playLogic.PlayerName == "")
        {
            ProvideChangePlayerNameAction();
        }
        AppEnvironment.GameLogic.SaveCurrentGame();
    }
}
