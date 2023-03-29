using Firebase.Storage;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataDownloader : MonoBehaviour
{
    private FirebaseStorage storage;

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
    }
}
