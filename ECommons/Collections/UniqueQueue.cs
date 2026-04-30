using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.Collections;

public class UniqueQueue<T> : IEnumerable<T>
{
    private HashSet<T> HashSet;
    private Queue<T> Queue;


    public UniqueQueue()
    {
        HashSet = [];
        Queue = [];
    }


    public int Count
    {
        get
        {
            return HashSet.Count;
        }
    }

    public void Clear()
    {
        HashSet.Clear();
        Queue.Clear();
    }


    public bool Contains(T item)
    {
        return HashSet.Contains(item);
    }


    public void Enqueue(T item)
    {
        if(HashSet.Add(item))
        {
            Queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        T item = Queue.Dequeue();
        HashSet.Remove(item);
        return item;
    }

    public bool TryDequeue(out T value)
    {
        if(Queue.TryDequeue(out value))
        {
            HashSet.Remove(value);
            return true;
        }
        return false;
    }


    public T Peek()
    {
        return Queue.Peek();
    }


    public IEnumerator<T> GetEnumerator()
    {
        return Queue.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return Queue.GetEnumerator();
    }
}

