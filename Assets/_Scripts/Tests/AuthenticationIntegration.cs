using Firebase;
using Firebase.Auth;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading.Tasks;
using Tests.Util;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.Common.Auth;
using static Tests.Constants;

namespace Tests.Misc
{
    [TestFixture]
    public class FirebaseAuthIntegration
    {
        private LoginService auth;

        static FirebaseAuthIntegration()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    //   app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    Debug.Log("Firebase: READY");
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        [SetUp]
        public void SetUp()
        {
            auth = new LoginService();
        }

        [TearDown]
        public void TearDown()
        {
            auth.Detach();
        }

        [UnityTest]
        public IEnumerator CreateAccountEmail()
        {
            yield return EnsureTestUserDoesNotExist();

            // test event handler 
            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            // this method creates a new user and also logs in with that user
            Task<FirebaseUser> taskRegister = null;
            taskRegister = auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithCallback(callback.Object);
            yield return taskRegister.AsIEnumerator();

            FirebaseUser user = taskRegister.Result;
            Assert.NotNull(user);
            StringAssert.AreEqualIgnoringCase(user.Email, TEST_EMAIL);

            // login event is fired
            eventHandler.Verify(handler => handler(Matcher.Same(auth), It.IsNotNull<AuthLoginEvent>()), Times.Once);
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnSuccess(It.IsAny<FirebaseUser>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        [UnityTest]
        public IEnumerator CreateDuplicateAccountEmail()
        {
            yield return EnsureTestUserExists();

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;
            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();


            Task taskRegister = AssertAsync.Throws<FirebaseException>(() =>
            {
                // this method creates a new user and also attempts to logs in with that user
                return auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD)
                    .WithCallback(callback.Object);
            });
            yield return taskRegister.AsIEnumerator();
            //LogAssert.Expect(LogType.Log, "CreateUserWithEmailAndPasswordAsync encountered an error: Firebase.FirebaseException: The email address is already in use by another account.");

            // no login or logout
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnFailure(It.IsAny<FirebaseException>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        [UnityTest]
        public IEnumerator SignInEmail()
        {
            yield return EnsureTestUserExists();

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            Task<FirebaseUser> signInTask = auth.SignInUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithCallback(callback.Object);
            yield return signInTask.AsIEnumerator();

            eventHandler.Verify(handler => handler(Matcher.Same(auth), It.IsNotNull<AuthLoginEvent>()), Times.Once);
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnSuccess(It.IsAny<FirebaseUser>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        //[UnityTest]
        public IEnumerator SignInSameEmailWhileSignedIn()
        {
            yield return EnsureTestUserExists();

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            Task<FirebaseUser> signInTask = auth.SignInUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithCallback(callback.Object);
            yield return signInTask.AsIEnumerator();

            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnSuccess(It.IsAny<FirebaseUser>()), Times.Once);
            callback.VerifyNoOtherCalls();

            // If user signs in again it is redundant, the task succeeds and no event is fired.
        }

        [UnityTest]
        public IEnumerator CheckIfUserExists()
        {
            yield return EnsureTestUserExists();
            yield return EnsureOtherTestUserDoesNotExist();

            // test user does exist
            Mock<ICallbackHandler<bool, Exception>> callback1 = new Mock<ICallbackHandler<bool, Exception>>();
            Task<bool> t1 = auth.CheckIfUserExists(TEST_EMAIL).WithCallback(callback1.Object);
            yield return t1.AsIEnumerator();

            callback1.Verify(c => c.OnSuccess(true), Times.Once);
            callback1.VerifyNoOtherCalls();


            // test user does not exist
            Mock<ICallbackHandler<bool, Exception>> callback2 = new Mock<ICallbackHandler<bool, Exception>>();
            Task<bool> t2 = auth.CheckIfUserExists(TEST_EMAIL_OTHER).WithCallback(callback2.Object);
            yield return t2.AsIEnumerator();

            callback2.Verify(c => c.OnSuccess(false), Times.Once);
            callback2.VerifyNoOtherCalls();
        }
    }
}
