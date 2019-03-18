using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Storage;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    #region Public Variables

    public static StorageManager Instance;

    #endregion

    #region Private Variables

    private bool attemptDone;
    public static FirebaseStorage storageReference { get; private set; }
    private StorageReference storageFolderReference;

    #endregion

    #region Unity

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Get root storage instance reference
        storageReference = Firebase.Storage.FirebaseStorage.DefaultInstance;

        // Point to the root reference
        storageFolderReference = storageReference.GetReferenceFromUrl("gs://regen-66cf8.appspot.com/Recipes");

    }
    #endregion

    #region Public Methods

    public string GetReference(string pathAppend)
    {
        // Points to itemPath
        StorageReference space_ref = storageFolderReference.Child(pathAppend);
        return space_ref.ToString();
    }

  

    private void PublishImageToStorage(string localFile, string key)
    {
        // Create a reference to the file you want to upload
        StorageReference imageReference = storageFolderReference.Child($"{key}.jpg");

        // Upload the file to the itemPath
        imageReference.PutFileAsync(localFile)
            .ContinueWith((task) =>
            {
                // error uploading
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                }

                // uploaded successfuly
                else
                {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    Firebase.Storage.StorageMetadata metadata = task.Result;
                    imageReference.GetMetadataAsync().ContinueWith((task2) =>
                    {
                        string downloadUrl = task2.ToString();

                        Debug.Log("Finished uploading...");
                        Debug.Log("download url = " + downloadUrl);
                    });
                }
            });
    }
#endregion

   
    
}
