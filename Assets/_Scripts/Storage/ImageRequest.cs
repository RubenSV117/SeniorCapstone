using System;
using UnityEngine;
using System.Collections;
using Firebase.Storage;
using UnityEngine.UI;

/// <summary>
/// References StorageManager to create a call to Fire Storage an get an image at the given path
/// </summary>
public class ImageRequest : MonoBehaviour
{
    public string itemPath;
    public Image image;
    private Uri uri;

    /// <summary>
    /// Link the image to initialize to the path to retrieve from 
    /// </summary>
    /// <param name="itemPath">The item path for the image in Firebase Storage</param>
    /// <param name="image">The UI image to be initialized with the retrieved data</param>
    public void Init(string itemPath, Image image)
    {
        this.itemPath = itemPath;
        this.image = image;

        GetSprite();
    }

    public void GetSprite()
    {
        Debug.Log($"StorageManager Requesting image at {itemPath}");

        // get reference to the itemPath
        StorageReference reference = StorageManager.storageReference.GetReferenceFromUrl(itemPath);

        uri = null;

        // launch coroutine to wait for the Uri to be set 
        StartCoroutine(GetImage(image));

        reference.GetDownloadUrlAsync().ContinueWith((task) =>
        {
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
    }

    private IEnumerator GetImage(Image image)
    {
        // wait until the Uri has been set
        yield return new WaitWhile(() => uri == null);

        // retrieve the image
        WWW request = new WWW(uri.ToString());

        yield return request;

        if (request.texture == null)
            yield break;

        // create a sprite with the data retrieved and set the image to it
        Sprite newSprite = Sprite.Create(request.texture,
            new Rect(Vector2.zero, new Vector2(request.texture.width, request.texture.height)),
            new Vector2(0.5f, 0.5f));

        image.sprite = newSprite;

        Destroy(gameObject);
    }
}