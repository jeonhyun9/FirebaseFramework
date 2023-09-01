using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct AddressableBuildInfo
{
    [JsonProperty]
    private Dictionary<string, byte[]> fileNameWithHashDic;

    [JsonProperty]
    private Dictionary<Type, Dictionary<string, string>> addressableDic;

    public IReadOnlyDictionary<string, byte[]> FileNameWithHashDic => fileNameWithHashDic;
    public IReadOnlyDictionary<Type, Dictionary<string, string>> AddressableDic => addressableDic;

    public AddressableBuildInfo(Dictionary<string, byte[]> pathListValue,
        Dictionary<Type, Dictionary<string, string>> addressableDicValue)
    {
        fileNameWithHashDic = pathListValue;
        addressableDic = addressableDicValue;
    }
}