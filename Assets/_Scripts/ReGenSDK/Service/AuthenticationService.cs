using System.Threading.Tasks;
using Firebase.Auth;
using JetBrains.Annotations;

namespace ReGenSDK.Service
{
    public abstract class AuthenticationService
    {
        /// <summary>
        /// The currently logged in user. Null if no user is logged in.
        /// </summary>
        [CanBeNull]
        public FirebaseUser User { get; internal set; }

        /// <summary>
        /// Registers a new user using an email and password. If the registration fails, the user is null.
        /// If the account is successfully created, the newly created account is logged in.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's new password to use for their account.</param>
        /// <returns>An asynchronous task that results in a nullable Firebase.Auth.FirebaseUser.</returns>
        [NotNull]
        public abstract Task<FirebaseUser> RegisterUserWithEmail([NotNull] string email, [NotNull] string password);

        /// <summary>
        /// Send password recovery email to the user
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns></returns>
        [NotNull]
        public abstract Task SendRecoverPasswordEmail([NotNull] string email);

        /// <summary>
        /// Signs in an existing user with their email and paswword.
        /// </summary>
        /// <param name="email">the email of the user.</param>
        /// <param name="password">the password of the user.</param>
        /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
        [NotNull]
        public abstract Task<FirebaseUser> SignInUserWithEmail([NotNull] string email, [NotNull] string password);

        /// <summary>
        /// Signs in a user with a Facebook access token. If their is a user already logged in, this will link the Facebook account
        /// with the currently logged in user. If the User does not have an account, an account is automatically created for them.
        /// This method does sign in the user if successful.
        /// </summary>
        /// <param name="accessToken">The accessToken returned from Facebook Authentication.</param>
        /// <returns>A Task that if succesfully completes, results in the Firebase.Auth.FirebaseUser. </returns>
        [NotNull]
        public abstract Task<FirebaseUser> SignInUserWithFacebook([NotNull] string accessToken);

        /// <summary>
        /// Signs out the currently logged in user by clearing all authentication data. This method never fails and succeeds immediately.
        /// </summary>
        public abstract void SignOut();

    }
}