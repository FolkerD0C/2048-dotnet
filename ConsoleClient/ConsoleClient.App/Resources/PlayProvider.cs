using ConsoleClient.App.Resources.Enums;
using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Managers;
using Game2048.Managers.Enums;
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
        if (AppEnvironment.GameConfiguration is null)
        {
            throw new NullReferenceException("Initializing new game failed, non-existent game configuration.");
        }
        AppEnvironment.CurrentPlayInstanceManager = AppEnvironment.GameManager.NewGame(AppEnvironment.GameConfiguration);
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        AppEnvironment.CurrentPlayInstanceManager.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstanceManager.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstanceManager.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstanceManager.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstanceManager.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstanceManager.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstanceManager.InputProcessingFinished += currentPlayInstanceOverlay.OnInputProcessingFinished;
        AppEnvironment.CurrentPlayInstanceManager.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        PlayEndedReason endedReason = Play(AppEnvironment.CurrentPlayInstanceManager);
        HandlePlayEnded(endedReason);
        AppEnvironment.CurrentPlayInstanceManager = null;
        AppEnvironment.CurrentOverlays.Remove("currentPlayInstanceOverlay");
    }

    /// <summary>
    /// Performs a loaded game action.
    /// </summary>
    /// <param name="saveGameName"></param>
    /// <exception cref="NullReferenceException"></exception>
    internal static void ProvideLoadedGame(string saveGameName)
    {
        if (AppEnvironment.GameManager is null)
        {
            throw new NullReferenceException("GameManager is null");
        }
        AppEnvironment.CurrentPlayInstanceManager = AppEnvironment.GameManager.LoadGame(saveGameName);
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        currentPlayInstanceOverlay.SetPreviousOverlaySuppression(true);
        AppEnvironment.CurrentPlayInstanceManager.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstanceManager.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstanceManager.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstanceManager.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstanceManager.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstanceManager.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstanceManager.InputProcessingFinished += currentPlayInstanceOverlay.OnInputProcessingFinished;
        AppEnvironment.CurrentPlayInstanceManager.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        AppEnvironment.CurrentOverlays["currentPlayInstanceOverlay"].SetPreviousOverlaySuppression(true);
        PlayEndedReason endedReason = Play(AppEnvironment.CurrentPlayInstanceManager);
        HandlePlayEnded(endedReason);
        AppEnvironment.CurrentMenus["loadGameMenu"].EndNavigation();
        AppEnvironment.CurrentPlayInstanceManager = null;
        AppEnvironment.CurrentOverlays.Remove("currentPlayInstanceOverlay");
    }

    /// <summary>
    /// Performs a play action on the play instance specified by <paramref name="playInstanceManager"/>.
    /// </summary>
    /// <param name="playInstanceManager">The play instace to perform the play action on.</param>
    /// <returns></returns>
    static PlayEndedReason Play(IPlayInstanceManager playInstanceManager)
    {
        bool inGame = true;
        var endReason = PlayEndedReason.Unknown;
        playInstanceManager.Start();
        while (inGame)
        {
            var input = GetPlayInput();
            if (input == GameInput.EndPlay)
            {
                inGame = false;
                endReason = PlayEndedReason.Exit;
                continue;
            }
            var inputResult = playInstanceManager.HandleInput(input);

            if (inputResult == InputResult.GameOver)
            {
                inGame = false;
                endReason = PlayEndedReason.GameOver;
            }
        }
        playInstanceManager.End();
        return endReason;
    }

    /// <summary>
    /// Converts <see cref="ExtendedGameInput"/> into <see cref="GameInput"/>, which then can be provided to an ongoing play.
    /// </summary>
    /// <returns>A valid or unknown play input.</returns>
    static GameInput GetPlayInput()
    {
        ExtendedGameInput input = InputProvider.ProvidePlayInput();
        if (input == ExtendedGameInput.Pause)
        {
            var pauseResult = Pause();
            switch (pauseResult)
            {
                case PauseResult.EndPlay:
                    {
                        return GameInput.EndPlay;
                    }
                case PauseResult.ExitGame:
                    {
                        AppEnvironment.MainMenu.EndNavigation();
                        return GameInput.EndPlay;
                    }
                default:
                    return GameInput.Unknown;
            }
        }
        else
        {
            return input switch
            {
                ExtendedGameInput.Up => GameInput.Up,
                ExtendedGameInput.Down => GameInput.Down,
                ExtendedGameInput.Left => GameInput.Left,
                ExtendedGameInput.Right => GameInput.Right,
                ExtendedGameInput.Undo => GameInput.Undo,
                _ => GameInput.Unknown
            };
        }
    }

    /// <summary>
    /// Handles the ending of a play action.
    /// </summary>
    /// <param name="endedReason">The reason the play has ended.</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    static void HandlePlayEnded(PlayEndedReason endedReason)
    {
        if (AppEnvironment.GameManager is null || AppEnvironment.CurrentPlayInstanceManager is null)
        {
            throw new NullReferenceException("Managers can not be null.");
        }
        if (endedReason != PlayEndedReason.GameOver)
        {
            return;
        }
        int lowestHighscore = AppEnvironment.GameManager.GetHighscores()
            .Select(highscore => highscore.PlayerScore).OrderBy(playerscore => playerscore).First();
        if (lowestHighscore >= AppEnvironment.CurrentPlayInstanceManager.PlayerScore)
        {
            return;
        }
        var congratsMessage = new MessageOverlay("Congratulations! You set a new highscore.", MessageType.Success);
        congratsMessage.PrintMessage();
        if (AppEnvironment.CurrentPlayInstanceManager.PlayerName is null || AppEnvironment.CurrentPlayInstanceManager.PlayerName == "")
        {
            var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(AppEnvironment.CurrentPlayInstanceManager.PlayerName ?? "");
            if (nameFormResult.ResultType == NameFormResultType.Cancelled || AppEnvironment.CurrentPlayInstanceManager.PlayerName == "")
            {
                return;
            }
            AppEnvironment.CurrentPlayInstanceManager.PlayerName = nameFormResult.Name;
        }
        AppEnvironment.GameManager.AddHighscore(AppEnvironment.CurrentPlayInstanceManager.PlayerName, AppEnvironment.CurrentPlayInstanceManager.PlayerScore);
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
        if (AppEnvironment.CurrentPlayInstanceManager is null)
        {
            throw new NullReferenceException("Play instance can not be null.");
        }
        var nameFormResult = new NameForm(InputProvider.ProvideNameFormInput).PromptPlayerName(AppEnvironment.CurrentPlayInstanceManager.PlayerName ?? "");
        if (nameFormResult.ResultType == NameFormResultType.Cancelled)
        {
            return;
        }
        if (nameFormResult.ResultType != NameFormResultType.Success)
        {
            throw new InvalidOperationException("Invalid return from name form.");
        }
        AppEnvironment.CurrentPlayInstanceManager.PlayerName = nameFormResult.Name;
    }

    /// <summary>
    /// Provides an action for saving the current game.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    static void ProvideSaveGameAction()
    {
        if (AppEnvironment.GameManager is null || AppEnvironment.CurrentPlayInstanceManager is null)
        {
            throw new NullReferenceException("Managers can not be null.");
        }
        if (AppEnvironment.CurrentPlayInstanceManager.PlayerName is null || AppEnvironment.CurrentPlayInstanceManager.PlayerName == "")
        {
            ProvideChangePlayerNameAction();
        }
        if (AppEnvironment.GameManager.GetSavedGames().Contains(AppEnvironment.CurrentPlayInstanceManager.PlayerName)
            && !PromptPlayerOverwritingSave(AppEnvironment.CurrentPlayInstanceManager.PlayerName ?? ""))
        {
            return;
        }
        var saveResult = AppEnvironment.GameManager.SaveGame(AppEnvironment.CurrentPlayInstanceManager.Id);
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
