#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Tools;
using UnityEditor.AddressableAssets;

public class AddressableBuildGeneratorEditorWindow : BaseEdtiorWindow
{
    private const float width = 400f;
    private const float height = 200f;
    private const float spacing = 5f;

    private AddressableBuildGenerator addressableBuildGenerator = new();

    private string AdressableAssetPath => GetParameter<string>("AddressableAssetPath");

    [MenuItem("Tools/Addressable/Addressable Build Generator")]
    public static void OpenAddresableBuildGenerator()
    {
        AddressableBuildGeneratorEditorWindow window = (AddressableBuildGeneratorEditorWindow)GetWindow(typeof(AddressableBuildGeneratorEditorWindow));
        window.InitializeWindow(window, width, height, spacing);
    }

    protected override void InitializeParameters()
    {
        AddLabel($"Current Addressable Build Path : {addressableBuildGenerator.GetLocalBuildPath()}");

        AddParameter("AddressableAssetPath", PathDefine.Addressable);
    }

    protected override void DrawActionButton()
    {
        if (GUILayout.Button("Generate Addressable Build"))
        {
            if (addressableBuildGenerator != null)
            {
                addressableBuildGenerator.Generate(AdressableAssetPath);
                Close();
            }
        }
    }
}
#endif