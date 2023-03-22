#pragma warning disable 0649
using Newtonsoft.Json;

public struct DataAnimal : IBaseData
{
    [JsonProperty(PropertyName = "Id")]
    public readonly int Id;
    [JsonProperty(PropertyName = "NameId")]
    public readonly string NameId;
    [JsonProperty(PropertyName = "AnimalType")]
    public readonly AnimalType AnimalType;

    public bool IsInit => Id == 0;
}
