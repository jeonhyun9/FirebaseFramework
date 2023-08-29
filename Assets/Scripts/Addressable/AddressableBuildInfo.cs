using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct AddressableBuildInfo
{
    [JsonProperty]
    public IReadOnlyDictionary<string, byte[]> FileNameWithByteDic;

    [JsonProperty]
    public IReadOnlyDictionary<Type, Dictionary<string, string>> AddressableDic;

    public AddressableBuildInfo(Dictionary<string, byte[]> pathListValue, Dictionary<Type, Dictionary<string, string>> addressableDicValue)
    {
        FileNameWithByteDic = pathListValue;
        AddressableDic = addressableDicValue;
    }
}