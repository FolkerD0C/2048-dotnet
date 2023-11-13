using System.IO;

namespace Game2048.Backend.Models;

internal interface ISaveMetaData
{
    string Name { get; }
    string Fullpath { get; }
}