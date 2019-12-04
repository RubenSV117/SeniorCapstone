using System;
using UnityEngine;
using System.Collections;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// References StorageManager to create a call to Fire Storage an get an image at the given path
/// </summary>
public class ImageRequest : MonoBehaviour
{
    public string itemPath;
    public Image image;

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
        if (string.IsNullOrWhiteSpace(itemPath))
        {
            print("No imagepath so using black texture");
            var black = Texture2D.blackTexture;
            image.sprite = Sprite.Create(black, new Rect(Vector2.zero, new Vector2(black.width, black.height)),
                new Vector2(0.5f, 00.5f));
            Destroy(gameObject);
            return;
        }

        // get reference to the itemPath
        StorageReference reference = StorageManager.storageReference.GetReferenceFromUrl(itemPath);
        reference.GetDownloadUrlAsync().ContinueWithOnMainThread((task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                print($"URI {task.Result}");
                var request = UnityWebRequestTexture.GetTexture(task.Result);
                var operation = request.SendWebRequest();
                operation.completed += asyncOperation =>
                {
                    var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                    try
                    {
                        image.sprite = Sprite.Create(texture,
                        new Rect(Vector2.zero, new Vector2(texture.width, texture.height)),
                        new Vector2(0.5f, 0.5f));
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e);
                    }
                    Destroy(gameObject);
                };

            }
            else
            {
                print($"StorageManager Requesting image Failed");
            }
        });
    }
}