namespace Game2048.ConsoleFrontend;

class App
{
    void Main(string[] args)
    {
        var logic = new GameLogic();
        logic.NewGame();
    }
}