using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainerManager : BaseManager<DataContainerManager>
{
    private Dictionary<Type, IBaseDataContainer> containerDic = new();

    public void AddDataContainerFromJson<T>(string json) where T : IBaseDataContainer, new()
    {
        containerDic[typeof(T)] = new T();
        containerDic[typeof(T)].SerializeJson(json);
    }

    public IBaseDataContainer GetDateContainer(Type type)
    {
        return containerDic.ContainsKey(type) ? containerDic[type] : default;
    }
}
