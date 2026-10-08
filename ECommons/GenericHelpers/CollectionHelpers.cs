using ECommons.MathHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons;

public static unsafe partial class GenericHelpers
{
    public static void SortBy<T, TKey>(this Span<T> span, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        SortCore(span, keySelector, comparer, descending: false);
    }

    public static void SortByDescending<T, TKey>(this Span<T> span, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        SortCore(span, keySelector, comparer, descending: true);
    }

    public static void SortBy<T, TKey>(this T[] array, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(array);
        SortCore(array.AsSpan(), keySelector, comparer, descending: false);
    }

    public static void SortByDescending<T, TKey>(this T[] array, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(array);
        SortCore(array.AsSpan(), keySelector, comparer, descending: true);
    }

    public static void SortBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        SortCore(CollectionsMarshal.AsSpan(list), keySelector, comparer, descending: false);
    }

    public static void SortByDescending<T, TKey>(this List<T> list, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        SortCore(CollectionsMarshal.AsSpan(list), keySelector, comparer, descending: true);
    }

    [OverloadResolutionPriority(-1)]
    public static void SortBy<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        SortIList(list, keySelector, comparer, descending: false);
    }

    [OverloadResolutionPriority(-1)]
    public static void SortByDescending<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        SortIList(list, keySelector, comparer, descending: true);
    }

    private static void SortIList<T, TKey>(IList<T> list, Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
    {
        ArgumentNullException.ThrowIfNull(list);

        switch(list)
        {
            case T[] arr: SortCore(arr.AsSpan(), keySelector, comparer, descending); return;
            case List<T> l: SortCore(CollectionsMarshal.AsSpan(l), keySelector, comparer, descending); return;
        }
        var buffer = new T[list.Count];
        list.CopyTo(buffer, 0);
        SortCore(buffer.AsSpan(), keySelector, comparer, descending);
        for(var i = 0; i < buffer.Length; i++)
        {
            list[i] = buffer[i];
        }
    }

    private static void SortCore<T, TKey>(Span<T> span, Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        var n = span.Length;
        if(n < 2)
        {
            return;
        }
        comparer ??= Comparer<TKey>.Default;
        var items = span.ToArray();
        var keys = new TKey[n];
        var order = new int[n];
        for(var i = 0; i < n; i++)
        {
            keys[i] = keySelector(items[i]);
            order[i] = i;
        }

        Array.Sort(order, (x, y) =>
        {
            var c = descending ? comparer.Compare(keys[y], keys[x]) : comparer.Compare(keys[x], keys[y]);
            return c != 0 ? c : x.CompareTo(y);
        });

        for(var i = 0; i < n; i++)
        {
            span[i] = items[order[i]];
        }
    }

    /// <summary>
    /// Adds an item to collection if it doesn't exist yet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="value"></param>
    /// <returns>Whether the item was successfully added</returns>
    public static bool AddIfNotExist<T>(this ICollection<T> collection, T value)
    {
        if(collection.Contains(value))
        {
            return false;
        }

        collection.Add(value);
        return true;
    }


    /// <inheritdoc cref="IsNullOrEmpty{T}(IEnumerable{T})"/>
    [OverloadResolutionPriority(1)]
    public static bool IsNullOrEmpty<T>(this List<T> value)
    {
        return value == null ? true : value.Count == 0;
    }

    /// <inheritdoc cref="IsNullOrEmpty{T}(IEnumerable{T})"/>
    [OverloadResolutionPriority(1)]
    public static bool IsNullOrEmpty<T>(this T[] value)
    {
        return value == null ? true : value.Length == 0;
    }

    /// <summary>
    /// Checks whether the collection is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
    {
        return value == null ? true : !value.Any();
    }

    /// <summary>
    /// Retrieves every <paramref name="num"/>th element from <paramref name="values"/>. First element is always returned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> values, int num)
    {
        ArgumentOutOfRangeException.ThrowIfZero(num);
        var i = 0;
        var e = values.GetEnumerator();
        while(e.MoveNext())
        {
            if(i % num == 0)
            {
                yield return e.Current;
            }

            i++;
        }
    }

    /// <summary>
    /// Retrieves first element of <paramref name="values"/> that matches the <paramref name="predicate"/> or null if no such element exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T? FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) where T : struct
    {
        return values.TryGetFirst(predicate, out var result) ? result : null;
    }

    /// <summary>
    /// Retrieves first element of <paramref name="values"/> or null if no such element exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static T? FirstOrNull<T>(this IEnumerable<T> values) where T : struct
    {
        return values.TryGetFirst(out var result) ? result : null;
    }

    /// <summary>
    /// Converts IEnumerable of value types to IEnumerable of nullable value types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> values) where T : struct
    {
        return values.Cast<T?>();
    }

    /// <summary>
    /// Checks whether <paramref name="values"/> contains <paramref name="value"/>. Returns false if <paramref name="value"/> is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ContainsNullable<T>(this IEnumerable<T> values, T? value) where T : struct
    {
        return value != null && Enumerable.Contains(values, value.Value);
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
    {
        return enumerable.ElementAt(Random.Shared.Next(enumerable.Count()));
    }

    /// <inheritdoc cref="SafeSelect{K, V}(IReadOnlyDictionary{K, V}, K, V)"/>
    public static V? SafeSelect<K, V>(this IReadOnlyDictionary<K, V> dictionary, K? key)
    {
        return SafeSelect(dictionary, key, default);
    }

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
        if(dictionary == null)
        {
            return default;
        }

        return key == null ? default : dictionary.TryGetValue(key, out var ret) ? ret : defaultValue;
    }

    /// <summary>
    /// Selects an entry of the <paramref name="list"/> at a specified <paramref name="index"/>, wrapping around if index is out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T CircularSelect<T>(this IList<T> list, int index)
    {
        return list[MathHelper.Mod(index, list.Count)];
    }

    /// <inheritdoc cref="CircularSelect{T}(IList{T}, int)"/>
    public static T CircularSelect<T>(this T[] list, int index)
    {
        return list[MathHelper.Mod(index, list.Length)];
    }

    /// <summary>
    /// Safely selects an entry of the <paramref name="list"/> at a specified <paramref name="index"/>, returning default value if index is out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T SafeSelect<T>(this IReadOnlyList<T> list, int index)
    {
        return list == null ? default : index < 0 || index >= list.Count ? default : list[index];
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
        return index < 0 || index >= array.Length ? default : array[index];
    }

    [Obsolete($"Use {nameof(SafeSelect)}")]
    public static T GetOrDefault<T>(this IReadOnlyList<T> List, int index)
    {
        return SafeSelect(List, index);
    }

    [Obsolete($"Use {nameof(SafeSelect)}")]
    public static T GetOrDefault<T>(this T[] Array, int index)
    {
        return SafeSelect(Array, index);
    }

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
        return List.TryDequeue(out var ret) ? ret : throw new InvalidOperationException("Sequence contains no elements");
    }

    /// <summary>
    /// Treats list as a queue, removing and returning element at last index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <param name="result"></param>
    /// <returns>Whether it was dequeued</returns>
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

    /// <inheritdoc cref="TryDequeueLast{T}(IList{T}, out T)"/>
    public static T DequeueLast<T>(this IList<T> List)
    {
        return List.TryDequeueLast(out var ret) ? ret : throw new InvalidOperationException("Sequence contains no elements");
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
        return Queue.Count > 0 ? Queue.Dequeue() : default;
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

    /// <summary>
    /// Checks whether <paramref name="haystack"/> contains <paramref name="needle"/> ignoring case.
    /// </summary>
    /// <param name="haystack"></param>
    /// <param name="needle"></param>
    /// <returns></returns>
    public static bool ContainsIgnoreCase(this IEnumerable<string> haystack, string needle)
    {
        foreach(var x in haystack)
        {
            if(x.EqualsIgnoreCase(needle))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Merges two arrays into one, removing duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="additionalValues"></param>
    /// <returns></returns>
    public static T[] Together<T>(this T[] array, params T[] additionalValues)
    {
        return array.Union(additionalValues).ToArray();
    }

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

    /// <summary>
    /// Retrieves a value from dictionary or default value if it doesn't exists.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static V GetOrDefault<K, V>(this IDictionary<K, V> dic, K key)
    {
        return dic.TryGetValue(key, out var value) ? value : default;
    }

    /// <summary>
    /// Increments value of <paramref name="key"/> in <paramref name="dic"/> by <paramref name="increment"/> or sets it to <paramref name="increment"/> if it doesn't exists yet.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <param name="increment"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Prints IEnumerable to string, separating elements with <paramref name="separator"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Print<T>(this IEnumerable<T> x, string separator = ", ")
    {
        return x.Select(x => x?.ToString() ?? "").Join(separator);
    }

    [Obsolete("Use SafeSelect")]
    public static V GetSafe<K, V>(this IDictionary<K, V> dic, K key, V Default = default)
    {
        return dic?.TryGetValue(key, out var value) == true ? value : Default;
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

    ///<inheritdoc cref="GetOrCreate{K, V}(IDictionary{K, V}, K)"/>
    public static V GetOrCreate<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
    {
        if(dictionary.TryGetValue(key, out var result))
        {
            return result;
        }
        dictionary.Add(key, defaultValue);
        return defaultValue;
    }

    ///<inheritdoc cref="GetOrCreate{K, V}(IDictionary{K, V}, K)"/>
    public static V GetOrCreate<K, V>(this IDictionary<K, V> dictionary, K key, Func<V> defaultValueGenerator)
    {
        if(dictionary.TryGetValue(key, out var result))
        {
            return result;
        }
        dictionary.Add(key, defaultValueGenerator());
        return dictionary[key];
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

    /// <summary>
    /// Converts IEnumerable to IEnumerable of tuples containing value and index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> src)
    {
        return src.Select((x, i) => (x, i));
    }


    /// <summary>
    /// Executes action for each element of collection, providing index of the element as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="function"></param>
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

    /// <summary>
    /// Adds <paramref name="value"/> into List if it doesn't exists yet or removes if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns first element of <paramref name="collection"/> that matches the <paramref name="predicate"/> or first element of <paramref name="collection"/> if no such element exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Applies multiple functions to each element of <paramref name="values"/> and returns the results as a single IEnumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="values"></param>
    /// <param name="funcs"></param>
    /// <returns></returns>
    public static IEnumerable<R> SelectMulti<T, R>(this IEnumerable<T> values, params Func<T, R>[] funcs)
    {
        foreach(var v in values)
        {
            foreach(var x in funcs)
            {
                yield return x(v);
            }
        }
    }

    /// <summary>
    /// Projects each element of a nested sequence to an IEnumerable and flattens the resulting sequences into one sequence.
    /// </summary>
    public static IEnumerable<TResult> SelectNested<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> source, Func<TSource, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        foreach(var innerSequence in source)
        {
            if(innerSequence != null)
            {
                foreach(var item in innerSequence)
                {
                    yield return selector(item);
                }
            }
        }
    }

    /// <summary>
    /// Checks whether <paramref name="source"/> contains all elements of <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
    {
        foreach(var x in values)
        {
            if(!source.Contains(x))
            {
                return false;
            }
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this IEnumerable<string> e, string separator)
    {
        return string.Join(separator, e);
    }

    /// <summary>
    /// Checks whether <paramref name="obj"/> contains any of the <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="values"></param>
    /// <returns></returns>
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

    ///<inheritdoc cref="ContainsAny{T}(IEnumerable{T}, T[])"/>
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

    /// <summary>
    /// Checks whether all of the <paramref name="objects"/> are null.
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllNull(params object[] objects)
    {
        return objects.All(s => s == null);
    }

    /// <summary>
    /// Checks whether any of the <paramref name="objects"/> is null.
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyNull(params object[] objects)
    {
        return objects.Any(s => s == null);
    }

    /// <summary>
    /// Finds all keys in <paramref name="dictionary"/> that have the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Attempts to get first element of <paramref name="dictionary"/> that matches the <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="predicate"></param>
    /// <param name="keyValuePair"></param>
    /// <returns></returns>
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
        if(source is IList<TSource> list)
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
    /// Attempts to get last element of <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetLast<TSource>(this IEnumerable<TSource> source, out TSource value)
    {
        if(source == null)
        {
            value = default;
            return false;
        }
        if(source is IList<TSource> list)
        {
            if(list.Count > 0)
            {
                value = list[^1];
                return true;
            }
        }
        else
        {
            if(source.Any())
            {
                value = Enumerable.Last(source);
                return true;
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

    /// <summary>
    /// Moves an item in a list to a specified index, based on a selector function to identify the source item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="sourceItemSelector"></param>
    /// <param name="targetedIndex"></param>
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
        if(sourceIndex == targetedIndex)
        {
            return;
        }

        var item = list[sourceIndex];
        list.RemoveAt(sourceIndex);
        list.Insert(targetedIndex, item);
    }
}
