﻿using System;
using System.Threading.Tasks;
using ReGenSDK;
using ReGenSDK.Tasks;
using UnityEngine;

/// <summary>
/// Manages Login UI
///
/// References LoginInService, implements IPanel
///
/// Ruben Sanchez
/// 2/23/19
/// </summary>
public class LoginManagerUI : MonoBehaviour, IPanel
{
    public delegate void AccountAction(string message);

    public event AccountAction OnAccountActionAttempt;

    public static LoginManagerUI Instance;

    [SerializeField] private GameObject canvas;

    [Header("Initial Screen UI")] [SerializeField]
    private GameObject initialGroup; // initial log in, sign up buttons
    //[SerializeField] private Button signUpButton;
    //[SerializeField] private Button loginButton;


    [Header("Registration Options UI")] [SerializeField]
    private GameObject signupOptionsGroup; // ui group to register new user

    [SerializeField] private GameObject emailRegisterGroup;

    [Header("Login Options UI")] [SerializeField]
    private GameObject emailLoginGroup;


    //[SerializeField] private Button emailLoginButton;

    [SerializeField] private GameObject forgotEmailPasswordButton;

    [SerializeField] private GameObject loginGroup; // ui button group to sign in user

    private string email;
    private string password;
    private string confirmPassword;

    private Task loginAttempt;
    
    

    public void UpdateEmail(string value)
    {
        email = value;
    }

    public void UpdatePassword(string value)
    {
        password = value;
    }

    public void UpdateConfirmPassword(string value)
    {
        confirmPassword = value;
    }

    /// <summary>
    /// Attempts to log in user with the given email and password by referencing LoginService static instance, will invoke ui notification with result
    /// </summary>
    public void LogInWithEmail()
    {
        // start coruoutine to handle attempt result ui notification
        if (loginAttempt != null && loginAttempt.Status == TaskStatus.Running)
            return;

        NotificationManager.Instance.SetLoadingPanel(true);
        loginAttempt = ReGenClient.Instance.Authentication.SignInUserWithEmail(email, password)
            .Success(user =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                SwitchToMainMenu("Login Successful");
            })
            .Failure(exception =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                OnAccountActionAttempt?.Invoke(exception.Message);
            });
    }


    /// <summary>
    /// Attempts to create a new user with the given email and password by referencing LoginService static instance, will invoke ui notification with result
    /// </summary>
    public void RegisterNewUserWithEmail()
    {
        if (confirmPassword != password)
        {
            OnAccountActionAttempt?.Invoke("Confirmation password must match first password.");
            return;
        }

        if (loginAttempt != null && loginAttempt.Status == TaskStatus.Running)
            return;

        NotificationManager.Instance.SetLoadingPanel(true);
        loginAttempt = ReGenClient.Instance.Authentication.RegisterUserWithEmail(email, password)
            .Success(user =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                SwitchToMainMenu("Account registered");
            })
            .Failure(exception =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                OnAccountActionAttempt?.Invoke(exception.Message);
            });
    }


    public void LogInWithFB()
    {
        Action<string> success = token =>
        {
            ReGenClient.Instance.Authentication.SignInUserWithFacebook(token).Success(user =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                SwitchToMainMenu("Login Successful");
            }).Failure(error =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                OnAccountActionAttempt?.Invoke(error.Message);
            });
        };
        FacebookManager.Instance.Login(success, error =>
        {
            MainThreadTask.Run(async () =>
            {
                NotificationManager.Instance.SetLoadingPanel(false);
                OnAccountActionAttempt?.Invoke(error);
            });
        });
    }

    public void EnableLogin()
    {
        initialGroup.SetActive(false);
        loginGroup.SetActive(true);
    }

    public void EnableRegister()
    {
        initialGroup.SetActive(false);
        signupOptionsGroup.SetActive(true);
    }

    public void EnableInitialGroup()
    {
        initialGroup.SetActive(true);
        loginGroup.SetActive(false);
        signupOptionsGroup.SetActive(false);
        emailLoginGroup.SetActive(false);
        emailRegisterGroup.SetActive(false);
    }

    public void RecoverEmailPassword()
    {
        //LoginService.Instance.
    }

    public void Logout()
    {
        ReGenClient.Instance.Authentication.SignOut();
    }

    public void SkipLogIn()
    {
        Disable();
        MainMenuManagerUI.Instance.Enable();
    }

    public void SwitchToMainMenu(string transitionMessage)
    {
        OnAccountActionAttempt?.Invoke(transitionMessage);
        Disable();
        /** 
        * To-do: remove dependency on MainMenuManagerUI
        * (this class shouldn't know about other UI classes).
        */
        MainMenuManagerUI.Instance.Enable();
    }

    /// <summary>
    /// Enable Email UI group, disable login ui group
    /// </summary>
    public void EnableEmailLogin()
    {
        loginGroup.SetActive(false);
        emailLoginGroup.SetActive(true);

        // clear email button listener and set to Email sign in
        //emailLoginButton.onClick.RemoveAllListeners();
        //emailLoginButton.onClick.AddListener(LogInWithEmail);
    }

    /// <summary>
    /// Enable Email UI group, disable login ui group, update email button listener
    /// </summary>
    public void EnableEmailRegister()
    {
        signupOptionsGroup.SetActive(false);
        emailRegisterGroup.SetActive(true);

        // clear email button listener and set to Email register
        //emailLoginButton.onClick.RemoveAllListeners();
        //emailLoginButton.onClick.AddListener(RegisterNewUserWithEmail);
    }

    public void Enable()
    {
        canvas.SetActive(true);
        EnableInitialGroup();
    }

    public void Disable()
    {
        canvas.SetActive(false);
    }

    public void Init()
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (canvas != null) canvas.SetActive(ReGenClient.Instance.Authentication.User == null);
    }
}