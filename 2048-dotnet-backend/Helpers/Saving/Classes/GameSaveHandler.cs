using System.IO;
using System.Collections.Generic;
using Game2048.Backend.Models;
using Game2048.Backend.Repository;
using System.Linq;
using System;
using System.Text.Json;

namespace Game2048.Backend.Helpers.Saving;

public class GameSaveHandler : FileHandler, IGameSaveHandler
{
    public GameSaveHandler(string saveFilePath) : base(saveFilePath)
    {
        gameRepository = new GameRepository();
    }

    IGameRepository gameRepository;
    public IGameRepository GameRepository => gameRepository;

    public string Convert(IGamePosition convertee)
    {
        return convertee.Serialize();
    }

    public IGamePosition Deconvert(string deconvertee)
    {
        IGamePosition gamePosition = new GamePosition();
        gamePosition.Deserialize(deconvertee);
        return gamePosition;
    }

    public void Load()
    {
        string jsonRepository = Read();
        using var jsonDoc = JsonDocument.Parse(jsonRepository);
        var jsonRoot = jsonDoc.RootElement;
        int remainingLives = jsonRoot.GetProperty("remainingLives").GetInt32();
        int gridWidth = jsonRoot.GetProperty("gridWidth").GetInt32();
        int gridHeight = jsonRoot.GetProperty("gridHeight").GetInt32();
        string playerName = (jsonRoot.GetProperty("playerName").GetString()
            ?? throw new ArgumentNullException(nameof(playerName), "Property can not be null")).Replace("\\\"", "\"");
        int goal = jsonRoot.GetProperty("goal").GetInt32();
        IList<int> acceptedSpawnables = new List<int>();
        var acceptedEnumerable = jsonRoot.GetProperty("acceptedSpawnables").EnumerateArray();
        foreach (var accepted in acceptedEnumerable)
        {
            acceptedSpawnables.Add(accepted.GetInt32());
        }
        IList<IGamePosition> undoChain = new List<IGamePosition>();
        var chainEnumerable = jsonRoot.GetProperty("undoChain").EnumerateArray();
        foreach (var chainElement in chainEnumerable)
        {
            undoChain.Add(Deconvert(chainElement.GetRawText()));
        }
        gameRepository = Repository.GameRepository.GetRepositoryFromSave(
            remainingLives, gridWidth, gridHeight, playerName, goal, acceptedSpawnables, undoChain
        );
    }

    public void Save()
    {
        var jsonRepository = "{";
        jsonRepository += $"\"remainingLives\":{gameRepository.RemainingLives},";
        jsonRepository += $"\"gridWidth\":{gameRepository.GridWidth},";
        jsonRepository += $"\"gridHeight\":{gameRepository.GridHeight},";
        jsonRepository += "\"playerName\":" + gameRepository.PlayerName.Replace("\"", "\\\"");
        jsonRepository += "\"acceptedSpawnables\":[" + string.Join(",", gameRepository.AcceptedSpawnables
            ?? throw new NullReferenceException("Accepted spawnables can not be null.")) + "]";
        jsonRepository += "\"undoChain\":[" + string.Join(",",
            gameRepository.UndoChain.Select(position => Convert(position))) + "]";
        jsonRepository += "}";
        Write(jsonRepository);
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

    public static IEnumerable<ISaveMetaData> GetSavedGames()
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
}