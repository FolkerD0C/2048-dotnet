using ConsoleClient.App.Resources;

namespace ConsoleClient.App;

class Run
{
    static void Main(string[] args)
    {
        AppEnvironment.Initialize();
        MainMenuProvider.ProvideMainMenuAction();
    }
}