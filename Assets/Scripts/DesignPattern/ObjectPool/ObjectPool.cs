using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> : IObjectPool where T : ObjectBase
{
    private Queue<T> _pool = new Queue<T>();

    public int ObjCount => _pool.Count;

    public void Push(T obj)
    {
        _pool.Enqueue(obj); 
    }

    public T Pop()
    {
        return _pool.Count > 0 ? _pool.Dequeue() : null;
    }
}
