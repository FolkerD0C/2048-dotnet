namespace Game2048.Managers.Enums;

/// <summary>
/// An enum that tells the backend about the result of the pause method during a play.
/// </summary>
public enum PauseResult
{
    Continue,
    EndPlay,
    ExitGame,
    Unknown
}