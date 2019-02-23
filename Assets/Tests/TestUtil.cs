using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests
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
                actual = e.GetType();
            }
            Assert.Equals(expected, actual);
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
            return Moq.It.Is<T>(other => Object.ReferenceEquals(other, o));
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

