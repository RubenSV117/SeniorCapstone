using System;
using UnityEngine;
using System.Collections;
using Firebase.Storage;
using UnityEngine.UI;

public class ImageRequest : MonoBehaviour
{
    public string itemPath;
    public Image image;
    private Uri uri;

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
        print("In Coroutine");

        yield return new WaitWhile(() => uri == null);

        WWW request = new WWW(uri.ToString());

        print($"StorageManager waiting for image at {uri}");

        yield return request;

        print($"StorageManager Done Waiting");

        Sprite newSprite = Sprite.Create(request.texture,
            new Rect(Vector2.zero, new Vector2(request.texture.width, request.texture.height)),
            new Vector2(0.5f, 0.5f));
        image.sprite = newSprite;

        Destroy(gameObject);
    }
}