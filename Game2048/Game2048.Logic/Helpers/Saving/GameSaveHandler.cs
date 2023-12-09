using Game2048.Config;
using Game2048.Repository;
using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Game2048.Logic.Saving;

/// <summary>
/// A class that represents a manager for game saving and loading.
/// </summary>
internal static class GameSaveHandler
{
    internal static IGameRepository Load(string saveGameName)
    {
        string saveFilePath = GetFullPathFromName(saveGameName);
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        string jsonRepository = File.ReadAllText(saveFilePath);
        var deserializedData = JsonSerializer.Deserialize<GameSaveData>(jsonRepository, serializerOptions) ?? throw new NullReferenceException("Failed to load game.");
        return new GameRepository(deserializedData);
    }

    internal static SaveResult Save(IGameRepository playProcessor)
    {
        SaveResult result = new()
        {
            ResultType = SaveResultType.Unknown,
            Message = string.Empty
        };
        try
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var jsonSaveData = JsonSerializer.Serialize(playProcessor.GetSaveDataObject(), serializerOptions);
            File.WriteAllText(GetFullPathFromName(playProcessor.PlayerName), jsonSaveData);
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

    /// <summary>
    /// Gets all saved games that are present in <see cref="GameData.SaveGameDirectoryPath"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{ISaveMetaData}"/> that contains all saved
    /// games contained in <see cref="GameData.SaveGameDirectoryPath"/>.</returns>
    internal static IEnumerable<string> GetSavedGames()
    {
        var saveFiles = new DirectoryInfo(GameData.SaveGameDirectoryPath).GetFiles("*.save.json");
        return saveFiles.Select(info => info.Name.Replace(".save.json", ""));
    }

    /// <summary>
    /// Checks if the <see cref="GameData.SaveGameDirectoryPath"/> directory exists and creates it if not.
    /// </summary>
    internal static void CheckOrCreateSaveDirectory()
    {
        if (!Directory.Exists(GameData.SaveGameDirectoryPath))
        {
            Directory.CreateDirectory(GameData.SaveGameDirectoryPath);
        }
    }

    /// <summary>
    /// Gets the full path of a save file (<see cref="GameData.SaveGameDirectoryPath"/>/<paramref name="name"/>.save.json) from the saved game name.
    /// </summary>
    /// <param name="name">The name of the saved game.</param>
    /// <returns>The full path of the saved game name.</returns>
    internal static string GetFullPathFromName(string name)
    {
        return Path.Join(GameData.SaveGameDirectoryPath, name + ".save.json");
    }
}