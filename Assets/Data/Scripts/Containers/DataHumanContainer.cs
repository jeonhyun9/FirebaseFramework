using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System;

public class DataHumanContainer : IBaseDataContainer
{
    private Dictionary<int, DataHuman> dicById = null;
    private Dictionary<string, DataHuman> dicByNameId = null;
	private DataHuman[] datas = null;
    public string FileName => "Human";
	
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
                DataHuman data = JsonConvert.DeserializeObject<DataHuman>(jObj.ToString());
                if (!dicById.ContainsKey(data.Id))
                {
					if (dicById == null)
                        dicById = new();
						
                    dicById.Add(data.Id, data);
                }
                else
                {
                    Debug.LogError($"ID 중복 {data.GetType()} / {data.Id}");
                }

                if (!dicByNameId.ContainsKey(data.NameId))
                {
					if (dicByNameId == null)
                        dicByNameId = new();
						
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
	public DataHuman Find(Predicate<DataHuman> predicate)
    {
        return Array.Find(datas, predicate);
    }

    public DataHuman[] FindAll(Predicate<DataHuman> predicate)
    {
        return Array.FindAll(datas, predicate);
    }
}