#pragma warning disable 0649
using Newtonsoft.Json;

namespace Data
{
    public struct DataHuman : BaseData
    {
        [JsonProperty(PropertyName = "Id")]
        public readonly int Id;
        [JsonProperty(PropertyName = "Name")]
        public readonly string NameId;
        [JsonProperty(PropertyName = "Scores")]
        public readonly int[] Scores;

        public bool IsInit => Id == 0;
    }
}

