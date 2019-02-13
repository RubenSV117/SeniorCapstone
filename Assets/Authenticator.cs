using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

/// <summary>
/// This class wraps around the Firebase Authentication methods and provides debugging for each task.
/// </summary>
public class Authenticator
{
    private readonly Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private IList<Action<Firebase.Auth.FirebaseUser>> loginListeners;
    private IList<Action<Firebase.Auth.FirebaseUser>> logoutListeners;

    public Authenticator()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    /// <summary>
    /// Registers a new user using an email and password.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's new password to use for their account.</param>
    /// <returns>An asynchronous task that results in a Firebase.Auth.FirebaseUser.</returns>
    public Task RegisterUserWithEmail(string email, string password)
    {
        return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    /// <summary>
    /// Signs in an existing user with their email and paswword.
    /// </summary>
    /// <param name="email">the email of the user.</param>
    /// <param name="password">the password of the user.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    public Task SignInUserWithEmail(string email, string password)
    {
        return SignInWithCredentials(EmailAuthProvider.GetCredential(email, password));
    }

    /// <summary>
    /// Signs in a user with a Facebook access token. If their is a user already logged in, this will link the Facebook account
    /// with the currently logged in user.
    /// </summary>
    /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    public Task SignInUserWithFacebook(string accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken);
        if (auth.CurrentUser != null)
        {
            return auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("LinkWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
        else
        {
            return SignInWithCredentials(credential);
        }
    }

    private Task SignInWithCredentials(Credential credential)
    {
        return auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }

    /// <summary>
    /// Attaches a listener that is called whenever a login event is fired.
    /// </summary>
    /// <param name="loginHandler">An Action that is called with the logged in User as a parameter when a login event occurs.</param>
    public void onLogin(Action<Firebase.Auth.FirebaseUser> loginHandler)
    {
        loginListeners.Add(loginHandler);
    }

    /// <summary>
    /// Attaches a logout listener that is called whenever a logout event is fired
    /// </summary>
    /// <param name="logoutHandler">An Action that is called with the logged out User as a parameter when a logout evnent occurs.</param>
    public void onLogout(Action<Firebase.Auth.FirebaseUser> logoutHandler)
    {
        logoutListeners.Add(logoutHandler);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                foreach (Action<Firebase.Auth.FirebaseUser> listener in logoutListeners)
                {
                    listener(user);
                }
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                foreach (Action<Firebase.Auth.FirebaseUser> listener in loginListeners)
                {
                    listener(user);
                }
            }
        }
    }

}
