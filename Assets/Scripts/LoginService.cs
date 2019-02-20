using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;


/// <summary>
/// This class wraps around the Firebase Authentication methods and provides debugging for each task.
/// Use cases:
///     Register User
///     Login User
///     Logout User
///     Retrieve User
///     Link Facebook Account
///     
/// The class also implements the Observable pattern by accepting an action to execute on login or logout
/// with the methods OnLogin, OnLogout, and the generic OnEvent that accepts an 
/// AuthenticationEventHandler object which handles both logout and login events.
/// </summary>
public class LoginService
{
    private readonly Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private ICollection<Action<Firebase.Auth.FirebaseUser>> loginListeners;
    private ICollection<Action<Firebase.Auth.FirebaseUser>> logoutListeners;

    public LoginService()
    {
        loginListeners = new LinkedList<Action<Firebase.Auth.FirebaseUser>>();
        logoutListeners = new LinkedList<Action<Firebase.Auth.FirebaseUser>>();

        // Retrieve default auth instance based on config file
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        // attach state change listener for login and logout events
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

           
    }

    /// <summary>
    /// Example task usage with login or registering
    /// </summary>
    private void Example()
    {
        RegisterUserWithEmail("email", "password").WithSuccess(user =>
        {
            // handle success here
        }).WithFailure( (FirebaseException exception) =>
        {
            // handle failure here   
        });
    }

    /// <summary>
    /// The currently logged in user
    /// </summary>
    public Firebase.Auth.FirebaseUser User
    {
        get { return auth.CurrentUser; }
    }


    /// <summary>
    /// Registers a new user using an email and password. If the registration fails, the user is null.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's new password to use for their account.</param>
    /// <returns>An asynchronous task that results in a nullable Firebase.Auth.FirebaseUser.</returns>
    public Task<FirebaseUser> RegisterUserWithEmail(string email, string password)
    {
        var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (t.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + t.Exception.GetBaseException());
                return;
            }
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            return;
        });
        return task;
    }

    /// <summary>
    /// Signs in an existing user with their email and paswword.
    /// </summary>
    /// <param name="email">the email of the user.</param>
    /// <param name="password">the password of the user.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    public Task<FirebaseUser> SignInUserWithEmail(string email, string password)
    {
        return SignInWithCredentials(EmailAuthProvider.GetCredential(email, password));
    }

    /// <summary>
    /// Signs in a user with a Facebook access token. If their is a user already logged in, this will link the Facebook account
    /// with the currently logged in user. If the User does not have an account, an account is automatically created for them.
    /// </summary>
    /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    public Task<FirebaseUser> SignInUserWithFacebook(string accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken);
        if (auth.CurrentUser != null)
        {
            return auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith<FirebaseUser>(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("LinkWithCredentialAsync was canceled.");
                    return null;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
                    return null;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                return newUser;
            });
        }
        else
        {
            return SignInWithCredentials(credential);
        }
    }

    private Task<FirebaseUser> SignInWithCredentials(Credential credential)
    {
        return auth.SignInWithCredentialAsync(credential).ContinueWith<FirebaseUser>(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return null;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return null;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            return newUser;
        });
    }

    /// <summary>
    /// Signs out the currently logged in user by clearing all authentication data. This method never fails and succeeds immediately.
    /// </summary>
    public void SignOut()
    {
        auth.SignOut();
    }

    /// <summary>
    /// Attaches a listener that is called whenever a login event is fired.
    /// </summary>
    /// <param name="loginHandler">An Action that is called with the logged in User as a parameter when a login event occurs.</param>
    public void OnLogin(Action<Firebase.Auth.FirebaseUser> loginHandler)
    {
        loginListeners.Add(loginHandler);
    }

    /// <summary>
    /// Attaches a logout listener that is called whenever a logout event is fired
    /// </summary>
    /// <param name="logoutHandler">An Action that is called with the logged out User as a parameter when a logout evnent occurs.</param>
    public void OnLogout(Action<Firebase.Auth.FirebaseUser> logoutHandler)
    {
        logoutListeners.Add(logoutHandler);
    }

    /// <summary>
    /// Attaches an AuthenticationEventHandler which listens for login and logout events.
    /// </summary>
    /// <param name="eventHandler">An object which implements the AuthenticationEventHandler interface</param>
    public void OnEvent(ILoginServiceEventHandler eventHandler)
    {
        OnLogin(eventHandler.OnLogin);
        OnLogout(eventHandler.OnLogout);
    }

    /// <summary>
    /// This internal method listens for authentication state changes and upon detecting a change, notifies either the attached
    /// login listeners or the logout listeners depending on the event.
    /// </summary>
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

/// <summary>
/// An event handler that listens for both login and logout events.
/// Register an object that implements this inteface using Authenticator#OnEvent
/// to begin handling events.
/// </summary>
public interface ILoginServiceEventHandler
{
    /// <summary>
    /// This method is called when a login occurs.
    /// </summary>
    /// <param name="user">The user that successfully logged in. Never null.</param>
    void OnLogin(FirebaseUser user);

    /// <summary>
    /// This method is called when a logout occurs.
    /// </summary>
    /// <param name="user">The user that successfully logged out.</param>
    void OnLogout(FirebaseUser user);
}

