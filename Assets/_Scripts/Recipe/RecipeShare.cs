using System.Collections;
using UnityEngine;
using Firebase.Storage;
using ReGenSDK.Model;

/// <summary>
/// Manages recipe sharing
///
/// Ruben Sanchez
/// </summary>
public class RecipeShare : MonoBehaviour
{
    public static RecipeShare Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void ShareRecipe(Recipe recipe)
    {
        StartCoroutine(ShareAndroidText(recipe));
    }

    private IEnumerator ShareAndroidText(Recipe recipe)
    {
        yield return StartCoroutine(GetImageURL(recipe));

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), recipe.ToString());
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);
    }

    private IEnumerator GetImageURL(Recipe recipe)
    {
        StorageReference reference = StorageManager.storageReference.GetReferenceFromUrl(recipe.ImageReferencePath);

        reference.GetDownloadUrlAsync().ContinueWith((task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                recipe.RootImagePath = task.Result.ToString();
            }
        });

        yield return new WaitUntil(() => recipe.RootImagePath != null);

    }

}
