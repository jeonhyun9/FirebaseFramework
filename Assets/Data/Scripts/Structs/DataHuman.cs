#pragma warning disable 0649
//This script is generated by jhg's DataGenerator.

using Newtonsoft.Json;

public struct DataHuman : IBaseData
{
    [JsonProperty(PropertyName = "id")]
    private readonly int id;
    [JsonProperty(PropertyName = "nameId")]
    private readonly string nameId;
    [JsonProperty(PropertyName = "Scores")]
    public readonly int[] Scores;
	[JsonProperty(PropertyName = "PetNameId")]
    private readonly string PetNameId;
    public readonly DataAnimal Pet
    {
        get
        {
		    if (DataContainerManager.Instance.GetDataContainer<DataAnimal>() == null)
                return default;
				
            return DataContainerManager.Instance.GetDataByNameId<DataAnimal>(PetNameId);
        }
    }
	
	public int Id => id;
    public string NameId => nameId;
    public bool IsInit => id != 0 && !string.IsNullOrEmpty(nameId);
}
