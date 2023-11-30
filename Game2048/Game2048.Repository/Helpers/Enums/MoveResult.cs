namespace Game2048.Repository.Enums;

/// <summary>
/// An enum that represents the result of a movement on the grid.
/// </summary>
public enum MoveResult
{
    NoError,
    CannotMoveInthatDirection,
    NotGameEndingError,
    GameOverError,
    Unkown
}
