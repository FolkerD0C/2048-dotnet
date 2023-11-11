using System.IO;

namespace Game2048.Backend.Models;

public class SaveMetaData : ISaveMetaData
{
    readonly string name;
    public string Name => name;

    readonly string fullpath;
    public string Fullpath => fullpath;

    public SaveMetaData(FileInfo info)
    {
        name = info.Name.Replace(".save.json", "");
        fullpath = Path.Join(info.FullName);
    }
}