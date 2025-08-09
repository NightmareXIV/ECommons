namespace ECommons.Configuration;
public interface ISerializationFactory
{
    string DefaultConfigFileName { get; }
    T? Deserialize<T>(string inputData);
    string? Serialize(object config, bool prettyPrint);
    string? Serialize(object config);
    T? Deserialize<T>(byte[] inputData);
    byte[]? SerializeAsBin(object config);
    bool IsBinary { get; }

    void WriteFile(string fullPath, string data);
    void WriteFile(string fullPath, byte[] data);
    string ReadFileAsText(string fullPath);
    byte[] ReadFileAsBin(string fullPath);
    bool FileExists(string fullPath);
}
