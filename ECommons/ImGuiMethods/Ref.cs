using ECommons.LazyDataHelpers;
using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods;
public static class Ref<T>
{
    static Ref()
    {
        Purgatory.Add(typeof(Ref<T>));
    }

    private static Dictionary<string, Box<T>> Storage = [];

    public static ref T? Get() => ref Get(GenericHelpers.GetCallStackID(), default(T));

    public static ref T? Get(T? defaultValue) => ref Get(GenericHelpers.GetCallStackID(), defaultValue);

    public static ref T? Get(Func<T?>? defaultValueGenerator) => ref Get(GenericHelpers.GetCallStackID(), defaultValueGenerator);

    public static ref T? Get(string key) => ref Get(key, default(T));

    public static ref T? Get(string key, T? defaultValue)
    {
        if (Storage.TryGetValue(key, out var ret))
        {
            return ref ret.Value;
        }
        else
        {
            Storage[key] = new(defaultValue);
            if (defaultValue == null && typeof(T) == typeof(string))
            {
                Storage[key].SetFoP("Value", string.Empty);
            }
            return ref Storage[key].Value;
        }
    }
    
    public static ref T? Get(string s, Func<T?>? defaultValueGenerator)
    {
        if (Storage.TryGetValue(s, out var ret))
        {
            return ref ret.Value;
        }
        else
        {
            Storage[s] = new(defaultValueGenerator == null?default:defaultValueGenerator.Invoke());
            if (defaultValueGenerator == null && typeof(T) == typeof(string))
            {
                Storage[s].SetFoP("Value", string.Empty);
            }
            return ref Storage[s].Value;
        }
    }
}
