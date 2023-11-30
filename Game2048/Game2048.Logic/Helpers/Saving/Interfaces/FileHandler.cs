namespace Game2048.Logic.Saving;

/// <summary>
/// An abstract class that is used for reading and writing files.
/// </summary>
public abstract class FileHandler
{
    /// <summary>
    /// The full path of the file to read and write.
    /// </summary>
    protected string saveFilePath;

    /// <summary>
    /// Abstract constructor that creates a new object derived from the <see cref="FileHandler"/> class.
    /// </summary>
    /// <param name="saveFilePath"></param>
    public FileHandler(string saveFilePath)
    {
        this.saveFilePath = saveFilePath;
    }

    /// <summary>
    /// Reads the file specified in <see cref="saveFilePath"/>.
    /// </summary>
    /// <returns>The content of <see cref="saveFilePath"/>.</returns>
    protected abstract string Read();

    /// <summary>
    /// Writes the file specified in <see cref="saveFilePath"/>.
    /// </summary>
    /// <param name="fileContent">The new content of <see cref="saveFilePath"/>.</param>
    protected abstract void Write(string fileContent);
}