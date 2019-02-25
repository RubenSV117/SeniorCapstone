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

        FB.LogInWithReadPermissions(permissions, (result) => 
        {
            print(string.Format("Logged in with token {0}, sending token to LoginService", result.AccessToken.TokenString));
            LoginService.Instance.SignInUserWithFacebook(result.AccessToken.TokenString); 
        }
        );
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
