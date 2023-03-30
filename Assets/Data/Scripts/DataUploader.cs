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

    private string bucketName;
    private string version;
    private bool setCurrentVersion;
    private string localJsonDataPath;

    private bool initialized = false;

    private string AppSpot
    {
        get
        {
            return $"gs://{bucketName}.appspot.com";
        }
    }

    private string CurrentVersionPath
    {
        get
        {
            return "CurrentVersion/version.txt";
        }
    }
    
    private string JsonDatasPath
    {
        get
        {
            return $"JsonDatas/{version}/";
        }
    }

    public void Initialize(string localJsonPathValue, string bucketNameValue, string versionValue, bool setCurrentVersionValue)
    {
        localJsonDataPath = localJsonPathValue;
        bucketName = bucketNameValue;
        version = versionValue;
        setCurrentVersion = setCurrentVersionValue;

        storage = FirebaseStorage.GetInstance(AppSpot);

        initialized = true;
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
        if (setCurrentVersion)
        {
            byte[] versionBytes = Encoding.UTF8.GetBytes(version);
            StorageReference storageReference = storage.RootReference.Child(CurrentVersionPath);
            Task<StorageMetadata> task = storageReference.PutBytesAsync(versionBytes);
            yield return new WaitUntil(() => task.IsCompleted);

            ShowTaskLog("version.txt", ref task);
        }

        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string fileName = Path.GetFileName(filePath);

            byte[] fileBytes = File.ReadAllBytes(filePath);
            StorageReference storageReference = storage.RootReference.Child(JsonDatasPath + fileName);
            Task<StorageMetadata> task = storageReference.PutBytesAsync(fileBytes);
            yield return new WaitUntil(() => task.IsCompleted);

            ShowTaskLog(fileName, ref task);
        }

        storage.App.Dispose();
        DestroyImmediate(gameObject);
    }

    private void ShowTaskLog(string fileName, ref Task<StorageMetadata> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("업로드 실패 : " + fileName + " - " + task.Exception);
        }
        else
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
