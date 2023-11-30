namespace Game2048.Repository.Enums;

/// <summary>
/// An enum that represents the type of a repository event.
/// </summary>
public enum GameRepositoryEvent
{
    MaxNumberChanged,
    UndoCountChanged,
    MaxLivesChanged,
    Unknown
}