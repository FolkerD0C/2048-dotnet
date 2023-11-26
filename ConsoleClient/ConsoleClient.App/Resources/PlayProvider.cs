using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Logic;
using Game2048.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.App.Resources;

internal static class PlayProvider
{
    static IPlayInstance? playLogic;

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
            AppEnvironment.MainMenu.EndNavigation();
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
        if (AppEnvironment.GameLogic.GetSavedGames().Contains(playLogic.PlayerName) && !PromptPlayerOverwritingSave(playLogic.PlayerName ?? ""))
        {
            return;
        }
        var saveResult = AppEnvironment.GameLogic.SaveCurrentGame();
        switch (saveResult.ResultType)
        {
            case SaveResultType.Success:
                {
                    new MessageOverlay(saveResult.Message, MessageType.Success).PrintMessage();
                    break;
                }
            case SaveResultType.Failure:
                {
                    new MessageOverlay(saveResult.Message, MessageType.Error).PrintMessage();
                    break;
                }
            default:
                break;
        }
    }

    static bool PromptPlayerOverwritingSave(string saveGameName)
    {
        IList<IMenuItem> exitGameSubMenuItems = new List<IMenuItem>()
        {
            new MenuItem("Yes", MenuItemResult.Yes),
            new MenuItem("No", MenuItemResult.No)
        };
        IList<string> displayText = new List<string>()
        {
            "The save file already exists for '" + saveGameName + "'.",
            "Would you like to overwrite it?"
        };
        IConsoleMenu promptOverwriteMenu = new ConsoleMenu(exitGameSubMenuItems, InputProvider.ProvideMenuInput, displayText);
        promptOverwriteMenu.AddNavigationBreaker(MenuItemResult.Yes);
        promptOverwriteMenu.AddNavigationBreaker(MenuItemResult.No);
        IMenuDisplay menuDisplay = new MenuDisplay();
        promptOverwriteMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        promptOverwriteMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        promptOverwriteMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        return promptOverwriteMenu.Navigate() == MenuItemResult.Yes;
    }
}
