namespace Game2048.Shared.Models;

public interface ISerializable
{
    string Serialize();

    void Deserialize(string deserializee);
}