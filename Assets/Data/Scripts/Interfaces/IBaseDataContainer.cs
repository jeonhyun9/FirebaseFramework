public interface IBaseDataContainer
{
    public void SerializeJson(string json);

    public string FileName { get; }

#if UNITY_EDITOR
    public string LocalJsonPath { get; }
#endif

    public bool Serialized { get; }
}
