using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableManager : BaseManager<AddressableManager>
{
    private AddressableList addressableList;
    private FireBaseStorage fireBaseStorage;

    public async UniTask<bool> LoadAddressableAsync()
    {
        fireBaseStorage = new FireBaseStorage(NameDefine.BucketDefaultName);

        if (await LoadAddressableVersion() == false)
            return false;

        if (await LoadAddressableList() == false)
            return false;

        if (await LoadAddressableBuild() == false)
            return false;

        await Addressables.InitializeAsync();

        await DownloadAllAssetsAsync();

        return true;
    }

    private async UniTask DownloadAllAssetsAsync()
    {
        if (addressableList.AddressableDic == null)
            return;

        foreach(Type type in addressableList.AddressableDic.Keys)
        {
            foreach(string guids in addressableList.AddressableDic[type].Values)
                await Addressables.DownloadDependenciesAsync(guids);
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

    private async UniTask<bool> LoadAddressableVersion()
    {
        StorageReference versionRef = fireBaseStorage.GetStoragePath(fireBaseStorage.CurrentAddressableVersionStoragePath);

        string currentVersion = await fireBaseStorage.LoadString(versionRef);

        if (!string.IsNullOrEmpty(currentVersion))
        {
            fireBaseStorage.SetAddressableVersion(currentVersion);

            Logger.Success($"Load AddressableList Version : {currentVersion}");
            return true;
        }

        return false;
    }

    private async UniTask<bool> LoadAddressableList()
    {
        StorageReference addressableListRef = fireBaseStorage.GetStoragePath(fireBaseStorage.AddressableListStoragePath);

        string addressableListJson = await fireBaseStorage.LoadString(addressableListRef);

        if (!string.IsNullOrEmpty(addressableListJson))
        {
            addressableList = JsonConvert.DeserializeObject<AddressableList>(addressableListJson);

            Logger.Success($"Load AddressableList List : {addressableListJson}");
            Logger.Log($"fileNameCount : {addressableList.FileNameList.Count}");
            return true;
        }

        return false;
    }

    private async UniTask<bool> LoadAddressableBuild()
    {
        string dataPath = $"{Application.persistentDataPath}/{PathDefine.AddressableBuildPathByFlatform}";

        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        try
        {
            foreach (string fileName in addressableList.FileNameList)
            {
                StorageReference fileRef = fireBaseStorage.GetStoragePath(fireBaseStorage.GetAddressableBuildStoragePath(fileName));
                byte[] fileByte = await fireBaseStorage.LoadBytes(fileRef);

                string fullPath = $"{dataPath}/{fileName}";

                if (Directory.Exists(fullPath) && File.Exists(fullPath))
                {
                    byte[] existingFileBytes = await File.ReadAllBytesAsync(fullPath);

                    if (existingFileBytes.IntegrityCheck(fileByte))
                        continue;
                }

                await File.WriteAllBytesAsync(fullPath, fileByte);

                Logger.Success($"Load Addressable Build : {fullPath}");
            }

            return true;
        }
        catch (Exception e)
        {
            Logger.Exception($"Fail to Load Addressable Build..", e);
            return false;
        }
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
