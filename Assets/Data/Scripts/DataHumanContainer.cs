using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public class DataHumanContainer
    {
        private Dictionary<int, DataHuman> dicById = new();

        private Dictionary<string, DataHuman> dicByNameId = new();

        public void SerializeJson(string json)
        {
            try
            {
                JArray jArray = JArray.Parse(json);

                foreach(var jObj in jArray)
                {
                    DataHuman data = JsonConvert.DeserializeObject<DataHuman>(jObj.ToString());
                    dicById.Add(data.Id, data);
                    dicByNameId.Add(data.NameId, data);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.StackTrace);
            }
            
            
            
        }

        public DataHuman GetById(int id)
        {
            if (dicById.ContainsKey(id))
                return dicById[id];

            return default;
        }

        public DataHuman GetByDataId(string nameId)
        {
            if (dicByNameId.ContainsKey(nameId))
                return dicByNameId[nameId];

            return default;
        }
    }
}


