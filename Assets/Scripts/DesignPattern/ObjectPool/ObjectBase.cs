using System;
using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    protected string _id;

    public string ID => _id;

    private void Awake()
    {
        SetID();
    }

    private void SetID()
    {
        if(string.IsNullOrEmpty(_id))
        _id = Guid.NewGuid().ToString();
    }
}
