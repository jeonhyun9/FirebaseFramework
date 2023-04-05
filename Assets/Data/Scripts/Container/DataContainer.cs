using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataContainer<T> where T : IBaseData
{
    private readonly Dictionary<int, T> dicById = new();
    private readonly Dictionary<string, T> dicByNameId = new();
    private T[] datas = null;

    public bool Deserialized => dicById != null && dicByNameId != null && datas != null;
    public bool DeserializeJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Logger.Null($"{typeof(T)}");
            return false;
        }

        try
        {
            JArray jArray = JArray.Parse(json);
            foreach (var jObj in jArray)
            {
                T data = JsonConvert.DeserializeObject<T>(jObj.ToString());

                if (!TryAddData(data))
                    return false;
            }
            datas = dicById.Values.ToArray();

            return true;
        }
        catch (Exception e)
        {
            Logger.Error("Json parsing failed");
            Logger.Exception(e);
            return false;
        }
    }

    public T GetById(int id)
    {
        if (dicById.ContainsKey(id))
            return dicById[id];

        return default;
    }

    public T GetByNameId(string nameId)
    {
        if (string.IsNullOrEmpty(nameId))
            return default;

        if (dicByNameId.ContainsKey(nameId))
            return dicByNameId[nameId];

        return default;
    }

    public T Find(Predicate<T> predicate)
    {
        return Array.Find(datas, predicate);
    }

    public T[] FindAll(Predicate<T> predicate)
    {
        return Array.FindAll(datas, predicate);
    }

    private bool TryAddData(T data)
    {
        if (data.NameId is null)
        {
            Logger.Null($"{typeof(T)}");
            return false;
        }

        if (!dicById.TryAdd(data.Id, data))
        {
            Logger.Error($"Duplicated Id : {data.GetType()} / {data.Id}");
            return false;
        }

        if (!dicByNameId.TryAdd(data.NameId, data))
        {
            Logger.Error($"Duplicated NameId : {data.GetType()} / {data.NameId}");
            return false;
        }

        return true;
    }
}
