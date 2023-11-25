using Game2048.Config;
using Game2048.Logic.Models;
using Game2048.Repository;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game2048.Logic.Saving;

public class GameSaveHandler : FileHandler, IGameSaveHandler
{
    public GameSaveHandler(string saveFilePath, IGameRepository gameRepository) : base(saveFilePath)
    {
        this.gameRepository = gameRepository;
    }

    IGameRepository gameRepository;
    public IGameRepository GameRepository => gameRepository;

    public void Load()
    {
        string jsonRepository = Read();
        gameRepository = new GameRepository(false);
        gameRepository.Deserialize(jsonRepository);
    }

    public SaveResult Save()
    {
        SaveResult result = new SaveResult()
        {
            ResultType = SaveResultType.Unknown,
            Message = ""
        };
        try
        {
            var jsonRepository = gameRepository.Serialize();
            Write(jsonRepository);
            result.ResultType = SaveResultType.Success;
            result.Message = "Game successfully saved.";
        }
        catch (Exception exc)
        {
            result.ResultType = SaveResultType.Failure;
            result.Message = exc.Message;
        }
        return result;
    }

    protected override string Read()
    {
        var readData = File.ReadAllText(saveFilePath);
        return readData;
    }

    protected override void Write(string fileContent)
    {
        File.WriteAllText(saveFilePath, fileContent);
    }

    internal static IEnumerable<ISaveMetaData> GetSavedGames()
    {
        var saveFiles = new DirectoryInfo(GameData.SaveGameDirectoryPath).GetFiles("*.save.json");
        return saveFiles.Select(info => new SaveMetaData(info));
    }

    public static void CheckOrCreateSaveDirectory()
    {
        if (!Directory.Exists(GameData.SaveGameDirectoryPath))
        {
            Directory.CreateDirectory(GameData.SaveGameDirectoryPath);
        }
    }

    public static string GetFullPathFromName(string name)
    {
        return Path.Join(GameData.SaveGameDirectoryPath, name + ".save.json");
    }

    public void UpdateFilePath(string filePath)
    {
        saveFilePath = filePath;
    }
}