using Game2048.Interfaces;

namespace Game2048.Classes;

class FileHandler : IFileHandler
{
    string gameDataDirectory;

    string savePrefix = ".save.json";

    List<string> fullSavePaths;

    string highscoresPath;
    public string HighscoresPath
    {
        get
        {
            return highscoresPath;
        }
    }

    public IJSONHandler Converter { get; }

    public FileHandler()
    {
        string envVar = "";
        if (OperatingSystem.IsWindows())
        {
            envVar = "USERPROFILE";
        }
        else if (OperatingSystem.IsLinux())
        {
            envVar = "HOME";
        }
        else
        {
            throw new SystemException("Not supported OS");
        }
        gameDataDirectory = Path.Combine(Environment.GetEnvironmentVariable(envVar), "Games", ".2048-dotnet");
        if (!Directory.Exists(gameDataDirectory))
        {
            Directory.CreateDirectory(gameDataDirectory);
        }
        Converter = new JSONHandler();
        highscoresPath = Path.Combine(gameDataDirectory, "highscores.json");
        if (!File.Exists(highscoresPath))
        {
            var highscores = new List<(string Name, int Score)>() {
                ("Alice", 150000), ("Bob", 130000),
                ("Clara", 110000), ("Daniel", 100000),
                ("Esther", 90000), ("Frank", 80000),
                ("Gina", 75000), ("Hank", 70000),
                ("Iolanda", 65000), ("Jack", 60000)
            };
            string jsonHighscores = Converter.SerializeHighScores(highscores);
            SaveObject(highscoresPath, jsonHighscores);
        }
    }

    public IList<(string FileName, string FullPath)> GetAllSaveFiles()
    {
        fullSavePaths = Directory.GetFiles(gameDataDirectory, "*" + savePrefix).ToList();
        var result = fullSavePaths.Select(
                path => (new string(path.SkipLast(savePrefix.Length)
                        .Skip(gameDataDirectory.Length + 1).ToArray()), path)
            ).ToList();
        return result;
    }

    public string GetSavedObject(string path)
    {
        string result = File.ReadAllText(path);
        return result;
    }

    public void SaveObject(string path, string jsonObject)
    {
        File.WriteAllText(path, jsonObject);
    }

    public string GetFullSavePath(string fileName)
    {
        if (!fileName.Contains(gameDataDirectory))
        {
            fileName = Path.Combine(gameDataDirectory, fileName);
        }
        if (!fileName.Contains(savePrefix))
        {
            fileName = fileName + savePrefix;
        }
        return fileName;
    }

    public bool SaveExists(string fileName)
    {
        return fullSavePaths.Contains(GetFullSavePath(fileName));
    }
}
