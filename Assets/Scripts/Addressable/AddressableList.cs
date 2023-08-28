using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct AddressableList
{
    [JsonProperty]
    public IReadOnlyList<string> FileNameList;

    [JsonProperty]
    public IReadOnlyDictionary<Type, Dictionary<string, string>> AddressableDic;

    public AddressableList(List<string> pathListValue, Dictionary<Type, Dictionary<string, string>> addressableDicValue)
    {
        FileNameList = pathListValue;
        AddressableDic = addressableDicValue;
    }
}
