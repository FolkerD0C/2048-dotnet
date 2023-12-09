namespace Game2048.Logic.Enums;

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