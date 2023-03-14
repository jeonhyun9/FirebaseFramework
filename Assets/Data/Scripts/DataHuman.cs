#pragma warning disable 0649
using Newtonsoft.Json;

public struct DataHuman : IBaseData
{
    [JsonProperty(PropertyName = "Id")]
    public readonly int Id;
    [JsonProperty(PropertyName = "Name")]
    public readonly string Name;
    [JsonProperty(PropertyName = "Scores")]
    public readonly int[] Scores;
}
