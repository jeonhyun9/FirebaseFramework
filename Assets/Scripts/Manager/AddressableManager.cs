using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableManager : BaseStaticManager<AddressableManager>
{
    private AddressableBuildInfo addressableBuildInfo;
    private FireBaseStorage fireBaseStorage;

    public async UniTask<bool> LoadAddressableAsync()
    {
        fireBaseStorage = new FireBaseStorage(NameDefine.BucketDefaultName);

        if (await LoadAddressableVersion() == false)
            return false;

        if (await LoadAddressableBuildInfo() == false)
            return false;

        if (await LoadAddressableBuild() == false)
            return false;

        await Addressables.InitializeAsync();

        await DownloadAllAssetsAsync();

        return true;
    }

    private async UniTask DownloadAllAssetsAsync()
    {
        if (addressableBuildInfo.AddressableDic == null)
            return;

        foreach(Type type in addressableBuildInfo.AddressableDic.Keys)
        {
            foreach(string guids in addressableBuildInfo.AddressableDic[type].Values)
                await Addressables.DownloadDependenciesAsync(guids);
        }
    }

    public async UniTask<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
    {
        if (!IsContain(typeof(T), name))
            return null;

        return await Addressables.LoadAssetAsync<T>(addressableBuildInfo.AddressableDic[typeof(T)][name]);
    }

    public async UniTask LoadSceneAsync(SceneState sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        string name = sceneName.ToString();

        if (!IsContain(typeof(Scene), name))
            return;

        await Addressables.LoadSceneAsync(addressableBuildInfo.AddressableDic[typeof(Scene)][name], loadSceneMode);
    }

    public async UniTask<GameObject> InstantiateAsync(string name)
    {
        if (!IsContain(typeof(GameObject), name))
            return null;

        return await Addressables.InstantiateAsync(addressableBuildInfo.AddressableDic[typeof(GameObject)][name]);
    }

    private async UniTask<bool> LoadAddressableVersion()
    {
        StorageReference versionRef = fireBaseStorage.GetStoragePath(fireBaseStorage.CurrentAddressableVersionStoragePath);

        string currentVersion = await fireBaseStorage.LoadString(versionRef);

        if (!string.IsNullOrEmpty(currentVersion))
        {
            fireBaseStorage.SetAddressableVersion(currentVersion);

            Logger.Success($"Load Addressable Build Info Version : {currentVersion}");
            return true;
        }

        return false;
    }

    private async UniTask<bool> LoadAddressableBuildInfo()
    {
        StorageReference addressableBuildInfoRef = fireBaseStorage.GetStoragePath(fireBaseStorage.AddressableBuildInfoStoragePath);

        string addressableBuildInfoJson = await fireBaseStorage.LoadString(addressableBuildInfoRef);

        if (!string.IsNullOrEmpty(addressableBuildInfoJson))
        {
            addressableBuildInfo = JsonConvert.DeserializeObject<AddressableBuildInfo>(addressableBuildInfoJson);

            Logger.Success($"Load Addressable Build Info : {addressableBuildInfoJson}");
            Logger.Log($"fileNameCount : {addressableBuildInfo.FileNameWithByteDic.Count}");
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
            List<UniTask> tasks = new List<UniTask>();

            foreach (string fileName in addressableBuildInfo.FileNameWithByteDic.Keys)
                tasks.Add(LoadAddressableBuildFileAsync(fileName, dataPath));

            await UniTask.WhenAll(tasks);

            return true;
        }
        catch (Exception e)
        {
            Logger.Exception($"Fail to Load Addressable Build..", e);
            return false;
        }
    }

    private async UniTask LoadAddressableBuildFileAsync(string fileName, string dataPath)
    {
        string fullPath = $"{dataPath}/{fileName}";

        Logger.Log($"Check Addressable Full path : {fullPath}");

        byte[] originFileByte = addressableBuildInfo.FileNameWithByteDic[fileName];

        if (File.Exists(fullPath))
        {
            byte[] localFileByte = null;

            await UniTask.RunOnThreadPool(() => { localFileByte = File.ReadAllBytes(fullPath); });

            if (localFileByte.IntegrityCheck(originFileByte))
            {
                Logger.Success($"Load Local Addressable Build : {fullPath}");
                return;
            }
        }

        StorageReference fileRef = fireBaseStorage.GetStoragePath(fireBaseStorage.GetAddressableBuildStoragePath(fileName));
        byte[] loadedFileByte = await fireBaseStorage.LoadBytes(fileRef);

        await File.WriteAllBytesAsync(fullPath, loadedFileByte);

        Logger.Success($"Load New Addressable Build : {fullPath}");
    }

    private bool IsContain(Type type, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.Null($"Addressable Name");
            return false;
        }

        if (!addressableBuildInfo.AddressableDic.ContainsKey(type))
        {
            Logger.Error($"{type} is not contained in addressable.");
            return false;
        }

        if (!addressableBuildInfo.AddressableDic[type].ContainsKey(name))
        {
            Logger.Error($"{name} is not contained in addressable.");
            return false;
        }

        return true;
    }
}
