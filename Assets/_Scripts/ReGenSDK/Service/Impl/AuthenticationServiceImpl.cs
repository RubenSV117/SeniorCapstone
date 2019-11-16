using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using ReGenSDK.Exceptions;
using ReGenSDK.Tasks;
using UnityEngine;

namespace ReGenSDK.Service.Impl
{
    internal class AuthenticationServiceImpl : AuthenticationService
    {
        private readonly FirebaseAuth _auth;

        public AuthenticationServiceImpl()
        {
            // Retrieve default auth instance based on config file
            _auth = FirebaseAuth.DefaultInstance;

            // attach state change listener for login and logout events
            _auth.StateChanged += AuthStateChanged;
            User = _auth.CurrentUser;
        }

        /// <summary>
        /// This internal method listens for authentication state changes and upon detecting a change, fires an AuthEvent
        /// </summary>
        private void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (_auth.CurrentUser != User)
            {
                bool signedIn = User != _auth.CurrentUser && _auth.CurrentUser != null;
                if (!signedIn && User != null)
                {
                    // user logged out
                }

                User = _auth.CurrentUser;
                if (signedIn)
                {
                    // user logged in
                }
            }
        }

        public override Task<FirebaseUser> RegisterUserWithEmail(string email, string password)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (password == null) throw new ArgumentNullException(nameof(password));
            Task<FirebaseUser> task = _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            return task.Success(user =>
            {
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    user.DisplayName, user.UserId);
            }).Failure(e =>
            {
                var baseException = (e as AggregateException)?.GetBaseException() ?? e;
                Debug.Log(Exception("RegisterUserWithEmail encountered an error", baseException));
            });
        }

        public override Task SendRecoverPasswordEmail(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            return _auth.SendPasswordResetEmailAsync(email)
                .Success(() => Debug.Log($"Recovery email sent to {email}"))
                .Failure<FirebaseException>(e =>
                    Debug.LogException(Exception($"Failed to send recovery email to {email}: {e.Message}", e)));
        }

        public override Task<FirebaseUser> SignInUserWithEmail(string email, string password)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (password == null) throw new ArgumentNullException(nameof(password));
            return _auth.SignInWithEmailAndPasswordAsync(email, password)
                .Success(user => Debug.Log($"User {user.Email} signed in with email and password : {user.UserId}"))
                .Failure<FirebaseUser, FirebaseException>(e =>
                    Debug.LogError(Exception($"Failed to log in user {email}", e)));
        }

        public override Task<FirebaseUser> SignInUserWithFacebook(string accessToken)
        {
            if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);
            if (_auth.CurrentUser != null)
            {
                return _auth.CurrentUser.LinkWithCredentialAsync(credential)
                    .Success(user => Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                        user.DisplayName, user.UserId))
                    .Failure<FirebaseUser, FirebaseException>(exception =>
                        Debug.LogException(Exception($"Failed to link facebook to user {_auth.CurrentUser.UserId}",
                            exception)));
            }

            return _auth.SignInWithCredentialAsync(credential)
                .Success(user => Debug.LogFormat("User signed in successfully: {0} ({1})",
                    user.DisplayName, user.UserId))
                .Failure(e => Debug.LogException(Exception("Failed to sign in user with facebook", e)));
        }
        public override void SignOut()
        {
            _auth.SignOut();
        }

        private static RegenAuthenticationException Exception(string message, Exception baseException)
        {
            return new RegenAuthenticationException(message,
                baseException);
        }
    }
}