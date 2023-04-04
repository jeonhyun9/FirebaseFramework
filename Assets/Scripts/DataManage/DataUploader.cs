#if UNITY_EDITOR
using Firebase.Storage;
using System.Collections;
using System.IO;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

//Editor ������ ������ ���� �߻�!

/// <summary>
/// ���� ����� FireBaseStorage�� ���� ��Ģ�� �б�/���� ������ ��ο��� ���Ǿ� �ִ�.
/// ������ ����� ���� �� ���� ��Ģ�� �����ϰ�, ���ε� �� ���� �ܰ踦 �߰��ؾ��Ѵ�.
/// </summary>
public class DataUploader : MonoBehaviour
{
    private FirebaseStorage storage;
    private EditorCoroutine editorCoroutine;

    private bool setCurrentVersion;
    private string localJsonDataPath;

    private bool initialized = false;
    private FireBaseDefine fireBaseDef;

    private string JsonListTextName => Path.GetFileName(PathDefine.JsonListText);
    private string VersionTextName => Path.GetFileName(PathDefine.VersionText);

    public bool Initialize(string localJsonPathValue, string bucketNameValue, bool setCurrentVersionValue)
    {
        localJsonDataPath = localJsonPathValue;
        string versionValue = File.ReadAllText(Path.Combine(localJsonDataPath, VersionTextName));

        if (string.IsNullOrEmpty(versionValue))
        {
            Debug.LogError("Version ������ ������ �� �����ϴ�.");
            return false;
        }

        fireBaseDef = new FireBaseDefine(bucketNameValue, versionValue);

        setCurrentVersion = setCurrentVersionValue;

        storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);

        initialized = true;

        return true;
    }

    public void StartUpload()
    {
        if (!initialized)
        {
            Debug.LogError("DataUploader�� �ʱ�ȭ ���� ����.");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
    }

    private IEnumerator UploadJsonDatas()
    {
        float progress = 0f;

        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Debug.LogError("Json������ �����ϴ�.");
            yield break;
        }

        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} ���ε� ��..", progress);

            yield return UploadTask(jsonPath, fireBaseDef.GetJsonPath(jsonName));
            progress += 1f / jsonFiles.Length;
        }

        EditorUtility.DisplayProgressBar("JsonList.txt", $"JsonList.txt ���ε� ��..", progress);

        yield return UploadTask(Path.Combine(localJsonDataPath, JsonListTextName), fireBaseDef.JsonListPath);

        if (setCurrentVersion)
        {
            EditorUtility.DisplayProgressBar("Version.txt", $"Version.txt ���ε� ��..", progress);
            yield return UploadTask(Path.Combine(localJsonDataPath, VersionTextName), fireBaseDef.CurrentVersionPath);

            Debug.Log($"Upload Current Version {fireBaseDef.Version}");
        }

        EditorUtility.ClearProgressBar();

        if (storage?.App != null)
            storage.App.Dispose();

        DestroyImmediate(gameObject);
    }

    //storagePath�� Path.Combine ����ϸ� �ȵ�
    private IEnumerator UploadTask(string localPath, string storagePath)
    {
        byte[] bytes = File.ReadAllBytes(localPath);
        StorageReference storageRef = storage.RootReference.Child(storagePath);
        Task<StorageMetadata> task = storageRef.PutBytesAsync(bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError($"Upload Fail : {Path.GetFileName(localPath)} - {task.Exception}");
        }
        else if (task.IsCompleted)
        {
            Debug.Log($"Upload Success : {Path.GetFileName(localPath)}");
        }
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
#endif
