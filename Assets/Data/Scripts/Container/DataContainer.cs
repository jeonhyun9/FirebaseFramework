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
            Debug.LogError("json is null!!");
            return false;
        }

        try
        {
            JArray jArray = JArray.Parse(json);
            foreach (var jObj in jArray)
            {
                T data = JsonConvert.DeserializeObject<T>(jObj.ToString());

                if (!dicById.ContainsKey(data.Id))
                {
                    dicById.Add(data.Id, data);
                }
                else
                {
                    Debug.LogError($"ID 중복 {data.GetType()} / {data.Id}");
                    return false;
                }

                if (data.NameId == null)
                {
                    Debug.LogError($"NameID is null");
                    return false;
                }
                    
                if (!dicByNameId.ContainsKey(data.NameId))
                {
                    dicByNameId.Add(data.NameId, data);
                }
                else
                {
                    Debug.LogError($"NameID 중복 {data.GetType()} / {data.NameId}");
                    return false;
                }
            }
            datas = dicById.Values.ToArray();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Json Parsing 실패 !!");
            Debug.LogError(e);
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
}
