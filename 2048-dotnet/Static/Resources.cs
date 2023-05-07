using Game2048.Classes;
using Game2048.Classes.Menus;
using Game2048.Interfaces;

namespace Game2048.Static;

public static class Resources
{
    public static IFileHandler IOHandler { get; set; }

    public static MainMenu EntryPoint { get; set; }

    static Resources()
    {
        IOHandler = new FileHandler();
    }

    static void ConstructMenu()
    {

    }

    public static void Initialize()
    {

    }
}
