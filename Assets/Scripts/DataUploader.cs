using Firebase.Storage;
using System.Collections;
using System.IO;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 현재 연결된 FireBaseStorage의 보안 규칙의 읽기/쓰기 권한이 모두에게 허용되어 있다.
/// 실제로 사용할 때는 꼭 보안 규칙을 수정하고, 업로드 전 인증 단계를 추가해야한다.
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
            Debug.LogError("Version 정보를 가져올 수 없습니다.");
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
            Debug.LogError("DataUploader가 초기화 되지 않음.");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
    }

    private IEnumerator UploadJsonDatas()
    {
        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Debug.LogError("Json파일이 없습니다.");
            yield break;
        }

        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            //파이어베이스 경로는 Path.Combine X
            yield return UploadTask(jsonPath, fireBaseDef.JsonDatasPath + jsonName);
        }

        yield return UploadTask(Path.Combine(localJsonDataPath, JsonListTextName), fireBaseDef.JsonListPath);

        if (setCurrentVersion)
            yield return UploadTask(Path.Combine(localJsonDataPath, VersionTextName), fireBaseDef.CurrentVersionPath);

        if (storage?.App != null)
            storage.App.Dispose();

        DestroyImmediate(gameObject);
    }

    private IEnumerator UploadTask(string localPath, string storagePath)
    {
        byte[] bytes = File.ReadAllBytes(localPath);
        StorageReference storageRef = storage.RootReference.Child(storagePath);
        Task<StorageMetadata> task = storageRef.PutBytesAsync(bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        ShowTaskLog(Path.GetFileName(localPath), task);
    }

    private void ShowTaskLog(string fileName, Task<StorageMetadata> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("업로드 실패 : " + fileName + " - " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            Debug.Log("업로드 완료 : " + fileName);
        }
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
