using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataContainer<T> where T : IBaseData
{
    private Dictionary<int, T> dicById = null;
    private Dictionary<string, T> dicByNameId = null;
    private T[] datas = null;

    public bool Deserialized => dicById != null && dicByNameId != null && datas != null;
    public bool DeserializeJson(string json)
    {
        try
        {
            Debug.Log(json);

            JArray jArray = JArray.Parse(json);
            foreach (var jObj in jArray)
            {
                T data = JsonConvert.DeserializeObject<T>(jObj.ToString());

                if (dicById == null)
                    dicById = new();

                if (!dicById.ContainsKey(data.Id))
                {
                    dicById.Add(data.Id, data);
                }
                else
                {
                    Debug.LogError($"ID 중복 {data.GetType()} / {data.Id}");
                    return false;
                }

                if (dicByNameId == null)
                    dicByNameId = new();

                if (data.NameId == null)
                {
                    Debug.LogError($"NameID null");
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
