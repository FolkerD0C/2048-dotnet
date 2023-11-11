using System.ComponentModel.DataAnnotations.Schema;

namespace Game2048.Backend.Models;

public interface ISerializable
{
    string Serialize();

    void Deserialize(string deserializee);
}