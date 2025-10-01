using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AdvancedMonoBehaviour.Scripts.Base;
using AdvancedMonoBehaviour.Scripts.Interfaces;
using UnityEngine;

public abstract class AdvancedMono : MonoBehaviour
{
    public int weight = 0;
    public Dictionary<Type, Component> components = new Dictionary<Type, Component>();
    public string mTag;
    public string mName;
    
    
    private void Start()
    {
        OnStart onStart = null;
        OnUpdate onUpdate = null;
        if(typeof(OnUpdate).GetTypeInfo().IsAssignableFrom(this.GetType().UnderlyingSystemType))
            onUpdate = this as OnUpdate;
        if(typeof(OnStart).GetTypeInfo().IsAssignableFrom(this.GetType().UnderlyingSystemType))
            onStart = this as OnStart;
        
        MonoGod.Instance.Register(this, weight, onUpdate, onStart);
        mTag = tag;
        mName = name;
    }

    protected bool MCompareTag(string tag)
    {
        return (mTag == tag);
    }
    

    protected T MGetComponent<T>() where T : Component
    {
        var Type = typeof(T);
        if (components.ContainsKey(Type)){
            return components[Type] as T;
        }
        else
        {
            var x = GetComponent(typeof(T));
            components.Add(Type, x);
            return x as T;
        }

        return null;
    }
    
    

    private void OnDestroy()
    {
        MonoGod.Instance.DeRegister(this);
    }
}
