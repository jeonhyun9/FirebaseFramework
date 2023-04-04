#pragma warning disable 0649
using Newtonsoft.Json;

public struct DataAnimal : IBaseData
{
    [JsonProperty(PropertyName = "_Id")]
    public readonly int _Id;
    [JsonProperty(PropertyName = "_NameId")]
    private readonly string _nameId;
    [JsonProperty(PropertyName = "AnimalType")]
    public readonly AnimalType AnimalType;

    public int Id => _Id;
    public string NameId => _nameId;
    public bool IsInit => _Id == 0;
}
