using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ECommons.Collections;

public readonly struct OneOrMany<T>
{
    private readonly T Single;
    private readonly T[]? Many;
    public readonly bool IsPopulated;

    public static readonly OneOrMany<T> Empty = default;

    public OneOrMany(T single)
    {
        Single = single;
        Many = null;
        IsPopulated = true;
    }

    [OverloadResolutionPriority(1)]
    public OneOrMany(T[] many)
    {
        Single = many.Length > 0 ? many[0] : default!;
        Many = many.Length > 1 ? many : null;
        IsPopulated = many.Length > 0;
    }

    public OneOrMany(List<T> many)
    {
        Single = many.Count > 0 ? many[0] : default!;
        Many = many.Count > 1 ? [..many] : null;
        IsPopulated = many.Count > 0;
    }

    [OverloadResolutionPriority(-1)]
    public OneOrMany(IEnumerable<T> many)
    {
        int i = 0;
        List<T> ret = null;
        foreach(var x in many)
        {
            IsPopulated = true;
            if(i == 0)
            {
                Single = x;
            }
            else
            {
                ret ??= [];
                ret.Add(x);
            }
        }
        if(ret != null)
        {
            Many = [Single, .. ret];
        }
    }

    public int Count => !IsPopulated?0:Many?.Length ?? (Single is not null ? 1 : 0);

    public T this[int index] => !IsPopulated?throw new IndexOutOfRangeException(): Many is not null ? Many[index] : index == 0 ? Single : throw new IndexOutOfRangeException();

    public Enumerator GetEnumerator() => new(this);

    public struct Enumerator
    {
        private readonly OneOrMany<T> Source;
        private int Index;

        internal Enumerator(OneOrMany<T> source)
        {
            Source = source;
            Index = -1;
        }

        public bool MoveNext() => Source.IsPopulated && ++Index < Source.Count;
        public T Current => Source[Index];
    }
}
