using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableManager : BaseManager<AddressableManager>
{
    private AddressableList addressableList;

    public void Initialize(string json)
    {
        //addressableDic = JsonConvert.DeserializeObject<Dictionary<Type, Dictionary<string, string>>>(json);
    }

    public async UniTask DownloadAllAssetsAsync()
    {
        foreach(Type type in addressableList.AddressableDic.Keys)
        {
            foreach(string key in addressableList.AddressableDic[type].Keys)
                await Addressables.DownloadDependenciesAsync(key);
        }
    }

    public async UniTask<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
    {
        if (!IsContain(typeof(T), name))
            return null;

        return await Addressables.LoadAssetAsync<T>(addressableList.AddressableDic[typeof(T)][name]);
    }

    public async UniTask<GameObject> InstantiateAsync(string name)
    {
        if (!IsContain(typeof(GameObject), name))
            return null;

        return await Addressables.InstantiateAsync(addressableList.AddressableDic[typeof(GameObject)][name]);
    }

    private bool IsContain(Type type, string name)
    {
        Logger.Log(Addressables.RuntimePath);

        if (!addressableList.AddressableDic.ContainsKey(type))
        {
            Logger.Error($"{type} is not contained in addressable.");
            return false;
        }

        if (!addressableList.AddressableDic[type].ContainsKey(name))
        {
            Logger.Error($"{name} is not contained in addressable.");
            return false;
        }

        return true;
    }
}
