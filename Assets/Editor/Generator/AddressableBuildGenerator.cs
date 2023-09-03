using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tools;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Linq;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEngine.AddressableAssets.Initialization;

public class AddressableBuildGenerator : BaseGenerator
{
    private Dictionary<Type, Dictionary<string, string>> addressableDic = new Dictionary<Type, Dictionary<string, string>>();

    public void Generate(string addresableAssetPath)
    {
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

        string[] guids = AssetDatabase.FindAssets("t:object", new[] { addresableAssetPath });
        UpdateSettings(addressableSettings, guids);

        string json = JsonConvert.SerializeObject(addressableDic);
        SaveFileAtPath(addresableAssetPath, NameDefine.AddressablePathName, json);

        BuildAddressables();
    }

    private void UpdateSettings(AddressableAssetSettings addressableSettings, string[] guids)
    {
        EditorUtility.SetDirty(addressableSettings);
        ClearNotUseEntries(addressableSettings, guids);

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Type mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (mainAssetType == typeof(DefaultAsset))
                continue;

            if (mainAssetType == typeof(SceneAsset))
                mainAssetType = typeof(UnityEngine.SceneManagement.Scene);

            UpdateEntryForAsset(addressableSettings, guid, mainAssetType, assetPath);
        }

        addressableSettings.OverridePlayerVersion = NameDefine.CurrentPlatformName;

        AssetDatabase.Refresh();
    }

    private void UpdateEntryForAsset(AddressableAssetSettings addressableSettings, string guid, Type mainAssetType, string assetPath)
    {
        AddressableAssetGroup targetGroup = FindOrCreateGroup(addressableSettings, mainAssetType.Name);

        AddressableAssetEntry entry = addressableSettings.CreateOrMoveEntry(guid, targetGroup);

        if (entry == null)
            return;

        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string groupName = mainAssetType.Name;

        if (!addressableSettings.GetLabels().Contains(groupName))
            addressableSettings.AddLabel(groupName);

        if (!entry.labels.Contains(groupName))
            entry.SetLabel(groupName, true);

        entry.SetAddress($"{mainAssetType}_{guid}");

        AddToAddressableDic(mainAssetType, assetName, entry.address);

        #region Check SubAssets

        //씬은 LoadAllAssetsAtPath 사용 시 에러 발생.
        if (entry.MainAssetType == typeof(SceneAsset))
            return;

        Type[] subAssetTypes = AssetDatabase.LoadAllAssetsAtPath(entry.AssetPath)
            .Where(x => x != entry.MainAsset && x.GetType() != entry.MainAsset.GetType())
            .Select(x => x.GetType())
            .ToArray();

        //서브에셋이 하나밖에 없고, 타입이 메인 에셋과 다른 경우에는 이름으로 찾을 수 있도록 한다... EX)싱글 스프라이트
        //이 외에 서브 에셋이 여러개인데 직접 접근해야하는 경우는 거의 없을 것으로 보이지만,
        //꼭 필요하다면 $"{entry.guid}[{assetName}_{index}]" 로 로드할 수 있다.
        if (subAssetTypes.Length == 1)
            AddToAddressableDic(subAssetTypes[0], assetName, $"{entry.guid}[{assetName}]");

        #endregion
    }

    private AddressableAssetGroup FindOrCreateGroup(AddressableAssetSettings addressableSettings, string groupName)
    {
        AddressableAssetGroup targetGroup = addressableSettings.FindGroup(groupName);

        if (targetGroup == null)
        {
            AddressableAssetGroup defaultGroup = addressableSettings.FindGroup(NameDefine.AddressableDefaultGroupName);
            targetGroup = defaultGroup == null ? null : addressableSettings.CreateGroup(groupName, false, false, false, defaultGroup.Schemas);
        }

        return targetGroup;
    }

    private void AddToAddressableDic(Type type, string name, string address)
    {
        if (!addressableDic.ContainsKey(type))
            addressableDic.Add(type, new Dictionary<string, string>());

        if (!addressableDic[type].ContainsKey(name))
        {
            addressableDic[type].Add(name, address);
        }
        else
        {
            Logger.Error($"Duplicate name... {name}");
        }
    }

    private void ClearNotUseEntries(AddressableAssetSettings addressableSettings, string[] guids)
    {
        HashSet<string> guidsHashSet = new HashSet<string>(guids);

        foreach (AddressableAssetGroup group in addressableSettings.groups)
        {
            List<AddressableAssetEntry> removeList = new List<AddressableAssetEntry>();

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (!guidsHashSet.Contains(entry.guid))
                    removeList.Add(entry);
            }

            foreach (AddressableAssetEntry entryToRemove in removeList)
                group.RemoveAssetEntry(entryToRemove);
        }
    }

    private void BuildAddressables()
    {
        string buildPath = GetLocalBuildPath();

        if (Directory.Exists(buildPath))
        {
            string[] files = Directory.GetFiles(buildPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AssetDatabase.Refresh();

        AddressableAssetSettings.BuildPlayerContent();
        AssetDatabase.Refresh();
    }

    public string GetLocalBuildPath()
    {
        string profileId = AddressableAssetSettingsDefaultObject.Settings.activeProfileId;
        var profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
        var localBuildPath = profileSettings.GetValueByName(profileId, "Local.BuildPath");

        string projectPath = Application.dataPath.Replace("/Assets", "");
        string buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
        localBuildPath = localBuildPath.Replace("[BuildTarget]", buildTarget);

        return $"{projectPath}/{localBuildPath}";
    }

    public static void UpdatePreviousAddressablesBuild()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        ContentUpdateScript.BuildContentUpdate(settings, PathDefine.AddressableBinPath);
    }
}
