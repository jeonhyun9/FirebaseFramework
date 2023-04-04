#pragma warning disable 0649
using Newtonsoft.Json;

public struct DataHuman : IBaseData
{
    [JsonProperty(PropertyName = "_Id")]
    public int _Id;
    [JsonProperty(PropertyName = "_NameId")]
    public string _NameId;
    [JsonProperty(PropertyName = "Scores")]
    public int[] Scores;
	[JsonProperty(PropertyName = "Pet_DataAnimal")]
    private readonly string Pet_DataAnimal;
    public readonly DataAnimal Pet
    {
        get
        {
		    if (DataContainerManager.Instance.GetDataContainer<DataAnimal>() == null)
                return default;
				
            return DataContainerManager.Instance.GetDataByNameId<DataAnimal>(Pet_DataAnimal);
        }
    }
	
	public int Id => _Id;
    public string NameId => _NameId;
    public bool IsInit => _Id == 0;
}
