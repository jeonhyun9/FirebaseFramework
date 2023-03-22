using System;
using System.Collections.Generic;

public class DataContainerManager : BaseManager<DataContainerManager>
{
    private Dictionary<Type, IBaseDataContainer> containerDic = new();

    public void AddDataContainerFromJson<T>(string json) where T : IBaseDataContainer, new()
    {
        containerDic[typeof(T)] = new T();
        containerDic[typeof(T)].SerializeJson(json);
    }

    public void AddDataContainerFromLocalJson<T>() where T : IBaseDataContainer, new()
    {
        containerDic[typeof(T)] = new T();
        containerDic[typeof(T)].SerializeJson(System.IO.File.ReadAllText(containerDic[typeof(T)].LocalJsonPath));
    }

    public T GetDataContainer<T>() where T : IBaseDataContainer
    {
        return containerDic.ContainsKey(typeof(T)) ? (T)containerDic[typeof(T)] : default;
    }
}
