using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private Dictionary<Type, IObjectPool> _poolDict = new Dictionary<Type, IObjectPool>();
    // 현재 오브젝트풀로 관리할 오브젝트 리스트
    private List<ObjectBase> _objects = new List<ObjectBase>();
    private List<PunObjectBase> _punObjects = new List<PunObjectBase>();

    public void AddToObjLists(ObjectBase obj)
    {
        _objects.Add(obj);
    }

    //public T CreateObj<T>(T obj, Transform parents = null) where T : ObjectBase
    //{
    //    T newTObj = parents == null ? Instantiate(obj) : Instantiate(obj, parents);
    //    newTObj.name = $"{typeof(T)}{obj.ID}";

    //    AddToObjLists(newTObj);

    //    return newTObj;
    //}

    // delete object in pool and destroy
    public void RemoveObj<T>(T obj) where T : ObjectBase
    {
        _objects.Remove(obj);
        Destroy(obj);
    }

    // 풀에 오브젝트가 부족할 때 생성
    public T CreateObjWithUsePool<T>(T obj, Transform parents = null) where T : ObjectBase
    {
        // 먼저 Pool에서 Obj 가져오고
        T newTObj = GetObjFromPool<T>();

        // Pool에 없을 경우 생성
        if(newTObj == null)
        {
            newTObj = parents == null ? Instantiate(obj) : Instantiate(obj, parents);
            newTObj.name = $"{obj.gameObject.name}{obj.ID}";
        }
        return newTObj;
    }

    // Pool로 반납
    public void SetObjInPool<T>(T obj) where T : ObjectBase
    {
        CreatePool<T>();
        ObjectPool<T> objectPool = GetTPool<T>();

        obj.gameObject.SetActive(false);
        objectPool.Push(obj);
    }

    public T GetObjFromPool<T>() where T : ObjectBase
    {
        // T에 해당하는 풀 가져옴
        ObjectPool<T> objectPool = GetTPool<T>();
        if(objectPool != null && objectPool.ObjCount > 0)
        {
            T obj = objectPool.Pop();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return null;
    }

    private void CreatePool<T>() where T : ObjectBase
    {
        if (_poolDict.ContainsKey(typeof(T)) == false)
            _poolDict[typeof(T)] = new ObjectPool<T>();
    }

    private ObjectPool<T> GetTPool<T>() where T : ObjectBase
    {
        if (_poolDict.ContainsKey(typeof(T)) == false) return null;

        return _poolDict[typeof(T)] as ObjectPool<T>;
    }

    public int GetTObjCount<T>() where T : ObjectBase
    {
        if (_poolDict.ContainsKey(typeof(T)) == false) return 0;

        ObjectPool<T> objPool = GetTPool<T>();
        return objPool.ObjCount;
    }

    // 풀에 오브젝트 미리 생성
    public void Init<T>(T obj, int count, Transform parents = null) where T : ObjectBase
    {
        CreatePool<T>();

        for (int i = 0; i < count; i++) 
        {
            T newTObj = parents == null ? Instantiate(obj) : Instantiate(obj, parents);
            //Debug.Log($"[ObjectPoolManager] {obj.gameObject.name}({obj.ID})");
            newTObj.name = $"{obj.gameObject.name}{obj.ID}";

            AddToObjLists(newTObj);

            SetObjInPool(newTObj);
        }
    }
}
