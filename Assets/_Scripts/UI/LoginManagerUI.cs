using UnityEngine;
using System.Collections;
using Firebase;
using System;
using UnityEngine.UI;

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
    public static LoginManagerUI Instance;

    public delegate void AccountAction(string message);
    public event AccountAction OnAccountActionAttempt;

    [SerializeField] private GameObject canvas;

    [SerializeField] private Button emailButton;
    [SerializeField] private GameObject forgotEmailPasswordButton;

    [SerializeField] private GameObject emailGroup;

    [SerializeField] private GameObject loginGroup; // ui button group to sign in user
    [SerializeField] private GameObject registerGroup; // ui group to register new user
    [SerializeField] private GameObject initialGroup; // initial log in, sign up buttons

    private string email;
    private string passsword;

    private bool attemptFinished;
    private bool attemptSuccess;
    private string errorMessage;

    private Coroutine attemptCo;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    public void UpdateEmail(string value)
    {
        email = value;
    }

    public void UpdatePassword(string value)
    {
        passsword = value;
    }

    /// <summary>
    /// Attempts to log in user with the given email and password by referencing LoginService static instance, will invoke ui notification with result
    /// </summary>
    public void LogInWithEmail()
    {
        // start coruoutine to handle attempt result ui notification
        if(attemptCo != null)
        {
            StopCoroutine(attemptCo);
            attemptCo = null;
        }

        attemptCo = StartCoroutine(HandleLoginAttempt());

        attemptFinished = false;

        LoginService.Instance.SignInUserWithEmailAndPassword(email, passsword).WithSuccess(user =>
        {
            attemptSuccess = true;
            attemptFinished = true;
        })
        .WithFailure((FirebaseException exception) =>
        {
            // parse error code to send to ui notification
            string errorStr = exception.GetAuthError().ToString();

            errorMessage = "";

            for (int i = 0; i < errorStr.Length; i++)
            {
                errorMessage += (Char.IsUpper(errorStr[i]) && i > 0
                ? " " + errorStr[i].ToString()
                : errorStr[i].ToString());
            }

            attemptSuccess = false;
            attemptFinished = true;

        });
    }


    /// <summary>
    /// Attempts to create a new user with the given email and password by referencing LoginService static instance, will invoke ui notification with result
    /// </summary>
    public void RegisterNewUserWithEmail()
    {
        // start coruoutine to handle attempt result ui notification
        if (attemptCo != null)
        {
            StopCoroutine(attemptCo);
            attemptCo = null;
        }

        attemptCo = StartCoroutine(HandleLoginAttempt());

        attemptFinished = false;

        LoginService.Instance.RegisterUserWithEmail(email, passsword).WithSuccess(user =>
        {
            attemptSuccess = true;
            attemptFinished = true;
        })
        .WithFailure((FirebaseException exception) =>
        {
            // parse error code to send to ui notification
            string errorStr = exception.GetAuthError().ToString();

            errorMessage = "";

            for (int i = 0; i < errorStr.Length; i++)
            {
                errorMessage += (Char.IsUpper(errorStr[i]) && i > 0
                ? " " + errorStr[i].ToString()
                : errorStr[i].ToString());
            }

            attemptSuccess = false;
            attemptFinished = true;
        });
    }

    /// <summary>
    /// Send ui event to show account action attempt result
    /// </summary>
    /// <returns></returns>
    public IEnumerator HandleLoginAttempt()
    {
        NotificationManager.Instance.SetLoadingPanel(true);

        // wait until firebase finishes (had really unpredictable behavior if handled from within the WithFailure callback)
        yield return new WaitUntil(() => attemptFinished);

        NotificationManager.Instance.SetLoadingPanel(false);

        // send success notification or error message
        if (OnAccountActionAttempt != null)
        {
            if (!attemptSuccess)
                OnAccountActionAttempt.Invoke(errorMessage);

            else
            {
                OnAccountActionAttempt.Invoke("Login Successful");
                Disable();
            }
        }

        attemptFinished = false;
        attemptSuccess = false;
    }

    public void EnableLogin()
    {
        initialGroup.SetActive(false);
        loginGroup.SetActive(true);
    }

    public void EnableRegister()
    {
        initialGroup.SetActive(false);
        registerGroup.SetActive(true);
    }

    public void EnableInitialGroup()
    {
        initialGroup.SetActive(true);
        loginGroup.SetActive(false);
        registerGroup.SetActive(false);
        emailGroup.SetActive(false);
    }

    public void SetAttempt(bool finished, bool succeeded, string message)
    {
        attemptFinished = finished;
        attemptSuccess = succeeded;
        errorMessage = message;
    }

    public void LogInWithFB()
    {
        // start coruoutine to handle attempt result ui notification
        if (attemptCo != null)
        {
            StopCoroutine(attemptCo);
            attemptCo = null;
        }

        attemptCo = StartCoroutine(HandleLoginAttempt());

        attemptFinished = false;

        FacebookManager.Instance.Login();
    }

    public void RecoverEmailPassword()
    {
        //LoginService.Instance.
    }

    public void Logout()
    {
        LoginService.Instance.SignOut();
    }

    /// <summary>
    /// Enable Email UI group, disable login ui group
    /// </summary>
    public void EnableEmailLogin()
    {
        loginGroup.SetActive(false);
        emailGroup.SetActive(true);
        forgotEmailPasswordButton.SetActive(true);

        // clear email button listener and set to Email sign in
        emailButton.onClick.RemoveAllListeners();
        emailButton.onClick.AddListener(LogInWithEmail);
    }

    /// <summary>
    /// Enable Email UI group, disable login ui group, update email button listener
    /// </summary>
    public void EnableEmailRegister()
    {
        registerGroup.SetActive(false);
        emailGroup.SetActive(true);
        forgotEmailPasswordButton.SetActive(false);

        // clear email button listener and set to Email register
        emailButton.onClick.RemoveAllListeners();
        emailButton.onClick.AddListener(RegisterNewUserWithEmail);
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
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        throw new System.NotImplementedException();
    }
}
