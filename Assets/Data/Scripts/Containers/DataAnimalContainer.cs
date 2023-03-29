using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System;

public class DataAnimalContainer : IBaseDataContainer
{
    private Dictionary<int, DataAnimal> dicById = null;
    private Dictionary<string, DataAnimal> dicByNameId = null;
	private DataAnimal[] datas = null;
    public string FileName => "Animal";
	
	#if UNITY_EDITOR
    public string LocalJsonPath => PathDefine.Json + $"/{FileName}.json";
	#endif
	
	public bool Serialized => dicById != null && dicByNameId != null && datas != null;
    public void SerializeJson(string json)
    {
        try
        {
            JArray jArray = JArray.Parse(json);
            foreach(var jObj in jArray)
            {
                DataAnimal data = JsonConvert.DeserializeObject<DataAnimal>(jObj.ToString());
				
				if (dicById == null)
					dicById = new();
						
                if (!dicById.ContainsKey(data.Id))
                {
                    dicById.Add(data.Id, data);
                }
                else
                {
                    Debug.LogError($"ID 중복 {data.GetType()} / {data.Id}");
                }
				
				if (dicByNameId == null)
					dicByNameId = new();
						
                if (!dicByNameId.ContainsKey(data.NameId))
                {
                    dicByNameId.Add(data.NameId, data);
                }
                else
                {
                    Debug.LogError($"NameID 중복 {data.GetType()} / {data.NameId}");
                }
            }
			datas = dicById.Values.ToArray();
        }
        catch (Exception e)
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
	public DataAnimal Find(Predicate<DataAnimal> predicate)
    {
        return Array.Find(datas, predicate);
    }

    public DataAnimal[] FindAll(Predicate<DataAnimal> predicate)
    {
        return Array.FindAll(datas, predicate);
    }
}