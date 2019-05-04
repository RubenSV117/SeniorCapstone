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
    public delegate void AccountAction(string message);
    public event AccountAction OnAccountActionAttempt;

    public static LoginManagerUI Instance;

    [SerializeField] private GameObject canvas;

    [Header("Initial Screen UI")]
    [SerializeField] private GameObject initialGroup; // initial log in, sign up buttons
    //[SerializeField] private Button signUpButton;
    //[SerializeField] private Button loginButton;


    [Header("Registration Options UI")]
    [SerializeField] private GameObject signupOptionsGroup; // ui group to register new user
    [SerializeField] private GameObject emailRegisterGroup;

    [Header("Login Options UI")]
    [SerializeField] private GameObject emailLoginGroup;

    Firebase.Auth.FirebaseAuth auth;
    //[SerializeField] private Button emailLoginButton;

    [SerializeField] private GameObject forgotEmailPasswordButton;

    [SerializeField] private GameObject loginGroup; // ui button group to sign in user

    private string email;
    private string password;
	private string confirmPassword;

    private bool attemptFinished;
    private bool attemptSuccess;
    private string errorMessage;

    private Coroutine attemptCo;

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
        if(attemptCo != null)
        {
            StopCoroutine(attemptCo);
            attemptCo = null;
        }

        attemptCo = StartCoroutine(HandleLoginAttempt());

        attemptFinished = false;

        LoginService.Instance.SignInUserWithEmailAndPassword(email, password).WithSuccess(user =>
        {
            attemptSuccess = true;

            attemptFinished = true;
        })
        .WithFailure((FirebaseException exception) =>
        {
            // parse error code to send to ui notification
            //string errorStr = exception.GetAuthError().ToString();

            //errorMessage = "";

            //for (int i = 0; i < errorStr.Length; i++)
            //{
            //    errorMessage += (Char.IsUpper(errorStr[i]) && i > 0
            //    ? " " + errorStr[i].ToString()
            //    : errorStr[i].ToString());
            //}
            errorMessage = exception.Message;

            attemptSuccess = false;
            attemptFinished = true;

        });
    }

    public void assignUsername(string username)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = username,
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }
    }


    /// <summary>
    /// Attempts to create a new user with the given email and password by referencing LoginService static instance, will invoke ui notification with result
    /// </summary>
    public void RegisterNewUserWithEmail()
    {
        if (confirmPassword != password)
        {
            errorMessage = "Confirmation password must match first password.";
            OnAccountActionAttempt?.Invoke(errorMessage);
            return;
        }

        // start coruoutine to handle attempt result ui notification
        if (attemptCo != null)
        {
            StopCoroutine(attemptCo);
            attemptCo = null;
        }

        attemptCo = StartCoroutine(HandleSignupAttempt());

        attemptFinished = false;

        LoginService.Instance.RegisterUserWithEmail(email, password).WithSuccess(user =>
        {
            attemptSuccess = true;
            attemptFinished = true;
        })
        .WithFailure((FirebaseException exception) =>
        {
            
            // parse error code to send to ui notification
            string errorStr = exception.GetAuthError().ToString();

            //errorMessage = "";

            //for (int i = 0; i < errorStr.Length; i++)
            //{
            //    errorMessage += (Char.IsUpper(errorStr[i]) && i > 0
            //    ? " " + errorStr[i].ToString()
            //    : errorStr[i].ToString());
            //}

            errorMessage = exception.Message;

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
            if (attemptSuccess)
            {
                //OnAccountActionAttempt.Invoke("Login Successful");
                //Disable();
                SwitchToMainMenu("Login Successful");
            }
            else
            {
                OnAccountActionAttempt.Invoke(errorMessage);
            }
        }

        attemptFinished = false;
        attemptSuccess = false;
    }

    /// <summary>
    /// Display a popup for event success or failure.
    /// </summary>
    /// <returns></returns>
    public IEnumerator HandleSignupAttempt()
    {
        NotificationManager.Instance.SetLoadingPanel(true);

        // wait until firebase finishes (had really unpredictable behavior if handled from within the WithFailure callback)
        yield return new WaitUntil(() => attemptFinished);

        NotificationManager.Instance.SetLoadingPanel(false);

        // send success notification or error message
        if (OnAccountActionAttempt != null)
        {
            OnAccountActionAttempt.Invoke(
                ( attemptSuccess ? 
                "Account registered" : errorMessage ));
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

    public void SkipLogIn()
    {
        Disable();
        MainMenuManagerUI.Instance.Enable();
    }

    public void SwitchToMainMenu(string transitionMessage)
    {
        OnAccountActionAttempt.Invoke(transitionMessage);
        this.Disable();

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
        forgotEmailPasswordButton.SetActive(true);

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
        forgotEmailPasswordButton.SetActive(false);

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
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        canvas?.SetActive(true);
        ;
    }
}
