using ConsoleClient.AppUI.Play;
using Game2048.Logic;
using Game2048.Shared.Enums;
using System;

namespace ConsoleClient.App.Resources;

internal static class PlayProvider
{
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
        AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
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
        AppEnvironment.GameLogic.Play(InputProvider.ProvidePlayInput, Pause);
    }

    // TODO
    static PauseResult Pause()
    {
        throw new NotImplementedException();
    }
}
