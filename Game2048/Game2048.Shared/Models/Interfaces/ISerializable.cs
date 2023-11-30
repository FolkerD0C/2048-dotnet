namespace Game2048.Shared.Models;

/// <summary>
/// Defines a serializable object, used for saving and loading.
/// </summary>
public interface ISerializable
{
    /// <summary>
    /// Serializes an object that implements the <see cref="ISerializable"/> interface.
    /// </summary>
    /// <returns>The string representation of a serialized object.</returns>
    string Serialize();

    /// <summary>
    /// Deserializes an object that implements the <see cref="ISerializable"/> interface.
    /// </summary>
    /// <param name="deserializee">The string representation of a serialized object.</param>
    void Deserialize(string deserializee); // TODO make it return 'T'
}