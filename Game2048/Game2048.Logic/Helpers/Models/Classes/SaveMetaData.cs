using System.IO;

namespace Game2048.Logic.Models;

/// <summary>
/// A class that is used for saving and loading a game.
/// </summary>
internal class SaveMetaData : ISaveMetaData
{
    readonly string name;
    public string Name => name;

    readonly string fullpath;
    public string Fullpath => fullpath;

    /// <summary>
    /// Creates a new instance of the <see cref="SaveMetaData"/> class.
    /// </summary>
    /// <param name="info">A <see cref="FileInfo"/> object that contains a serialized game save.</param>
    public SaveMetaData(FileInfo info)
    {
        name = info.Name.Replace(".save.json", "");
        fullpath = Path.Join(info.FullName);
    }
}