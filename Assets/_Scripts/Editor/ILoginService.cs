using System;
using System.Threading.Tasks;
using Firebase.Auth;

public interface ILoginService
{
    /// <summary>
    /// The currently logged in user. Null if no user is logged in.
    /// </summary>
    FirebaseUser User { get; }

    /// <summary>
    /// This event is fired whenever a user logs in or out
    /// </summary>
    event EventHandler<AuthEvent> OnEvent;
    /// <summary>
    /// This event is fired whenever a user logs in
    /// </summary>
    event EventHandler<AuthLoginEvent> OnLoginEvent;
    /// <summary>
    /// This event is fired whenevet a user logs out
    /// </summary>
    event EventHandler<AuthLogoutEvent> OnLogoutEvent;

    /// <summary>
    /// Check if the User already exists
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> CheckIfUserExists(string email);


    /// <summary>
    /// Check if the User already exists
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    CoroutineTask<bool> CheckIfUserExistsAsync(string email);

    /// <summary>
    /// Registers a new user using an email and password. If the registration fails, the user is null.
    /// If the account is successfully created, the newly created account is logged in.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's new password to use for their account.</param>
    /// <returns>An asynchronous task that results in a nullable Firebase.Auth.FirebaseUser.</returns>
    Task<FirebaseUser> RegisterUserWithEmail(string email, string password);

    /// <summary>
    /// Registers a new user using an email and password. If the registration fails, the user is null.
    /// If the account is successfully created, the newly created account is logged in.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's new password to use for their account.</param>
    /// <returns>An asynchronous task that results in a nullable Firebase.Auth.FirebaseUser.</returns>
    CoroutineTask<FirebaseUser> RegisterUserWithEmailAsync(string email, string password);

    [Obsolete("RegisterUserWithEmailAndPassword is deprecated, please use RegisterUserWithEmail instead.")]
    Task<FirebaseUser> RegisterUserWithEmailAndPassword(string email, string password);

    /// <summary>
    /// Send password recovery email to the user
    /// </summary>
    /// <param name="email">the email of the user</param>
    /// <returns></returns>
    Task SendRecoverPasswordEmail(string email);

    /// <summary>
    /// Send password recovery email to the user
    /// </summary>
    /// <param name="email">the email of the user</param>
    /// <returns></returns>
    CoroutineTask SendRecoverPasswordEmailAsync(string email);

    /// <summary>
    /// Signs in an existing user with their email and paswword.
    /// </summary>
    /// <param name="email">the email of the user.</param>
    /// <param name="password">the password of the user.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    Task<FirebaseUser> SignInUserWithEmail(string email, string password);

    /// <summary>
    /// Signs in an existing user with their email and paswword.
    /// </summary>
    /// <param name="email">the email of the user.</param>
    /// <param name="password">the password of the user.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    CoroutineTask<FirebaseUser> SignInUserWithEmailAsync(string email, string password);

    [Obsolete("SignInUserWithEmailAndPassword is deprecated, please use SignInUserWithEmail instead.")]
    Task<FirebaseUser> SignInUserWithEmailAndPassword(string email, string password);

    /// <summary>
    /// Signs in a user with a Facebook access token. If their is a user already logged in, this will link the Facebook account
    /// with the currently logged in user. If the User does not have an account, an account is automatically created for them.
    /// This method does sign in the user if successful.
    /// </summary>
    /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    Task<FirebaseUser> SignInUserWithFacebook(string accessToken);

    /// <summary>
    /// Signs in a user with a Facebook access token. If their is a user already logged in, this will link the Facebook account
    /// with the currently logged in user. If the User does not have an account, an account is automatically created for them.
    /// This method does sign in the user if successful.
    /// </summary>
    /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
    /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
    CoroutineTask<FirebaseUser> SignInUserWithFacebookAsync(string accessToken);

    /// <summary>
    /// Detaches this class from Firebase. This allows this class to be garbage collected if no other references exist.
    /// Upon calling this method, no further events are emitted and all event listeners are removed.
    /// </summary>
    void Detach();

    /// <summary>
    /// Signs out the currently logged in user by clearing all authentication data. This method never fails and succeeds immediately.
    /// </summary>
    [Obsolete("Use #LogOut instead")]
    void SignOut();

    /// <summary>
    /// Signs out the currently logged in user by clearing all authentication data. This method never fails and succeeds immediately.
    /// </summary>
    void LogOut();
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