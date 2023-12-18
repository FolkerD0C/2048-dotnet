using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Managers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        AppEnvironment.CurrentPlayInstance = AppEnvironment.GameManager.NewGame(AppEnvironment.GameConfiguration);
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        AppEnvironment.CurrentPlayInstance.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstance.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstance.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstance.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstance.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstance.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstance.InputProcessingFinished += currentPlayInstanceOverlay.OnInputProcessingFinished;
        AppEnvironment.CurrentPlayInstance.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        PlayEndedReason endedReason = AppEnvironment.GameManager.Play(AppEnvironment.CurrentPlayInstance.Id, ConvertInput).GetAwaiter().GetResult();
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
        if (AppEnvironment.GameManager is null)
        {
            throw new NullReferenceException("GameManager is null");
        }
        AppEnvironment.CurrentPlayInstance = AppEnvironment.GameManager.LoadGame(saveGameName);
        IGameDisplay currentPlayInstanceOverlay = new GameDisplay();
        AppEnvironment.CurrentOverlays.Add("currentPlayInstanceOverlay", currentPlayInstanceOverlay);
        currentPlayInstanceOverlay.SetPreviousOverlaySuppression(true);
        AppEnvironment.CurrentPlayInstance.PlayStarted += currentPlayInstanceOverlay.OnPlayStarted;
        AppEnvironment.CurrentPlayInstance.MoveHappened += currentPlayInstanceOverlay.OnMoveHappened;
        AppEnvironment.CurrentPlayInstance.UndoHappened += currentPlayInstanceOverlay.OnUndoHappened;
        AppEnvironment.CurrentPlayInstance.ErrorHappened += currentPlayInstanceOverlay.OnErrorHappened;
        AppEnvironment.CurrentPlayInstance.MiscEventHappened += currentPlayInstanceOverlay.MiscEventHappenedDispatcher;
        AppEnvironment.CurrentPlayInstance.PlayerNameChanged += currentPlayInstanceOverlay.OnPlayerNameChanged;
        AppEnvironment.CurrentPlayInstance.InputProcessingFinished += currentPlayInstanceOverlay.OnInputProcessingFinished;
        AppEnvironment.CurrentPlayInstance.PlayEnded += currentPlayInstanceOverlay.OnPlayEnded;
        AppEnvironment.CurrentOverlays["currentPlayInstanceOverlay"].SetPreviousOverlaySuppression(true);
        PlayEndedReason endedReason = AppEnvironment.GameManager.Play(AppEnvironment.CurrentPlayInstance.Id, ConvertInput).GetAwaiter().GetResult();
        HandlePlayEnded(endedReason);
        AppEnvironment.CurrentMenus["loadGameMenu"].EndNavigation();
        AppEnvironment.CurrentPlayInstance = null;
        AppEnvironment.CurrentOverlays.Remove("currentPlayInstanceOverlay");
    }

    /// <summary>
    /// Converts <see cref="ExtendedGameInput"/> into <see cref="GameInput"/>, which then can be provided to an ongoing play.
    /// </summary>
    /// <returns>A valid or unknown play input.</returns>
    async static Task<GameInput> ConvertInput()
    {
        ExtendedGameInput input = default;
        await Task.Run(() => input = InputProvider.ProvidePlayInput());
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
        if (AppEnvironment.GameManager is null || AppEnvironment.CurrentPlayInstance is null)
        {
            throw new NullReferenceException("Managers can not be null.");
        }
        if (endedReason != PlayEndedReason.GameOver)
        {
            return;
        }
        int lowestHighscore = AppEnvironment.GameManager.GetHighscores()
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
            AppEnvironment.CurrentPlayInstance.PlayerName = nameFormResult.Name;
        }
        AppEnvironment.GameManager.AddHighscore(AppEnvironment.CurrentPlayInstance.PlayerName, AppEnvironment.CurrentPlayInstance.PlayerScore);
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
            throw new NullReferenceException("Play instance can not be null.");
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
        if (AppEnvironment.GameManager is null || AppEnvironment.CurrentPlayInstance is null)
        {
            throw new NullReferenceException("Managers can not be null.");
        }
        if (AppEnvironment.CurrentPlayInstance.PlayerName is null || AppEnvironment.CurrentPlayInstance.PlayerName == "")
        {
            ProvideChangePlayerNameAction();
        }
        if (AppEnvironment.GameManager.GetSavedGames().Contains(AppEnvironment.CurrentPlayInstance.PlayerName)
            && !PromptPlayerOverwritingSave(AppEnvironment.CurrentPlayInstance.PlayerName ?? ""))
        {
            return;
        }
        var saveResult = AppEnvironment.GameManager.SaveGame(AppEnvironment.CurrentPlayInstance.Id);
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
