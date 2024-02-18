namespace ECommons.Configuration;
public interface ISerializationFactory
{
    public string DefaultConfigFileName { get; }
    public T? Deserialize<T>(string inputData);
    public string? Serialize(object config, bool prettyPrint);
}
