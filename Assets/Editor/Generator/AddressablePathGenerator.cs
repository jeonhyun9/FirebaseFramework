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
        AddressableAssetGroup defaultGroup = addressableSettings.DefaultGroup;

        string[] guids = AssetDatabase.FindAssets("t:object", new string[] { addresableAssetPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 폴더는 건너뜀
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (assetType == typeof(DefaultAsset))
                continue;

            if (assetType == typeof(SceneAsset))
                assetType = typeof(UnityEngine.SceneManagement.Scene);

            Logger.Log($"{assetType}");

            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            AddressableAssetEntry entry = addressableSettings.CreateOrMoveEntry(guid, defaultGroup);

            if (entry != null)
            {
                if (entry.address != guid)
                    entry.address = guid;
            }

            if (!addressableDic.ContainsKey(assetType))
                addressableDic.Add(assetType, new Dictionary<string, string>());

            if (!addressableDic[assetType].ContainsKey(assetName))
            {
                addressableDic[assetType].Add(assetName, guid);
            }
            else
            {
                Logger.Error($"중복 이름.. {assetName}");
                continue;
            }
        }

        EditorUtility.SetDirty(addressableSettings);
        AssetDatabase.Refresh();

        string json = JsonConvert.SerializeObject(addressableDic);

        SaveFileAtPath(addresableAssetPath, "AddressablePath.txt", json);
    }
}
