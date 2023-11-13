namespace Game2048.Backend.Helpers.Saving;

public abstract class FileHandler
{
    public readonly string saveFilePath;

    public string SaveFilePath { get { return saveFilePath; } }

    public FileHandler(string saveFilePath)
    {
        this.saveFilePath = saveFilePath;
    }

    protected abstract string Read();

    protected abstract void Write(string fileContent);
}