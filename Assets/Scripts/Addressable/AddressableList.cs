using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct AddressableList
{
    [JsonProperty]
    private List<string> addressableBuildStoargePathList;

    [JsonProperty]
    private Dictionary<Type, Dictionary<string, string>> addressableDic;

    public IReadOnlyList<string> FileNameList => addressableBuildStoargePathList;

    public IReadOnlyDictionary<Type, Dictionary<string, string>> AddressableDic => addressableDic;

    public AddressableList(List<string> pathListValue, Dictionary<Type, Dictionary<string, string>> addressableDicValue)
    {
        addressableBuildStoargePathList = pathListValue;
        addressableDic = addressableDicValue;
    }
}
