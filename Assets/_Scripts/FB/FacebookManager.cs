﻿using System;
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

//        if (!FB.IsInitialized)
//        {
//            FB.Init(() =>
//                {
//                    if (FB.IsInitialized)
//                        FB.ActivateApp();
//                    else
//                        Debug.LogError("Couldn't initialize");
//                },
//                isGameShown =>
//                {
//                    if (!isGameShown)
//                        Time.timeScale = 0;
//                    else
//                        Time.timeScale = 1;
//                });
//        }
//
//        else
//            FB.ActivateApp();
    }

    public void Login(Action<string> success, Action<string> failure)
    {
        var permissions = new List<string> {"public_profile", "email"};

//        FB.LogInWithReadPermissions(permissions, (result) =>
//        {
//            if (result.Cancelled)
//                failure(result.Error);
//            else
//            {
//                print($"Logged in with token {result.AccessToken.TokenString}, sending token to LoginService");
//                success(result.AccessToken.TokenString);
//            }
//        });
    }

    public void Logout()
    {
//        FB.LogOut();
    }

    public void ShareRecipe()
    {
    }

    public void ShareApp()
    {
    }
}