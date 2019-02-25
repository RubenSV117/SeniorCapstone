using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logs the user into their facebook account, handles social utlitiies
/// 
/// Ruben Sanchez
/// 2/23/19
/// </summary>
public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }

        else
            FB.ActivateApp();
    }

    public void Login()
    {
        var permissions = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, FBAuthCallback);
    }

    private void FBAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            print(string.Format("Logged in with token {0}, sending token to LoginService", AccessToken.CurrentAccessToken.ToString()));

            // sign into firebase with the acquired access token
            LoginService.Instance.SignInUserWithFacebook(AccessToken.CurrentAccessToken.ToString()); 
        }

        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void Logout()
    {
        FB.LogOut();
    }

    public void ShareRecipe()
    {
      
    }

    public void ShareApp()
    {

    }
}
