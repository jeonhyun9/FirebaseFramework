using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class AddressableManager : BaseManager<AddressableManager>
{
    public enum Sequence
    {
        LoadAddressableVersion,
        LoadAddressableBuildInfo,
        LoadAddressableBuild,
        Done,
    }

    public Sequence CurrentSequence { get; private set; } = Sequence.LoadAddressableVersion;

    private string loadPath = PathDefine.AddressableLoadPath;

    private AddressableBuildInfo addressableBuildInfo;
    private FireBaseStorage fireBaseStorage;

    public async UniTask<bool> LoadAddressableAsync(Action onChangeSequenceCallback = null)
    {
        fireBaseStorage = new FireBaseStorage(NameDefine.BucketDefaultName);

        while(CurrentSequence != Sequence.Done)
        {
            if (onChangeSequenceCallback != null)
                onChangeSequenceCallback.Invoke();

            bool result = await GetLoadAddressableSequence();

            if (result)
            {
                CurrentSequence++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public string CurrentSequenceMessage
    {
        get
        {
            string prefix = $"[Version:{fireBaseStorage.AddressableVersion}]";
            string suffix = $"{(int)CurrentSequence + 1} / {(int)Sequence.Done + 1}";

            string message = null;

            switch (CurrentSequence)
            {
                case Sequence.LoadAddressableVersion:
                    message = "Loading Addressable Version..";
                    break;

                case Sequence.LoadAddressableBuildInfo:
                    message = "Loading Addressable Build Info..";
                    break;

                case Sequence.LoadAddressableBuild:
                    message = "Loading Addressable Build..";
                    break;
            }

            return $"{prefix} {message} {suffix}";
        }
    }

    private async UniTask<bool> GetLoadAddressableSequence()
    {
        switch (CurrentSequence)
        {
            case Sequence.LoadAddressableVersion:
                return await LoadAddressableVersion();

            case Sequence.LoadAddressableBuildInfo:
                return await LoadAddressableBuildInfo();

            case Sequence.LoadAddressableBuild:
                return await LoadAddressableBuild();
        }

        return false;
    }

    public async UniTask DownLoadDependenciesAsUniTask(string guid)
    {
        await Addressables.DownloadDependenciesAsync(guid);
    }

    public async UniTask<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
    {
        if (!IsContain(typeof(T), name))
            return null;

        return await Addressables.LoadAssetAsync<T>(addressableBuildInfo.AddressableDic[typeof(T)][name]);
    }

    public async UniTask LoadSceneAsync(SceneType sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        string name = sceneName.ToString();

        if (!IsContain(typeof(Scene), name))
            return;

        await Addressables.LoadSceneAsync(addressableBuildInfo.AddressableDic[typeof(Scene)][name], loadSceneMode);
    }

    public async UniTask<GameObject> InstantiateAsync(string name, Transform transform = null)
    {
        if (!IsContain(typeof(GameObject), name))
            return null;

        return await Addressables.InstantiateAsync(addressableBuildInfo.AddressableDic[typeof(GameObject)][name], transform);
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

            Logger.Log($"fileCount : {addressableBuildInfo.FileNameWithByteDic.Count}");
            return true;
        }

        return false;
    }

    private async UniTask<bool> LoadAddressableBuild()
    {
        if (!Directory.Exists(loadPath))
            Directory.CreateDirectory(loadPath);

        try
        {
            List<UniTask> tasks = new List<UniTask>();

            foreach (string fileName in addressableBuildInfo.FileNameWithByteDic.Keys)
                tasks.Add(LoadAddressableBuildFileAsync(fileName, loadPath));

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

        byte[] originFileByte = addressableBuildInfo.FileNameWithByteDic[fileName];

        if (File.Exists(fullPath))
        {
            byte[] localFileByte = null;

            await UniTask.RunOnThreadPool(() => { localFileByte = File.ReadAllBytes(fullPath); });

            if (localFileByte.IntegrityCheck(originFileByte))
            {
                Logger.Success($"[Local] Load Addressable Build : {fullPath}");
                return;
            }
        }

        StorageReference fileRef = fireBaseStorage.GetStoragePath(fireBaseStorage.GetAddressableBuildStoragePath(fileName));
        byte[] loadedFileByte = await fireBaseStorage.LoadBytes(fileRef);

        await File.WriteAllBytesAsync(fullPath, loadedFileByte);

        Logger.Success($"[New] Load Addressable Build : {fullPath}");
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
