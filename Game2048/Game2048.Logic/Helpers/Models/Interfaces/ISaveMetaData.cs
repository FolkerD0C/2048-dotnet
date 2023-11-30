namespace Game2048.Logic.Models;

/// <summary>
/// Represents two properties used for game saving.
/// </summary>
internal interface ISaveMetaData
{
    /// <summary>
    /// The name of the save.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The full path of the save file.
    /// </summary>
    string Fullpath { get; }
}