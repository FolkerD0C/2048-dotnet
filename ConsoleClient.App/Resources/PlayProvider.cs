using ConsoleClient.AppUI.Play;
using ConsoleClient.Menu;
using Game2048.Logic;
using Game2048.Shared.Enums;
using System;

namespace ConsoleClient.App.Resources;

internal static class PlayProvider
{
    static IConsoleMenu? mainMenu;

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
        IPlayLogic playLogic = AppEnvironment.GameLogic.NewGame();
        IGameDisplay gameDisplay = new GameDisplay();
        playLogic.PlayStarted += gameDisplay.OnPlayStarted;
        playLogic.MoveHappened += gameDisplay.OnMoveHappened;
        playLogic.UndoHappened += gameDisplay.OnUndoHappened;
        playLogic.ErrorHappened += gameDisplay.OnErrorHappened;
        playLogic.MiscEventHappened += gameDisplay.MiscEventHappenedDispatcher;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
        if (endedReason == PlayEndedReason.QuitGame)
        {
            mainMenu?.EndNavigation();
        }
    }

    internal static void ProvideLoadedGame(string saveGameName)
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException("GameLogic is null");
        }
        IPlayLogic playLogic = AppEnvironment.GameLogic.LoadGame(saveGameName);
        IGameDisplay gameDisplay = new GameDisplay();
        playLogic.PlayStarted += gameDisplay.OnPlayStarted;
        playLogic.MoveHappened += gameDisplay.OnMoveHappened;
        playLogic.UndoHappened += gameDisplay.OnUndoHappened;
        playLogic.ErrorHappened += gameDisplay.OnErrorHappened;
        playLogic.MiscEventHappened += gameDisplay.MiscEventHappenedDispatcher;
        PlayEndedReason endedReason = AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
        if (endedReason == PlayEndedReason.QuitGame)
        {
            mainMenu?.EndNavigation();
        }
    }

    static PauseResult Pause()
    {
        return PauseMenuProvider.ProvidePauseMenuAction(ProvideSaveGameAction);
    }

    // TODO needs NameForm
    static void ProvideSaveGameAction()
    {
        throw new NotImplementedException();
    }
}
