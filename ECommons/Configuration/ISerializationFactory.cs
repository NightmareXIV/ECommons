using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Configuration;
public interface ISerializationFactory
{
    public T? Deserialize<T>(string inputData);
    public string? Serialize(object config, bool prettyPrint);
}
