namespace ECommons.Configuration;
public interface ISerializationFactory
{
    public string DefaultConfigFileName { get; }
    public T? Deserialize<T>(string inputData);
    public string? Serialize(object config, bool prettyPrint);
    public string? Serialize(object config);
    public T? Deserialize<T>(byte[] inputData);
    public byte[]? SerializeAsBin(object config);
    public bool IsBinary { get; }
}
