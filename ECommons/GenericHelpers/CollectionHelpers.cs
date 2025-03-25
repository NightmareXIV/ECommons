using ECommons.MathHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    public static T? FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) where T : struct
    {
        if(values.TryGetFirst(predicate, out var result))
        {
            return result;
        }
        return null;
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> values) where T : struct
    {
        if(values.TryGetFirst(out var result))
        {
            return result;
        }
        return null;
    }

    public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> values) where T : struct
        => values.Cast<T?>();

    public static bool ContainsNullable<T>(this IEnumerable<T> values, T? value) where T : struct
    {
        if(value == null) return false;
        return Enumerable.Contains(values, value.Value);
    }

    /// <summary>
    /// Adds all <paramref name="values"/> to the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="values"></param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        foreach(var x in values)
        {
            collection.Add(x);
        }
    }

    /// <summary>
    /// Returns random element from <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this IEnumerable<T> enumerable)
        => enumerable.ElementAt(Random.Shared.Next(enumerable.Count()));

    /// <inheritdoc cref="SafeSelect{K, V}(IReadOnlyDictionary{K, V}, K, V)"/>
    public static V? SafeSelect<K, V>(this IReadOnlyDictionary<K, V> dictionary, K? key) => SafeSelect(dictionary, key, default);

    /// <summary>
    /// Safely selects a value from a <paramref name="dictionary"/>. Does not throws exceptions under any circumstances.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue">Returns if <paramref name="dictionary"/> is <see langword="null"/> or <paramref name="key"/> is <see langword="null"/> or <paramref name="key"/> is not found in <paramref name="dictionary"/></param>
    /// <returns></returns>
    public static V? SafeSelect<K, V>(this IReadOnlyDictionary<K, V> dictionary, K key, V defaultValue)
    {
        if(dictionary == null) return default;
        if(key == null) return default;
        if(dictionary.TryGetValue(key, out var ret))
        {
            return ret;
        }
        return defaultValue;
    }

    public static T CircularSelect<T>(this IList<T> list, int index) => list[MathHelper.Mod(index, list.Count)];

    public static T CircularSelect<T>(this T[] list, int index) => list[MathHelper.Mod(index, list.Length)];

    /// <summary>
    /// Safely selects an entry of the <paramref name="list"/> at a specified <paramref name="index"/>, returning default value if index is out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T SafeSelect<T>(this IReadOnlyList<T> list, int index)
    {
        if(list == null) return default;
        if(index < 0 || index >= list.Count) return default;
        return list[index];
    }

    /// <summary>
    /// Safely selects an entry of the <paramref name="array"/> at a specified <paramref name="index"/>, returning <see langword="default"/> value if index is out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T SafeSelect<T>(this T[] array, int index)
    {
        if(index < 0 || index >= array.Length) return default;
        return array[index];
    }

    [Obsolete($"Use {nameof(SafeSelect)}")]
    public static T GetOrDefault<T>(this IReadOnlyList<T> List, int index) => SafeSelect(List, index);

    [Obsolete($"Use {nameof(SafeSelect)}")]
    public static T GetOrDefault<T>(this T[] Array, int index) => SafeSelect(Array, index);

    /// <summary>
    /// Treats list as a queue, removing and returning element at index 0.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryDequeue<T>(this IList<T> List, out T result)
    {
        if(List.Count > 0)
        {
            result = List[0];
            List.RemoveAt(0);
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Treats list as a queue, removing and returning element at index 0.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Dequeue<T>(this IList<T> List)
    {
        if(List.TryDequeue(out var ret))
        {
            return ret;
        }
        throw new InvalidOperationException("Sequence contains no elements");
    }

    public static bool TryDequeueLast<T>(this IList<T> List, out T result)
    {
        if(List.Count > 0)
        {
            result = List[List.Count - 1];
            List.RemoveAt(List.Count - 1);
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static T DequeueLast<T>(this IList<T> List)
    {
        if(List.TryDequeueLast(out var ret))
        {
            return ret;
        }
        throw new InvalidOperationException("Sequence contains no elements");
    }

    /// <summary>
    /// Treats list as a queue, removing and returning element at index 0 or default value if there's nothing to dequeue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <returns></returns>
    public static T DequeueOrDefault<T>(this IList<T> List)
    {
        if(List.Count > 0)
        {
            var ret = List[0];
            List.RemoveAt(0);
            return ret;
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Dequeues element from queue or returns default value if there's nothing to dequeue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Queue"></param>
    /// <returns></returns>
    public static T DequeueOrDefault<T>(this Queue<T> Queue)
    {
        if(Queue.Count > 0)
        {
            return Queue.Dequeue();
        }
        return default;
    }

    /// <summary>
    /// Searches index of first element in IEnumerable that matches the predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this IEnumerable<T> values, Predicate<T> predicate)
    {
        var ret = -1;
        foreach(var v in values)
        {
            ret++;
            if(predicate(v))
            {
                return ret;
            }
        }
        return -1;
    }

    /// <summary>
    /// Searches index of first element in IEnumerable that matches the predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this IEnumerable<T> values, T value)
    {
        var ret = -1;
        foreach(var v in values)
        {
            ret++;
            if(v.Equals(value))
            {
                return ret;
            }
        }
        return -1;
    }

    public static bool ContainsIgnoreCase(this IEnumerable<string> haystack, string needle)
    {
        foreach(var x in haystack)
        {
            if(x.EqualsIgnoreCase(needle)) return true;
        }
        return false;
    }

    public static T[] Together<T>(this T[] array, params T[] additionalValues)
        => array.Union(additionalValues).ToArray();

    /// <summary>
    /// Tries to add multiple items to collection
    /// </summary>
    /// <typeparam name="T">Collection type</typeparam>
    /// <param name="collection">Collection</param>
    /// <param name="values">Items</param>
    public static void Add<T>(this ICollection<T> collection, params T[] values)
    {
        foreach(var x in values)
        {
            collection.Add(x);
        }
    }

    /// <summary>
    /// Tries to remove multiple items to collection. In case if few of the same values are present in the collection, only first will be removed.
    /// </summary>
    /// <typeparam name="T">Collection type</typeparam>
    /// <param name="collection">Collection</param>
    /// <param name="values">Items</param>
    public static void Remove<T>(this ICollection<T> collection, params T[] values)
    {
        foreach(var x in values)
        {
            collection.Remove(x);
        }
    }

    public static V GetOrDefault<K, V>(this IDictionary<K, V> dic, K key)
    {
        if(dic.TryGetValue(key, out var value)) return value;
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IncrementOrSet<K>(this IDictionary<K, int> dic, K key, int increment = 1)
    {
        if(dic.ContainsKey(key))
        {
            dic[key] += increment;
        }
        else
        {
            dic[key] = increment;
        }
        return dic[key];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Print<T>(this IEnumerable<T> x, string separator = ", ")
    {
        return x.Select(x => (x?.ToString() ?? "")).Join(separator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V GetSafe<K, V>(this IDictionary<K, V> dic, K key, V Default = default)
    {
        if(dic?.TryGetValue(key, out var value) == true)
        {
            return value;
        }
        return Default;
    }

    /// <summary>
    /// Retrieves a value from dictionary, adding it first if it doesn't exists yet.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V GetOrCreate<K, V>(this IDictionary<K, V> dictionary, K key)
    {
        if(dictionary.TryGetValue(key, out var result))
        {
            return result;
        }
        V newValue;
        if(typeof(V).FullName == typeof(string).FullName)
        {
            newValue = (V)(object)"";
        }
        else
        {
            try
            {
                newValue = (V)Activator.CreateInstance(typeof(V));
            }
            catch(Exception)
            {
                newValue = default;
            }
        }
        dictionary.Add(key, newValue);
        return newValue;
    }

    /// <summary>
    /// Executes action for each element of collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="function"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<T>(this IEnumerable<T> collection, Action<T> function)
    {
        foreach(var x in collection)
        {
            function(x);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> src) => src.Select((x, i) => (x, i));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> function)
    {
        foreach(var (x, i) in collection.WithIndex())
        {
            function(x, i);
        }
    }

    /// <summary>
    /// Adds <paramref name="value"/> into HashSet if it doesn't exists yet or removes if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hashSet"></param>
    /// <param name="value"></param>
    /// <returns>Whether <paramref name="hashSet"/> contains <paramref name="value"/> after function has been executed.</returns>
    public static bool Toggle<T>(this HashSet<T> hashSet, T value)
    {
        if(hashSet.Contains(value))
        {
            hashSet.Remove(value);
            return false;
        }
        else
        {
            hashSet.Add(value);
            return true;
        }
    }

    public static bool Toggle<T>(this List<T> list, T value)
    {
        if(list.Contains(value))
        {
            list.RemoveAll(x => x.Equals(value));
            return false;
        }
        else
        {
            list.Add(value);
            return true;
        }
    }

    public static T FirstOr0<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach(var x in collection)
        {
            if(predicate(x))
            {
                return x;
            }
        }
        return collection.First();
    }

    public static IEnumerable<R> SelectMulti<T, R>(this IEnumerable<T> values, params Func<T, R>[] funcs)
    {
        foreach(var v in values)
            foreach(var x in funcs)
            {
                yield return x(v);
            }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
    {
        foreach(var x in values)
        {
            if(!source.Contains(x)) return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this IEnumerable<string> e, string separator)
    {
        return string.Join(separator, e);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(this IEnumerable<T> obj, params T[] values)
    {
        foreach(var x in values)
        {
            if(obj.Contains(x))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(this IEnumerable<T> obj, IEnumerable<T> values)
    {
        foreach(var x in values)
        {
            if(obj.Contains(x))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllNull(params object[] objects) => objects.All(s => s == null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyNull(params object[] objects) => objects.Any(s => s == null);

    public static IEnumerable<K> FindKeysByValue<K, V>(this IDictionary<K, V> dictionary, V value)
    {
        foreach(var x in dictionary)
        {
            if(value.Equals(x.Value))
            {
                yield return x.Key;
            }
        }
    }

    public static bool TryGetFirst<K, V>(this IDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate, out KeyValuePair<K, V> keyValuePair)
    {
        try
        {
            keyValuePair = dictionary.First(predicate);
            return true;
        }
        catch(Exception)
        {
            keyValuePair = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to get first element of <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, out TSource value)
    {
        if(source == null)
        {
            value = default;
            return false;
        }
        var list = source as IList<TSource>;
        if(list != null)
        {
            if(list.Count > 0)
            {
                value = list[0];
                return true;
            }
        }
        else
        {
            using(var e = source.GetEnumerator())
            {
                if(e.MoveNext())
                {
                    value = e.Current;
                    return true;
                }
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get first element of IEnumerable
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate">Function to test elements.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource value)
    {
        if(source == null)
        {
            value = default;
            return false;
        }
        if(predicate == null)
        {
            value = default;
            return false;
        }
        foreach(var element in source)
        {
            if(predicate(element))
            {
                value = element;
                return true;
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get last element of <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="predicate">Function to test elements.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetLast<K>(this IEnumerable<K> enumerable, Func<K, bool> predicate, out K value)
    {
        try
        {
            value = enumerable.Last(predicate);
            return true;
        }
        catch(Exception)
        {
            value = default;
            return false;
        }
    }

    public static void MoveItemToPosition<T>(IList<T> list, Func<T, bool> sourceItemSelector, int targetedIndex)
    {
        var sourceIndex = -1;
        for(var i = 0; i < list.Count; i++)
        {
            if(sourceItemSelector(list[i]))
            {
                sourceIndex = i;
                break;
            }
        }
        if(sourceIndex == targetedIndex) return;
        var item = list[sourceIndex];
        list.RemoveAt(sourceIndex);
        list.Insert(targetedIndex, item);
    }
}
