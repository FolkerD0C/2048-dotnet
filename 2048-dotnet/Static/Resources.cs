using Game2048.Classes;
using Game2048.Classes.Menus;
using Game2048.Interfaces;

namespace Game2048.Static;

public static class Resources
{
    static IFileHandler iOHandler = new FileHandler();

    public static IFileHandler IOHandler { get { return iOHandler; } set { iOHandler = value; } }

    static NavigationMenu entryPoint;

    public static NavigationMenu EntryPoint { get { return entryPoint; } set { entryPoint = value; } }

    static string[] gameDescription = { "dummy" };

    static NavigationMenu ConstructMainMenu()
    {
        List<IMenu> subMenus = new List<IMenu>();

        subMenus.Add(new ObjectMenu("New game", Play.Initialize, IOHandler));

        var allSaves = IOHandler.GetAllSaveFiles();
        List<IMenu> loadGameSubMenus = allSaves.Select(tup =>
            (IMenu)(new ObjectMenu(tup.FileName, Play.Initialize, IOHandler, tup.FullPath)
            )).ToList();
        loadGameSubMenus.Add(new ReturnMenu(MenuResult.Back));
        var loadGameMenu = new NavigationMenu("Load game", loadGameSubMenus);
        loadGameMenu.SetReturnValue(MenuResult.Obj);
        subMenus.Add(loadGameMenu);

        var jsonHighScores = IOHandler.GetSavedObject(IOHandler.HighscoresPath);
        var highScores = IOHandler.Converter.DeserializeHighScores(jsonHighScores)
            .Select(tup => tup.Name + $" {tup.Score}").ToArray();
        subMenus.Add(new DisplayMenu("Highscores", highScores));

        subMenus.Add(new DisplayMenu("Description", gameDescription));

        subMenus.Add(new ActionMenu("Exit", GracefulExit));

        return new NavigationMenu("mainMenu", subMenus);
    }

    public static void Run()
    {
        Console.CursorVisible = false;
        while (true)
        {
            var main = ConstructMainMenu();
            main.MenuAction();
        }
    }

    public static void GracefulExit()
    {
        Console.Clear();
        Console.CursorVisible = true;
        Environment.Exit(0);
    }
}
