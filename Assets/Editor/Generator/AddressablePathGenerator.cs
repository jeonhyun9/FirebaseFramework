using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Tools;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressablePathGenerator : BaseGenerator
{
    private Dictionary<Type, Dictionary<string, string>> addressableDic = new Dictionary<Type, Dictionary<string, string>>();

    public void Generate(string addresableAssetPath)
    {
        Logger.Log(addresableAssetPath);

        AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

        string[] guids = AssetDatabase.FindAssets("t:object", new string[] { addresableAssetPath });

        ClearNotUseEntries(addressableSettings, guids);

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (assetType == typeof(DefaultAsset))
                continue;

            if (assetType == typeof(SceneAsset))
                assetType = typeof(UnityEngine.SceneManagement.Scene);

            Logger.Log($"{assetType}");

            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            AddressableAssetGroup targetGroup = null;
            string groupName = assetType.Name;

            foreach (var group in addressableSettings.groups)
            {
                if (group.Name == groupName)
                {
                    targetGroup = group;
                    break;
                }
            }

            if (targetGroup == null)
                targetGroup = addressableSettings.CreateGroup(groupName, false, false, false, null);

            AddressableAssetEntry entry = addressableSettings.CreateOrMoveEntry(guid, targetGroup);

            if (entry != null)
            {
                if (entry.address != guid)
                    entry.address = guid;

                if (!addressableSettings.GetLabels().Contains(groupName))
                    addressableSettings.AddLabel(groupName);

                entry.SetLabel(groupName, true);
            }

            if (!addressableDic.ContainsKey(assetType))
                addressableDic.Add(assetType, new Dictionary<string, string>());

            if (!addressableDic[assetType].ContainsKey(assetName))
            {
                addressableDic[assetType].Add(assetName, guid);
            }
            else
            {
                Logger.Error($"Duplicate name... {assetName}");
                continue;
            }
        }

        EditorUtility.SetDirty(addressableSettings);
        AssetDatabase.Refresh();

        string json = JsonConvert.SerializeObject(addressableDic);

        SaveFileAtPath(addresableAssetPath, NameDefine.AddressablePathName, json);
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
}
