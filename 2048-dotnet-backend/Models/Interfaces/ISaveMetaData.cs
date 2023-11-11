using System.IO;

namespace Game2048.Backend.Models;

public interface ISaveMetaData
{
    public string Name { get; }
    public string Fullpath { get; }
}