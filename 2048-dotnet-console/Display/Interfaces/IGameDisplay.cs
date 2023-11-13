namespace Game2048.ConsoleFrontend.Display;

public interface IGameDisplay : IOverLay
{
    public void Initialize(IGameRepository gameRepository);
    public void OnMoveHappened(object? sender, MoveHappenedEventArgs args);
    public void OnUndoHappened(object? sender, UndoHappenedEventArgs args);
    public void OnErrorHappened(object? sender, ErrorHappenedEventArgs args);
    public void MiscEventHappenedDispatcher(object? sender, MiscEventHappenedEventArgs args);
}