using System;
using System.Collections.Generic;

public class DataContainerManager : BaseManager<DataContainerManager>
{
    private Dictionary<Type, IBaseDataContainer> containerDic = new();

#if UNITY_EDITOR
    public void AddDataContainerFromLocalJson<T>() where T : IBaseDataContainer, new()
    {
        containerDic[typeof(T)] = new T();
        containerDic[typeof(T)].SerializeJson(System.IO.File.ReadAllText(containerDic[typeof(T)].LocalJsonPath));
    }
#endif

    public T GetDataContainer<T>() where T : IBaseDataContainer
    {
        return containerDic.ContainsKey(typeof(T)) ? (T)containerDic[typeof(T)] : default;
    }

    public bool AddDataContainer(string fileName, string json)
    {
        fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

        string containerName = $"Data{fileName}Container";

        Type containerType = Type.GetType(containerName);

        if (containerType == null)
            return false;

        if (!typeof(IBaseDataContainer).IsAssignableFrom(containerType))
            return false;

        IBaseDataContainer container = (IBaseDataContainer)Activator.CreateInstance(containerType);
        containerDic[containerType] = container;
        containerDic[containerType].SerializeJson(json);

        return true;
    }
}
