using ConsoleClient.Display;

namespace Game2048.ConsoleFrontend.Display;

// TODO do I need this interface
public interface IPlayErrorMessageOverlay : IOverLay
{
    void PrintErrorMessage();
}