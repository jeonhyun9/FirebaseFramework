#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Tools;

public class AddressablePathEditorWindow : BaseEdtiorWindow
{
    private const float width = 400f;
    private const float height = 200f;
    private const float spacing = 5f;

    private AddressablePathGenerator addressablePathGenerator = new AddressablePathGenerator();

    private string AdressableAssetPath => GetParameter<string>("AddressablePath");

    [MenuItem("Tools/Addressable Path Generator")]
    public static void OpenAddresablePathGenerator()
    {
        AddressablePathEditorWindow window = (AddressablePathEditorWindow)GetWindow(typeof(AddressablePathEditorWindow));
        window.InitializeWindow(window, width, height, spacing);
    }

    protected override void InitializeParameters()
    {
        AddParameter("AddressablePath", PathDefine.AddressablePathJson);
    }

    protected override void DrawActionButton()
    {
        if (GUILayout.Button("Generate Addressable Path"))
        {
            if (addressablePathGenerator != null)
            {
                addressablePathGenerator.Generate(AdressableAssetPath);
                Close();
            }
        }
    }
}
#endif