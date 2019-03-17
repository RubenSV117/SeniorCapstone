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
    private FirebaseStorage storageReference;
    private StorageReference storageFolderReference;
    private Uri uri;
    

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

    public void GetSprite(string itemPath, Image image)
    {
        print($"StorageManager Requesting image at {itemPath}");

        // get reference to the itemPath
        StorageReference reference = storageReference.GetReferenceFromUrl(itemPath);

        uri = null;
        StartCoroutine(GetImage(image));

        reference.GetDownloadUrlAsync().ContinueWith((task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                print($"URI {task.Result}");
                uri = task.Result;
            }

            else
            {
                print($"StorageManager Requesting image Failed");
            }
        });

        //Task task = reference.GetDownloadUrlAsync().ContinueWith(task2 =>
        //    {
        //        print($"StorageManager Starting Coroutine");
        //        StartCoroutine(GetImage(task2, image));

        //        Debug.Log("Download URL: " + task2.ToString());

        //    })
        //.WithFailure((FirebaseException exception) =>
        //    {
        //        print($"StorageManager Requesting image Failed");

        //    });

    }

    #endregion

    #region Private Methods

    private IEnumerator GetImage(Image image)
    {
        print("In Coroutine");

        yield return new WaitWhile(() => uri == null);

        WWW request = new WWW(uri.ToString());

        print($"StorageManager waiting for image at {uri}");

        yield return request;

        print($"StorageManager Done Waiting");


        //if (request.isNetworkError || request.isHttpError)
        //{
        //    Debug.Log(request.error);
        //    print($"StorageManager request Failed");

        //    yield break;
        //}

        Sprite newSprite = Sprite.Create(request.texture, new Rect(Vector2.zero, new Vector2(request.texture.width, request.texture.height)), new Vector2(0.5f, 0.5f));
        image.sprite = newSprite;
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
