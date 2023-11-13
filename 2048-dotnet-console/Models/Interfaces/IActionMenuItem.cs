namespace Game2048.ConsoleFrontend.Models;

public interface IActionMenuItem
{
    void PerformAction(IMenuActionRequestedArgs args);
}