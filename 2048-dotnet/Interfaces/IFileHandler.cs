using Game2048.Classes;

namespace Game2048.Interfaces;

public interface IFileHandler
{
    string HighscoresPath { get; }

    IJSONHandler Converter { get; }

    IList<(string FileName, string FullPath)> GetAllSaveFiles();

    string GetSavedObject(string path);

    void SaveObject(string path, string jsonObject);
}
