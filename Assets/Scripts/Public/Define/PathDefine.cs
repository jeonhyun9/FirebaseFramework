public class PathDefine
{
    #region const
    public const string Excel = "Assets/Data/Excels";
    public const string Json = "Assets/Data/Jsons";
    public const string DataStruct = "Assets/Data/Scripts/Structs";
    public const string Manager = "Assets/Scripts/Manager/";
    public const string DataManager = "Assets/Scripts/Manager/DataManager.cs";
    public const string VersionText = "Assets/Data/Jsons/Version.txt";
    public const string JsonListText = "Assets/Data/Jsons/JsonList.txt";
    public const string DefaultPrefabPath = "Prefab";
    public const string PrefabAddressableFullPath = "Assets/Addressable/Prefab";
    public const string ContentsScriptsFolderPath = "Assets/Scripts/Contents";
    public const string Addressable = "Assets/Addressable";
    public const string AddressablePathJson = "Assets/Addressable/AddressablePath.json";
    public const string DefinePath = "Assets/Scripts/Public/Define";
    public const string EditorWindowPath = "Assets/Editor/Window";
    #endregion

    #region property
    public static string AddressableBuildPathByPlatform
    {
        get
        {
#if UNITY_STANDALONE
            return "Addressable/StandaloneWindows64";
#elif UNITY_ANDROID
            return "Addressable/Android";
#else
            return "Addressable";
#endif
        }
    }

    public static string AddressableLoadPath
    {
        get
        {
            return $"{UnityEngine.Application.persistentDataPath}/{AddressableBuildPathByPlatform}";
        }
    }

    public static string AddressableBinPath
    {
        get
        {
            return $"Assets/AddressableAssetsData/{NameDefine.CurrentPlatformName}/addressables_content_state.bin";
        }
    }
#endregion
}

public class TemplatePathDefine
{
    public const string TemplatePath = "Assets/Templates/";

    public const string StartDataTemplate = "Assets/Templates/StartDataTemplate.txt";
    public const string EndDateTemplate = "Assets/Templates/EndDataTemplate.txt";
    public const string DataValueTemplate = "Assets/Templates/DataValueTemplate.txt";
    public const string StructValueTemplate = "Assets/Templates/StructValueTemplate.txt";
    public const string DataManagerTemplate = "Assets/Templates/DataManagerTemplate.txt";
    public const string AddContainerTypeTemplate = "Assets/Templates/AddContainerTypeTemplate.txt";
    public const string MVC_ControllerTemplate = "Assets/Templates/MVC_ControllerTemplate.txt";
    public const string MVC_ViewModelTemplate = "Assets/Templates/MVC_ViewModelTemplate.txt";
    public const string MVC_ViewTemplate = "Assets/Templates/MVC_ViewTemplate.txt";
    public const string UnitTemplate = "Assets/Templates/UnitTemplate.txt";
    public const string UnitModelTemplate = "Assets/Templates/UnitModelTemplate.txt";
    public const string ManagerTemplate = "Assets/Templates/ManagerTemplate.txt";
    public const string MonoManagerTemplate = "Assets/Templates/MonoManagerTemplate.txt";
    public const string UITypeTemplate = "Assets/Templates/UITypeTemplate.txt";
    public const string EditorWindowTemplate = "Assets/Templates/EditorWindowTemplate.txt";
    
}

