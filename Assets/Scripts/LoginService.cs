using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
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
/// </summary>
public class LoginService
{
    private static readonly Lazy<LoginService> lazy = new Lazy<LoginService>(() => new LoginService());

    public static LoginService Instance { get { return lazy.Value; } }
    
    private readonly FirebaseAuth auth;
    public FirebaseUser User { get; private set; }

    /// <summary>
    /// This event is fired whenever a user logs in or out
    /// </summary>
    public event EventHandler<AuthEvent> OnEvent;
    /// <summary>
    /// This event is fired whenever a user logs in
    /// </summary>
    public event EventHandler<AuthLoginEvent> OnLoginEvent;
    /// <summary>
    /// This event is fired whenevet a user logs out
    /// </summary>
    public event EventHandler<AuthLogoutEvent> OnLogoutEvent;

    public LoginService()
    {
        OnEvent += (o, e) =>
        {
            if (e is AuthLoginEvent)
                OnLoginEvent(this, (AuthLoginEvent)e);
            else if (e is AuthLogoutEvent)
                OnLogoutEvent(this, (AuthLogoutEvent)e);
        };
        OnLoginEvent += (o, e) => Debug.Log("Signed in " + e.User.UserId);
        OnLogoutEvent += (o, e) => Debug.Log("Signed out " + e.User.UserId);

        // Retrieve default auth instance based on config file
        auth = FirebaseAuth.DefaultInstance;

        // attach state change listener for login and logout events
        auth.StateChanged += AuthStateChanged;
        User = auth.CurrentUser;
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
    /// Registers a new user using an email and password. If the registration fails, the user is null.
    /// If the account is successfully created, the newly created account is logged in.
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
            Firebase.Auth.FirebaseUser newUser = t.Result;
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
    /// This method does sign in the user if successful.
    /// </summary>
    /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    public Task<FirebaseUser> SignInUserWithFacebook(string accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken);
        if (auth.CurrentUser != null)
        {
            var task = auth.CurrentUser.LinkWithCredentialAsync(credential);
            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Debug.LogError("LinkWithCredentialAsync was canceled.");
                }
                if (t.IsFaulted)
                {
                    Debug.LogError("LinkWithCredentialAsync encountered an error: " + t.Exception);
                }

                Firebase.Auth.FirebaseUser newUser = t.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
            return task;
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
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
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
    /// Detaches this class from Firebase. This allows this class to be garbage collected if no other references exist.
    /// Upon calling this method, no further events are emitted and all event listeners are removed.
    /// </summary>
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
                Debug.Log("Signed in " + User.UserId);
                OnEvent(this, new AuthLoginEvent(User));
            }

        }
    }

    public abstract class AuthEvent : EventArgs
    {
        public readonly FirebaseUser User;

        internal AuthEvent(FirebaseUser user)
        {
            User = user;
        }
    }

    public class AuthLoginEvent : AuthEvent
    {
        public AuthLoginEvent(FirebaseUser user) : base(user)
        {
        }
    }

    public class AuthLogoutEvent : AuthEvent
    {
        public AuthLogoutEvent(FirebaseUser user) : base(user)
        {
        }
    }

}