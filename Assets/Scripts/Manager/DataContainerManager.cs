using System;
using System.Collections.Generic;
using UnityEngine;

public class DataContainerManager : BaseManager<DataContainerManager>
{
    private Dictionary<Type, object> containerDic = new();

    public DataContainer<T> GetDataContainer<T>() where T : IBaseData
    {
        return containerDic.ContainsKey(typeof(T)) ? (DataContainer<T>)containerDic[typeof(T)] : default;
    }

    public T GetDataById<T>(int id) where T : IBaseData
    {
        return GetDataContainer<T>() != default ? GetDataContainer<T>().GetById(id) : default;
    }

    public T GetDataByNameId<T>(string nameId) where T : IBaseData
    {
        return GetDataContainer<T>() != default ? GetDataContainer<T>().GetByNameId(nameId) : default;
    }

    public T FindData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != default ? GetDataContainer<T>().Find(predicate) : default;
    }

    public T[] FindAllData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != default ? GetDataContainer<T>().FindAll(predicate) : default;
    }

    public bool AddDataContainer<T>(string json) where T : IBaseData
    {
        if (containerDic.ContainsKey(typeof(T)))
        {
            Debug.LogError($"이미 추가된 DataContainer : {typeof(T)}");
            return false;
        }

        DataContainer<T> dataContainer = new();

        if (dataContainer.DeserializeJson(json))
        {
            containerDic[typeof(T)] = dataContainer;
            return true;
        }

        return false;
    }
	
	public bool AddDataContainer(string fileName, string json)
    {
        Type type = Type.GetType($"Data{fileName}");

        switch (type)
        {
             
			case Type t when t == typeof(DataAnimal):
                return AddDataContainer<DataAnimal>(json);
 
			case Type t when t == typeof(DataHuman):
                return AddDataContainer<DataHuman>(json);

        }
        
        Debug.LogError($"Invalid Type : {fileName}");

        return false;
    }
}
