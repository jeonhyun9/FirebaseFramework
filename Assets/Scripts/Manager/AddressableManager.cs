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

    private AddressableBuildInfo addressableBuildInfo;
    private FireBaseStorage fireBaseStorage;

    private bool clearBundle;

    #region Initialize Addressable
    public async UniTask<bool> LoadAddressableAsync(Action onChangeSequenceCallback = null)
    {
        clearBundle = false;
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

    private async UniTask<bool> LoadAddressableVersion()
    {
        string currentVersion = await fireBaseStorage.LoadString(fireBaseStorage.CurrentAddressableVersionStoragePath);

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
        string addressableBuildInfoJson = await fireBaseStorage.LoadString(fireBaseStorage.AddressableBuildInfoStoragePath);

        if (!string.IsNullOrEmpty(addressableBuildInfoJson))
        {
            addressableBuildInfo = JsonConvert.DeserializeObject<AddressableBuildInfo>(addressableBuildInfoJson);

            Logger.Log($"fileCount : {addressableBuildInfo.FileNameWithHashDic.Count}");
            return true;
        }

        return false;
    }

    private async UniTask CleanOldBuild()
    {
        HashSet<string> fileNames = new HashSet<string>(addressableBuildInfo.FileNameWithHashDic.Keys);

        string[] existFilesPath = Directory.GetFiles(PathDefine.AddressableLoadPath);

        foreach (string filePath in existFilesPath)
        {
            string fileName = Path.GetFileName(filePath);
            if (!fileNames.Contains(fileName))
            {
                Logger.Log($"Removed : {fileName}");
                File.Delete(filePath);

                if (clearBundle == false)
                {
                    try
                    {
                        Caching.ClearCache();
                        await Addressables.CleanBundleCache();
                    }
                    catch (Exception e)
                    {
                        Logger.Exception("ClearBundle", e);
                    }
                    finally
                    {
                        clearBundle = true;
                    }
                }
            }
        }
    }

    private async UniTask<bool> LoadAddressableBuild()
    {
        if (!Directory.Exists(PathDefine.AddressableLoadPath))
            Directory.CreateDirectory(PathDefine.AddressableLoadPath);

        try
        {
            await CleanOldBuild();

            List<UniTask> tasks = new List<UniTask>();

            foreach (string fileName in addressableBuildInfo.FileNameWithHashDic.Keys)
                tasks.Add(LoadAddressableBuildFileAsync(fileName, PathDefine.AddressableLoadPath));

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

        byte[] originFileHash = addressableBuildInfo.FileNameWithHashDic[fileName];

        if (File.Exists(fullPath))
        {
            byte[] localFileByte = null;

            await UniTask.RunOnThreadPool(() => { localFileByte = File.ReadAllBytes(fullPath); });

            byte[] localHash = localFileByte.GetSHA256();

            if (localHash.SequenceEqual(originFileHash))
            {
                Logger.Success($"[Local] Load Addressable Build : {fullPath}");
                return;
            }
        }

        byte[] loadedFileByte = await fireBaseStorage.LoadBytes(fireBaseStorage.GetAddressableBuildStoragePath(fileName));

        await File.WriteAllBytesAsync(fullPath, loadedFileByte);

        Logger.Success($"[New] Load Addressable Build : {fullPath}");
    }

    #endregion

    private bool IsContain(Type type, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.Null($"Addressable Name");
            return false;
        }

        if (!addressableBuildInfo.AddressableDic.ContainsKey(type))
        {
            Logger.Error($"{type} type is not contained in addressable.");
            return false;
        }

        if (!addressableBuildInfo.AddressableDic[type].ContainsKey(name))
        {
            Logger.Error($"{name} name is not contained in addressable.");
            return false;
        }

        return true;
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
}
