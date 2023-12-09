namespace Game2048.Managers.Enums;

/// <summary>
/// An enum that is used by <see cref="EventHandlers.MiscEventHappenedEventArgs"/> as a dispatcher.
/// </summary>
public enum MiscEvent
{
    GoalReached,
    MaxNumberChanged,
    UndoCountChanged,
    MaxLivesChanged,
    Unknown
}