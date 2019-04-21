using UnityEngine;

/// <summary>
///
///
/// Ruben Sanchez
/// 
/// </summary>
public class CameraManager : MonoBehaviour
{
    public void TakePicture()
    {
        NativeCamera.TakePicture(TakePictureCallback);
    }

    private void TakePictureCallback(string path)
    {
        print($"PATH: {path}");
    }
}
