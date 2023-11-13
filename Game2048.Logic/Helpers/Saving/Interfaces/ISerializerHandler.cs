using Game2048.Shared.Models;

namespace Game2048.Logic.Saving;

public interface ISerializerHandler<T> where T : ISerializable
{
    string Convert(T convertee);

    T Deconvert(string deconvertee);
}