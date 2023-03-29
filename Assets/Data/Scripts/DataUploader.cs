using Firebase.Storage;
using System.IO;
using UnityEngine;

public class DataUploader : MonoBehaviour
{
    private FirebaseStorage storage;

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
    }
}
