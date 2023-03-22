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

    public bool IsInit => Id == 0;
}
