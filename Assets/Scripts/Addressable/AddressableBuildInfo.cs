using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct AddressableBuildInfo
{
    [JsonProperty]
    private Dictionary<string, byte[]> fileNameWithByteDic;

    [JsonProperty]
    private Dictionary<Type, Dictionary<string, string>> addressableDic;

    public IReadOnlyDictionary<string, byte[]> FileNameWithByteDic => fileNameWithByteDic;
    public IReadOnlyDictionary<Type, Dictionary<string, string>> AddressableDic => addressableDic;

    public AddressableBuildInfo(Dictionary<string, byte[]> pathListValue,
        Dictionary<Type, Dictionary<string, string>> addressableDicValue)
    {
        fileNameWithByteDic = pathListValue;
        addressableDic = addressableDicValue;
    }
}