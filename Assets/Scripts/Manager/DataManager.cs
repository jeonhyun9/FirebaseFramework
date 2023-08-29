using System;
using System.Collections.Generic;
using System.Linq;

public class DataManager : BaseStaticManager<DataManager>
{
    private readonly Dictionary<Type, object> containerDic = new();

    public Type[] GetAllTypes() => containerDic.Keys.ToArray();

    public DataContainer<T> GetDataContainer<T>() where T : IBaseData
    {
        return containerDic.ContainsKey(typeof(T)) ? (DataContainer<T>)containerDic[typeof(T)] : null;
    }

    public T GetDataById<T>(int id) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().GetById(id) : default;
    }

    public T GetDataByNameId<T>(string nameId) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().GetByNameId(nameId) : default;
    }

    public T FindData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().Find(predicate) : default;
    }

    public T[] FindAllData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().FindAll(predicate) : default;
    }

    public T[] GetAllData<T>() where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().FindAll(x => x.IsInit) : default;
    }

    public bool AddDataContainer<T>(string json) where T : IBaseData
    {
        if (containerDic.ContainsKey(typeof(T)))
        {
            Logger.Error($"Duplicated DataContainer type : {typeof(T)}");
            return false;
        }

        DataContainer<T> dataContainer = new();

        if (dataContainer.DeserializeJson(json))
        {
            containerDic[typeof(T)] = dataContainer;
			Logger.Success($"Add DataContainer type : {typeof(T)}");
            return true;
        }

        return false;
    }

    public bool AddDataContainerByDataDic(Dictionary<string, string> dicJsonByFileName)
    {
        foreach (string fileName in dicJsonByFileName.Keys)
        {
            bool result = AddDataContainer(fileName, dicJsonByFileName[fileName]);

            if (!result)
                return false;
        }

        return true;
    }
	
	public bool AddDataContainer(string fileName, string json)
    {
        if (fileName.Contains(".json"))
            fileName = fileName.Replace(".json", "");

        Type type = Type.GetType($"Data{fileName}");

        switch (type)
        {
             
			case Type t when t == typeof(DataAnimal):
                return AddDataContainer<DataAnimal>(json);
 
			case Type t when t == typeof(DataHuman):
                return AddDataContainer<DataHuman>(json);
        }
        
        Logger.Error($"Invalid Type : {fileName}");

        return false;
    }
}