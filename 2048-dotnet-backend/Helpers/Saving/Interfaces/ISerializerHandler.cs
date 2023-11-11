using Game2048.Backend.Models;

namespace Game2048.Backend.Helpers.Saving;

public interface ISerializerHandler<T> where T : ISerializable
{
    string Convert(T convertee);

    T Deconvert(string deconvertee);
}