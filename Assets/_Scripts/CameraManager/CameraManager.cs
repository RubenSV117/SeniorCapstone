using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

/// <summary>
/// Manages Native Camera functionalities
///
/// Ruben Sanchez
/// </summary>
public class CameraManager : MonoBehaviour
{
    public delegate void CameraEvent(Sprite sprite);
    public static event CameraEvent OnPictureTaken;
    public static event CameraEvent OnCameraRollPictureChosen;

    public static string PathOfCurrentImage { get; private set; }
    
    private void OnEnable()
    {
        NativeToolkit.OnImagePicked += OnImagePicked;
    }

    private void OnDisable()
    {
        NativeToolkit.OnImagePicked -= OnImagePicked;
    }

    public void TakePicture()
    {
        NativeCamera.TakePicture(TakePictureCallback, 1024);
    }

    private void TakePictureCallback(string path)
    {


        if (path != null)
        {
            // Create a Texture2D from the captured image in the cache
            Texture2D texture = NativeCamera.LoadImageAtPath(path, 1024);

            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + path);
                return;
            }

            // create sprite from the texture
            Sprite newSprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(.5f, .5f));

            if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                NotificationManager.Instance.ShowNotification("External Write Permission Required");
                return;
            }

            PathOfCurrentImage = NativeToolkit.SaveImage(texture, "Recipes");

            print($"CameraManager: Path of Image {PathOfCurrentImage}");

            OnPictureTaken?.Invoke(newSprite);
        }
    }

    public void RequestWritePermission()
    {
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);

    }


    public void RequestReadPermission()
    {
        Permission.RequestUserPermission(Permission.ExternalStorageRead);

    }

    public void ChoosePicture()
    {
        NativeToolkit.PickImage();
    }

    public void OnImagePicked(Texture2D texture, string path)
    {
        PathOfCurrentImage = path;

        print($"CameraManager: Path of Image {PathOfCurrentImage}");

        // create sprite from the texture
        Sprite newSprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(.5f, .5f));

        OnCameraRollPictureChosen?.Invoke(newSprite);
    }
}
