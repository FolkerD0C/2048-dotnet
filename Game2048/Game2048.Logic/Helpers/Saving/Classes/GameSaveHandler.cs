using Game2048.Config;
using Game2048.Logic.Models;
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
internal class GameSaveHandler : FileHandler, IGameSaveHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="GameSaveHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath">The full path for the game save file.</param>
    /// <param name="gameRepository">The value for <see cref="GameRepository"/>.</param>
    public GameSaveHandler(string saveFilePath, IGameRepository? gameRepository) : base(saveFilePath)
    {
        this.gameRepository = gameRepository;
    }

    IGameRepository? gameRepository;
    public IGameRepository? GameRepository => gameRepository;

    public void Load()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        string jsonRepository = Read();
        var deserializedData = JsonSerializer.Deserialize<GameSaveData>(jsonRepository, serializerOptions) ?? throw new NullReferenceException("Failed to load game.");
        gameRepository = new GameRepository(deserializedData);
    }

    public SaveResult Save()
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
            var jsonRepository = JsonSerializer.Serialize(gameRepository?.GetSaveDataObject(), serializerOptions);
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

    /// <summary>
    /// Gets all saved games that are present in <see cref="GameData.SaveGameDirectoryPath"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{ISaveMetaData}"/> that contains all saved
    /// games contained in <see cref="GameData.SaveGameDirectoryPath"/>.</returns>
    internal static IEnumerable<ISaveMetaData> GetSavedGames() // TODO create a new static class that does this and the 2 others below
    {
        var saveFiles = new DirectoryInfo(GameData.SaveGameDirectoryPath).GetFiles("*.save.json");
        return saveFiles.Select(info => new SaveMetaData(info));
    }

    /// <summary>
    /// Checks if the <see cref="GameData.SaveGameDirectoryPath"/> directory exists and creates it if not.
    /// </summary>
    public static void CheckOrCreateSaveDirectory()
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
    public static string GetFullPathFromName(string name)
    {
        return Path.Join(GameData.SaveGameDirectoryPath, name + ".save.json");
    }

    public void UpdateFilePath(string filePath)
    {
        saveFilePath = filePath;
    }
}