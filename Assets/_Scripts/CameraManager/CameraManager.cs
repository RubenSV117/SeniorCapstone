using UnityEngine;

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

    public void TakePicture()
    {
        NativeCamera.TakePicture(TakePictureCallback, 1024);
    }

    private void TakePictureCallback(string path)
    {
        Debug.Log("Image path: " + path);
        if (path != null)
        {
            // Create a Texture2D from the captured image in the cache
            Texture2D texture = NativeCamera.LoadImageAtPath(path);

            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + path);
                return;
            }

            // create sprite from the texture
            Sprite newSprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(.5f, .5f));

            OnPictureTaken?.Invoke(newSprite);
        }
    }

    public void ChoosePicture()
    {
        NativeToolkit.PickImage();
    }
}
