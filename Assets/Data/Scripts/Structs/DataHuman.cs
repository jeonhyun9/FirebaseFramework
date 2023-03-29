#pragma warning disable 0649
using Newtonsoft.Json;

public struct DataHuman : IBaseData
{
    [JsonProperty(PropertyName = "Id")]
    public readonly int Id;
    [JsonProperty(PropertyName = "NameId")]
    public readonly string NameId;
    [JsonProperty(PropertyName = "Scores")]
    public readonly int[] Scores;
	[JsonProperty(PropertyName = "Pet_DataAnimal")]
    private readonly string Pet_DataAnimal;
    public readonly DataAnimal Pet
    {
        get
        {
		    if (DataContainerManager.Instance.GetDataContainer<DataAnimalContainer>() == null)
                return default;
				
            return DataContainerManager.Instance.GetDataContainer<DataAnimalContainer>()
                .GetByNameId(Pet_DataAnimal);
        }
    }

    public bool IsInit => Id == 0;
}
