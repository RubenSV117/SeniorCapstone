using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;
using System;
using NUnit.Framework;
using Tests.Util;
using System.Collections;

namespace Tests
{
    public class Constants
    {
        public static readonly string TEST_EMAIL = "TestEmail@FakeDomain.com";
        public static readonly string TEST_PASSWORD = "NotARealPassword";

        public static readonly string TEST_EMAIL_OTHER = "OtherTestEmail@FakeDomain.com";
        public static readonly string TEST_PASSWORD_OTHER = "DefinitelyNotARealPassword";
    }


    namespace Common
    {
        public class Auth
        {
            public static IEnumerator EnsureTestUserDoesNotExist()
            {
                return EnsureTestUserDoesNotExist(Constants.TEST_EMAIL, Constants.TEST_PASSWORD).AsIEnumerator();
            }

            public static IEnumerator EnsureOtherTestUserDoesNotExist()
            {
                return EnsureTestUserDoesNotExist(Constants.TEST_EMAIL_OTHER, Constants.TEST_PASSWORD_OTHER).AsIEnumerator();
            }

            public static async Task EnsureTestUserDoesNotExist(string email, string pass)
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
                if (!deleteTask.IsFaulted)
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

            public static IEnumerator EnsureTestUserExists()
            {
                return EnsureTestUserExists(false, Constants.TEST_EMAIL, Constants.TEST_PASSWORD).AsIEnumerator();
            }

            public static IEnumerator EnsureOtherTestUserExists()
            {
                return EnsureTestUserExists(false, Constants.TEST_EMAIL_OTHER, Constants.TEST_PASSWORD_OTHER).AsIEnumerator();
            }

            public static async Task EnsureTestUserExists(bool staySignedIn, string email, string pass)
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
                    Task<FirebaseUser> createUserTask = firebase.CreateUserWithEmailAndPasswordAsync(Constants.TEST_EMAIL, Constants.TEST_PASSWORD);
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

            public static IEnumerator EnsureUserExists(bool staySignedIn, string email, string pass)
            {
                string debugPrefix = "EnsureTestUserExists: ";
                Debug.Log(debugPrefix + "Begin");
                // find existing test user if any
                FirebaseAuth firebase = FirebaseAuth.DefaultInstance;
                Task<FirebaseUser> loginTask = firebase.SignInWithEmailAndPasswordAsync(email, pass);
                yield return loginTask.AsIEnumerator();
                FirebaseUser user;
                if (loginTask.IsFaulted)
                {
                    // user does not exist so we create it
                    Task<FirebaseUser> createUserTask = firebase.CreateUserWithEmailAndPasswordAsync(Constants.TEST_EMAIL, Constants.TEST_PASSWORD);
                    yield return createUserTask.AsIEnumerator();
                    try
                    {
                        user = createUserTask.Result;
                    }
                    catch (AggregateException)
                    {
                        // failed to create user
                        Debug.LogError(debugPrefix + "Failed to Create Test User: " + createUserTask.Exception.GetBaseException());
                        yield break;
                    }
                }
                user = loginTask.Result;
              

                if (!staySignedIn)
                {
                    Task nextAuthEvent = firebase.WaitForNextEvent("StateChanged");
                    firebase.SignOut();
                    yield return nextAuthEvent.AsIEnumerator();
                }
                Debug.Log(debugPrefix + "Finish");
                yield break;
            }
        }
    }

    namespace Util
    {
        public static class AssertAsync
        {
            public static async Task Throws<TException>(Func<Task> func)
            {
                var expected = typeof(TException);
                Type actual = null;
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    if (e is AggregateException)
                    {
                        e = e.GetBaseException();
                    }
                    actual = e.GetType();
                }
                Assert.AreEqual(expected, actual);
            }

        }

        public static class TaskExtensions
        {
            public static bool IsRunning(this Task<object> task) => !(task.IsCanceled || task.IsCompleted || task.IsFaulted);

            public static bool IsRunning(this Task task) => !(task.IsCanceled || task.IsCompleted || task.IsFaulted);
        }

        public static class Matcher
        {
            /// <summary>
            /// Checks to see if the arguments object is the same object o.
            /// Checks for reference equality
            /// </summary>
            /// <typeparam name="T">The argument type</typeparam>
            /// <param name="o">the object to check the argument reference with</param>
            /// <returns></returns>
            public static T Same<T>(T o)
            {
                return Moq.It.Is<T>(other => System.Object.ReferenceEquals(other, o));
            }
        }

        public static class EventHandlerExtensions
        {

            public static Task<T> WaitForNextEvent<T>(this object obj, string eventName)
            {
                var t = new TaskCompletionSource<T>();
                var evt = obj.GetType().GetEvent(eventName);
                EventHandler<T> x = null;
                x = (o, e) =>
                {
                    t.SetResult(e);
                    evt.RemoveEventHandler(obj, x);
                };
                evt.AddEventHandler(obj, x);
                return t.Task;
            }

            public static Task WaitForNextEvent(this object obj, string eventName)
            {
                var t = new TaskCompletionSource<EventArgs>();
                var evt = obj.GetType().GetEvent(eventName);
                EventHandler x = null;
                x = (o, e) =>
                {
                    t.SetResult(e);
                    evt.RemoveEventHandler(obj, x);
                };
                evt.AddEventHandler(obj, x);
                return t.Task;
            }
        }
    }

}