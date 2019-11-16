using System;
using System.Globalization;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

namespace ReGenSDK.Tasks
{
    public static class MainThreadTask
    {
        public static Task Run(Func<Task> operation)
        {
            return Task.CompletedTask.ContinueWithOnMainThread(async dummy => await operation());
        }

        public static Task<T> Run<T>(Func<Task<T>> operation)
        {
            var task = Task.FromResult<T>(default);
            return task.ContinueWithOnMainThread(async dummy => await operation()).Unwrap();
        }


        public static Task Run(this MonoBehaviour behaviour, Func<Task> operation)
        {
            return Run(operation);
        }
    }
}