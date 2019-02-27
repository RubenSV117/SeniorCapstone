using Firebase;
using Firebase.Auth;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    /// <summary>
    /// These are integration tests that verify interactions with Firebase.
    /// Unity does not actually support async tests so all tests return immediately and pass.
    /// </summary>
    [TestFixture]
    public class AccountCreationTests
    {
        private static readonly string TEST_EMAIL = "TestEmail@FakeDomain.com";
        private static readonly string TEST_PASSWORD = "NotARealPassword";

        private static readonly string TEST_EMAIL_OTHER = "OtherTestEmail@FakeDomain.com";
        private static readonly string TEST_PASSWORD_OTHER = "DefinitelyNotARealPassword";

        private LoginService auth;

        [SetUp]
        public void SetUpService()
        {
            auth = new LoginService();
        }

        [TearDown]
        public void TearDownService()
        {
            auth.Detach();
        }

        [UnityTest]
        public IEnumerator CreateAccountEmail()
        {
            Task setupTask = EnsureTestUserDoesNotExist();
            while (setupTask.IsRunning())
            {
                yield return null;
            }
            // test event handler 
            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            // this method creates a new user and also logs in with that user
            Task<FirebaseUser> taskRegister = auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD)
                .WithCallback(callback.Object);
            while (taskRegister.IsRunning())
            {
                yield return null;
            }

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
            Task setupTask = EnsureTestUserExists(false);
            while (setupTask.IsRunning())
            {
                yield return null;
            }

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;
            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            LogAssert.Expect(LogType.Error, "CreateUserWithEmailAndPasswordAsync encountered an error: Firebase.FirebaseException: The email address is already in use by another account.");
            Task taskRegister = AssertAsync.Throws<FirebaseException>(() =>
            {
                // this method creates a new user and also attempts to logs in with that user
                return auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD)
                    .WithCallback(callback.Object);
            });
            while (taskRegister.IsRunning())
            {
                yield return null;
            }

            // no login or logout
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnFailure(It.IsAny<FirebaseException>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        [UnityTest]
        public IEnumerator SignInEmail()
        {
            Task setupTask = EnsureTestUserExists(false);
            while (setupTask.IsRunning())
            {
                yield return null;
            }

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            Task<FirebaseUser> signInTask = auth.SignInUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithCallback(callback.Object);
            while (signInTask.IsRunning())
            {
                yield return null;
            }

            eventHandler.Verify(handler => handler(Matcher.Same(auth), It.IsNotNull<AuthLoginEvent>()), Times.Once);
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnSuccess(It.IsAny<FirebaseUser>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        [UnityTest]
        public IEnumerator SignInSameEmailWhileSignedIn()
        {
            Task setupTask = EnsureTestUserExists(true);
            while (setupTask.IsRunning())
            {
                yield return null;
            }

            Mock<EventHandler<AuthEvent>> eventHandler = new Mock<EventHandler<AuthEvent>>();
            auth.OnEvent += eventHandler.Object;

            // test callback
            Mock<ICallbackHandler<FirebaseUser, FirebaseException>> callback = new Mock<ICallbackHandler<FirebaseUser, FirebaseException>>();

            Task<FirebaseUser> signInTask = auth.SignInUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithCallback(callback.Object);
            while (signInTask.IsRunning())
            {
                yield return null;
            }

            eventHandler.Verify(handler => handler(Matcher.Same(auth), It.IsNotNull<AuthLoginEvent>()), Times.Once);
            eventHandler.VerifyNoOtherCalls();

            callback.Verify(c => c.OnSuccess(It.IsAny<FirebaseUser>()), Times.Once);
            callback.VerifyNoOtherCalls();
        }

        //[OneTimeTearDown]
        public static void CleanUpTestUsers()
        {
            Task t1 = EnsureTestUserDoesNotExist();
            Task t2 = EnsureOtherTestUserDoesNotExist();
        }

        private static async Task EnsureTestUserDoesNotExist()
        {
            await EnsureTestUserDoesNotExist(TEST_EMAIL, TEST_PASSWORD);
        }

        private static async Task EnsureOtherTestUserDoesNotExist()
        {
            await EnsureTestUserDoesNotExist(TEST_EMAIL_OTHER, TEST_PASSWORD_OTHER);
        }

        private static async Task EnsureTestUserDoesNotExist(string email, string pass)
        {
            string debugPrefix = "EnsureTestUserDoesNotExist: ";
            Debug.Log(debugPrefix + "Begin");
            // remove existing test user if any
            Task<FirebaseUser> loginTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, pass);
            FirebaseUser user;
            try
            {
                user = await loginTask;
            }
            catch (AggregateException)
            {
                Debug.Log(debugPrefix + "Finish");
                return;
            }
            Debug.Log(debugPrefix + "Test User Found: " + user.Email);
            Task deleteTask = user.DeleteAsync();
            await deleteTask;
            if (deleteTask.IsCompleted)
            {
                Debug.Log(debugPrefix + "Deleted Test User Successfully");
            }
            else
            {
                Debug.LogError(debugPrefix + "Failed to Delete Test User: " + deleteTask.Exception.GetBaseException());
            }
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log(debugPrefix + "Finish");
            return;
        }

        private static async Task EnsureTestUserExists(bool staySignedIn)
        {
            await EnsureTestUserExists(staySignedIn, TEST_EMAIL, TEST_PASSWORD);
        }

        private static async Task EnsureOtherTestUserExists(bool staySignedIn)
        {
            await EnsureTestUserExists(staySignedIn, TEST_EMAIL_OTHER, TEST_PASSWORD_OTHER);
        }

        private static async Task EnsureTestUserExists(bool staySignedIn, string email, string pass)
        {
            string debugPrefix = "EnsureTestUserExists: ";
            Debug.Log(debugPrefix + "Begin");
            // find existing test user if any
            FirebaseAuth firebase = FirebaseAuth.DefaultInstance;
            Task<FirebaseUser> loginTask = firebase.SignInWithEmailAndPasswordAsync(email, pass);
            FirebaseUser user;
            try
            {
                user = await loginTask;
            }
            catch (AggregateException)
            {
                // user does not exist so we create it
                Task<FirebaseUser> createUserTask = firebase.CreateUserWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
                try
                {
                    user = await createUserTask;
                }
                catch (AggregateException)
                {
                    // failed to create user
                    Debug.LogError(debugPrefix + "Failed to Create Test User: " + createUserTask.Exception.GetBaseException());
                    return;
                }
            }


            if (!staySignedIn)
            {
                Task nextAuthEvent = firebase.WaitForNextEvent("StateChanged");
                firebase.SignOut();
                await nextAuthEvent;
            }



            Debug.Log(debugPrefix + "Finish");
            return;
        }

    }
}
