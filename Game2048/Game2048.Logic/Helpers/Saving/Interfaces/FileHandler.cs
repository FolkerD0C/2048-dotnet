using System.IO;

namespace Game2048.Logic.Saving;

/// <summary>
/// An abstract class that is used for reading and writing files.
/// </summary>
internal class FileHandler
{
    /// <summary>
    /// The full path of the file to read and write.
    /// </summary>
    protected string saveFilePath;

    /// <summary>
    /// Creates a new instance of the <see cref="FileHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath">The full path of the file to read and write.</param>
    public FileHandler(string saveFilePath)
    {
        this.saveFilePath = saveFilePath;
    }

    /// <summary>
    /// Reads the file specified in <see cref="saveFilePath"/>.
    /// </summary>
    /// <returns>The content of <see cref="saveFilePath"/>.</returns>
    protected string Read()
    {
        var readData = File.ReadAllText(saveFilePath);
        return readData;
    }

    /// <summary>
    /// Writes the file specified in <see cref="saveFilePath"/>.
    /// </summary>
    /// <param name="fileContent">The new content of <see cref="saveFilePath"/>.</param>
    protected void Write(string fileContent)
    {
        File.WriteAllText(saveFilePath, fileContent);
    }
}