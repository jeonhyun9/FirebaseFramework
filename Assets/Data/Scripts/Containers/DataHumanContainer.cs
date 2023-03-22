using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public class DataHumanContainer : IBaseDataContainer
{
    private Dictionary<int, DataHuman> dicById = new();
    private Dictionary<string, DataHuman> dicByNameId = new();
    public string FileName => "Human";
    public string LocalJsonPath => PathDefine.JsonPath + $"/{FileName}.json";
    public void SerializeJson(string json)
    {
        try
        {
            JArray jArray = JArray.Parse(json);
            foreach(var jObj in jArray)
            {
                DataHuman data = JsonConvert.DeserializeObject<DataHuman>(jObj.ToString());
                if (!dicById.ContainsKey(data.Id))
                {
                    dicById.Add(data.Id, data);
                }
                else
                {
                    Debug.LogError($"ID 중복 {data.GetType()} / {data.Id}");
                }

                if (!dicByNameId.ContainsKey(data.NameId))
                {
                    dicByNameId.Add(data.NameId, data);
                }
                else
                {
                    Debug.LogError($"NameID 중복 {data.GetType()} / {data.NameId}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Json Parsing 실패 !!");
            Debug.LogError(e.StackTrace);
        }
    }
    public DataHuman GetById(int id)
    {
        if (dicById.ContainsKey(id))
            return dicById[id];

        return default;
    }
    public DataHuman GetByNameId(string nameId)
    {
        if (dicByNameId.ContainsKey(nameId))
            return dicByNameId[nameId];

        return default;
    }
}