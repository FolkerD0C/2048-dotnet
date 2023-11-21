using ConsoleClient.Display;

namespace Game2048.ConsoleFrontend.Display;

public interface IPlayErrorMessageOverlay : IOverLay
{
    void PrintErrorMessage();
}