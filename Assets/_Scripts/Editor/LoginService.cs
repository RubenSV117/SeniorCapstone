using Firebase;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class wraps around the Firebase Authentication methods.
/// Use cases:
///     Register User
///     Login User
///     Logout User
///     Retrieve User
///     Link Facebook Account
///     
/// Available Events:
///     OnEvent
///     OnLoginEvent
///     OnLogoutEvent
///     
/// To release this object for garbage collection, the #Detach method
/// must be called to detach this object from Firebase 
/// 
/// </summary>
public class LoginService : ILoginService
{
    private static readonly Lazy<ILoginService> lazy = new Lazy<ILoginService>(() => new LoginService());

    public static ILoginService Instance => lazy.Value;

    private readonly FirebaseAuth auth;

    public event EventHandler<AuthEvent> OnEvent;
    public event EventHandler<AuthLoginEvent> OnLoginEvent;
    public event EventHandler<AuthLogoutEvent> OnLogoutEvent;

    public FirebaseUser User { get; private set; }

    public LoginService()
    {
        OnEvent += (o, e) =>
        {
            if (e is AuthLoginEvent)
            {
                OnLoginEvent(this, (AuthLoginEvent)e);
            }
            else if (e is AuthLogoutEvent)
            {
                OnLogoutEvent(this, (AuthLogoutEvent)e);
            }
        };
        OnLoginEvent += (o, e) => Debug.Log("Signed in " + e.User.UserId);
        OnLogoutEvent += (o, e) => Debug.Log("Signed out " + e.User.UserId);
        // Retrieve default auth instance based on config file
        auth = FirebaseAuth.DefaultInstance;

        // attach state change listener for login and logout events
        auth.StateChanged += AuthStateChanged;
        User = auth.CurrentUser;
    }

    public Task<FirebaseUser> SignInUserWithEmailAndPassword(string email, string password)
    {
        var task = auth.SignInWithEmailAndPasswordAsync(email, password);

        task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (t.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = t.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

        return task;
    }


    public Task<FirebaseUser> RegisterUserWithEmailAndPassword(string email, string password)
    {
        return RegisterUserWithEmail(email, password);
    }

    public Task<FirebaseUser> RegisterUserWithEmail(string email, string password)
    {
        Task<FirebaseUser> task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (t.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + t.Exception.GetBaseException());
                return;
            }
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = t.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            return;
        });
        return task;
    }

    public Task<bool> CheckIfUserExists(string email)
    {
        Task<IEnumerable<string>> task = auth.FetchProvidersForEmailAsync(email);

        return task.ContinueWith(providers =>
        {
            return providers.Result.Any();
        });
    }


    public Task<FirebaseUser> SignInUserWithEmail(string email, string password)
    {
        return SignInWithCredentials(EmailAuthProvider.GetCredential(email, password));
    }

    //public Task<FirebaseUser> SignInUserWithEmailAndPassword(string email, string password)
    //{
    //    return SignInUserWithEmail(email, password);
    //}

    public Task SendRecoverPasswordEmail(string email)
    {
        return auth.SendPasswordResetEmailAsync(email)
            .WithSuccess(() => Debug.Log("Recovery email sent to " + email))
            .WithFailure<FirebaseException>(e => Debug.Log("Failed to send recovery email to " + email + ": " + e.Message));
    }

    public Task<FirebaseUser> SignInUserWithFacebook(string accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken);
        if (auth.CurrentUser != null)
        {
            Task<FirebaseUser> task = auth.CurrentUser.LinkWithCredentialAsync(credential);
            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Debug.Log("LinkWithCredentialAsync was canceled.");
                }
                if (t.IsFaulted)
                {
                    Debug.Log("LinkWithCredentialAsync encountered an error: " + t.Exception);
                }

                Firebase.Auth.FirebaseUser newUser = t.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
            return task;
        }
        else
        {
            Task<FirebaseUser> task = auth.SignInWithCredentialAsync(credential);

            task.ContinueWith(t =>
                {
                    if (t.IsCanceled)
                    {
                        Debug.Log("SignInWithCredentialAsync was canceled.");
                        return;
                    }
                    if (t.IsFaulted)
                    {
                        Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Firebase.Auth.FirebaseUser newUser = t.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                                        newUser.DisplayName, newUser.UserId);
                });
            return task;
        }
    }

    private Task<FirebaseUser> SignInWithCredentials(Credential credential)
    {
        return auth.SignInWithCredentialAsync(credential);
    }

    public void SignOut()
    {
        auth.SignOut();
    }

    public void LogOut()
    {
        auth.SignOut();
    }

    public void Detach()
    {
        auth.StateChanged -= AuthStateChanged;
        OnEvent = null;
        OnLoginEvent = null;
        OnLogoutEvent = null;
    }

    /// <summary>
    /// This internal method listens for authentication state changes and upon detecting a change, fires an AuthEvent
    /// </summary>
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != User)
        {
            bool signedIn = User != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && User != null)
            {
                OnEvent(this, new AuthLogoutEvent(User));
            }
            User = auth.CurrentUser;
            if (signedIn)
            {
                OnEvent(this, new AuthLoginEvent(User));
            }

        }
    }

    CoroutineTask<bool> ILoginService.CheckIfUserExistsAsync(string email)
    {
        return CheckIfUserExists(email).AsCoroutine();
    }

    CoroutineTask<FirebaseUser> ILoginService.RegisterUserWithEmailAsync(string email, string password)
    {
        return RegisterUserWithEmail(email, password).AsCoroutine();
    }

    CoroutineTask ILoginService.SendRecoverPasswordEmailAsync(string email)
    {
        return SendRecoverPasswordEmail(email).AsCoroutine();
    }

    CoroutineTask<FirebaseUser> ILoginService.SignInUserWithEmailAsync(string email, string password)
    {
        return SignInUserWithEmail(email, password).AsCoroutine();
    }

    CoroutineTask<FirebaseUser> ILoginService.SignInUserWithFacebookAsync(string accessToken)
    {
        return SignInUserWithFacebook(accessToken).AsCoroutine();
    }
}