namespace Game2048.Shared.Enums;

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