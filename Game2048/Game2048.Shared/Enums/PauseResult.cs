namespace Game2048.Shared.Enums;

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