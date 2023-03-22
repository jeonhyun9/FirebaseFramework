using UnityEngine;

public interface IBaseDataContainer
{
    public void SerializeJson(string json);

    public string FileName { get; }

    public string LocalJsonPath { get; }
}
