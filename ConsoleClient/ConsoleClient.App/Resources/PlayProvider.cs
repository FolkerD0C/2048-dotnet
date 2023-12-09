using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.App.Resources;

/// <summary>
/// A static class that provides play actions for the game.
/// </summary>
internal static class PlayProvider
{
    /// <summary>
    /// Performs a new game action.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    internal static void ProvideNewGame()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException("GameLogic is null");
        }
        AppEnvironment.CurrentPlayInstance = AppEnvironment.GameLogic.NewGame();
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        AppEnvironment.CurrentPlayInstance.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstance.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstance.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstance.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstance.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstance.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstance.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(AppEnvironment.CurrentPlayInstance.Id, InputProvider.ProvidePlayInput, Pause);
        HandlePlayEnded(endedReason);
        AppEnvironment.CurrentPlayInstance = null;
        AppEnvironment.CurrentOverlays.Remove("currentPlayInstanceOverlay");
    }

    /// <summary>
    /// Performs a loaded game action.
    /// </summary>
    /// <param name="saveGameName"></param>
    /// <exception cref="NullReferenceException"></exception>
    internal static void ProvideLoadedGame(string saveGameName)
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException("GameLogic is null");
        }
        AppEnvironment.CurrentPlayInstance = AppEnvironment.GameLogic.LoadGame(saveGameName);
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        currentPlayInstanceOverlay.SetPreviousOverlaySuppression(true);
        AppEnvironment.CurrentPlayInstance.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstance.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstance.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstance.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstance.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstance.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstance.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(AppEnvironment.CurrentPlayInstance.Id, InputProvider.ProvidePlayInput, Pause);
        HandlePlayEnded(endedReason);
        AppEnvironment.CurrentPlayInstance = null;
        AppEnvironment.CurrentOverlays.Remove("currentPlayInstanceOverlay");
    }

    /// <summary>
    /// Handles the ending of a play action.
    /// </summary>
    /// <param name="endedReason">The reason the play has ended.</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    static void HandlePlayEnded(PlayEndedReason endedReason)
    {
        if (AppEnvironment.GameLogic is null || AppEnvironment.CurrentPlayInstance is null)
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
            throw new InvalidOperationException("Invalid return from playing the game.");
        }
        int lowestHighscore = AppEnvironment.GameLogic.GetHighscores()
            .Select(highscore => highscore.PlayerScore).OrderBy(playerscore => playerscore).First();
        if (lowestHighscore >= AppEnvironment.CurrentPlayInstance.PlayerScore)
        {
            return;
        }
        var congratsMessage = new MessageOverlay("Congratulations! You set a new highscore.", MessageType.Success);
        congratsMessage.PrintMessage();
        if (AppEnvironment.CurrentPlayInstance.PlayerName is null || AppEnvironment.CurrentPlayInstance.PlayerName == "")
        {
            var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(AppEnvironment.CurrentPlayInstance.PlayerName ?? "");
            if (nameFormResult.ResultType == NameFormResultType.Cancelled || AppEnvironment.CurrentPlayInstance.PlayerName == "")
            {
                return;
            }
            if (nameFormResult.ResultType != NameFormResultType.Success)
            {
                throw new InvalidOperationException("Invalid return from name form.");
            }
            AppEnvironment.CurrentPlayInstance.PlayerName = nameFormResult.Name;
        }
        AppEnvironment.GameLogic.AddHighscore(AppEnvironment.CurrentPlayInstance.PlayerName, AppEnvironment.CurrentPlayInstance.PlayerScore);
    }

    /// <summary>
    /// Provides a pause function for the game.
    /// </summary>
    /// <returns>The result of the pause.</returns>
    static PauseResult Pause()
    {
        return PauseMenuProvider.ProvidePauseMenuAction(ProvideChangePlayerNameAction, ProvideSaveGameAction);
    }
    /// <summary>
    /// Provides a method to change the player's name.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void ProvideChangePlayerNameAction()
    {
        if (AppEnvironment.CurrentPlayInstance is null)
        {
            throw new NullReferenceException("Logic can not be null.");
        }
        var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(AppEnvironment.CurrentPlayInstance.PlayerName ?? "");
        if (nameFormResult.ResultType == NameFormResultType.Cancelled)
        {
            return;
        }
        if (nameFormResult.ResultType != NameFormResultType.Success)
        {
            throw new InvalidOperationException("Invalid return from name form.");
        }
        AppEnvironment.CurrentPlayInstance.PlayerName = nameFormResult.Name;
    }

    /// <summary>
    /// Provides an action for saving the current game.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    static void ProvideSaveGameAction()
    {
        if (AppEnvironment.GameLogic is null || AppEnvironment.CurrentPlayInstance is null)
        {
            throw new NullReferenceException("Logic can not be null.");
        }
        if (AppEnvironment.CurrentPlayInstance.PlayerName is null || AppEnvironment.CurrentPlayInstance.PlayerName == "")
        {
            ProvideChangePlayerNameAction();
        }
        if (AppEnvironment.GameLogic.GetSavedGames().Contains(AppEnvironment.CurrentPlayInstance.PlayerName)
            && !PromptPlayerOverwritingSave(AppEnvironment.CurrentPlayInstance.PlayerName ?? ""))
        {
            return;
        }
        var saveResult = AppEnvironment.GameLogic.SaveGame(AppEnvironment.CurrentPlayInstance.Id);
        switch (saveResult.ResultType)
        {
            case SaveResultType.Success:
                {
                    new MessageOverlay(saveResult.Message ?? "Game saved succesfully.", MessageType.Success).PrintMessage();
                    break;
                }
            case SaveResultType.Failure:
                {
                    new MessageOverlay(saveResult.Message ?? "Game saving failed for unknown reason.", MessageType.Error).PrintMessage();
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    /// Prompts the player about overwriting an existing savefile.
    /// </summary>
    /// <param name="saveGameName">The name of the saved game.</param>
    /// <returns>True if the application can overwrite the existing save file.</returns>
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
        IMenuDisplay promptOverwriteMenuOverlay = new MenuDisplay();
        promptOverwriteMenu.MenuNavigationStarted += promptOverwriteMenuOverlay.OnMenuNavigationStarted;
        promptOverwriteMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("promptOverwriteMenu", promptOverwriteMenu);
            AppEnvironment.CurrentOverlays.Add("promptOverwriteMenuOverlay", promptOverwriteMenuOverlay);
        };
        promptOverwriteMenu.MenuSelectionChanged += promptOverwriteMenuOverlay.OnMenuSelectionChanged;
        promptOverwriteMenu.MenuNavigationEnded += promptOverwriteMenuOverlay.OnMenuNavigationEnded;
        promptOverwriteMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("promptOverwriteMenu");
            AppEnvironment.CurrentOverlays.Remove("promptOverwriteMenuOverlay");
        };
        return promptOverwriteMenu.Navigate() == MenuItemResult.Yes;
    }
}
