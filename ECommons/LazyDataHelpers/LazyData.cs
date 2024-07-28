using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.LazyDataHelpers;
public static class LazyData<T>
{
    private static readonly string RandomGuid = Guid.NewGuid().ToString();
    static LazyData()
    {
        Purgatory.Add(() => Cache = null!);
    }

    private static Dictionary<string, T> Cache = [];
    public static T Get(string tag, Func<T> generator)
    {
        if(Cache.TryGetValue(tag, out var ret) == true)
        {
            return ret;
        }
        else
        {
            ret = generator();
            Cache.Add(tag, ret);
            return ret;
        }
    }

    public static T Get(Func<T> generator) => Get(RandomGuid, generator);
}
