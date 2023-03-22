using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public class DataAnimalContainer : IBaseDataContainer
{
    private Dictionary<int, DataAnimal> dicById = new();
    private Dictionary<string, DataAnimal> dicByNameId = new();
    public string FileName => "Animal";
    public string LocalJsonPath => PathDefine.JsonPath + $"/{FileName}.json";
    public void SerializeJson(string json)
    {
        try
        {
            JArray jArray = JArray.Parse(json);
            foreach(var jObj in jArray)
            {
                DataAnimal data = JsonConvert.DeserializeObject<DataAnimal>(jObj.ToString());
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
    public DataAnimal GetById(int id)
    {
        if (dicById.ContainsKey(id))
            return dicById[id];

        return default;
    }
    public DataAnimal GetByNameId(string nameId)
    {
        if (dicByNameId.ContainsKey(nameId))
            return dicByNameId[nameId];

        return default;
    }
}