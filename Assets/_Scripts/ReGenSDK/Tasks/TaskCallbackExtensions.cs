using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Firebase.Extensions;
using JetBrains.Annotations;

namespace ReGenSDK.Tasks
{
    public static class TaskCallbackExtensions
    {
        public static Task<T> Callback<T, E>(this Task<T> task, [NotNull] ICallbackHandler<T, E> callback)
            where E : Exception
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            return Callback<T, E>(task, callback.OnSuccess, callback.OnFailure);
        }

        public static Task<T> Callback<T, E>([NotNull] this Task<T> task,
            [CanBeNull] Action<T> success,
            [CanBeNull] Action<E> failure)
            where E : Exception
        {
            return task.ContinueWithOnMainThread<T>(t =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        success?.Invoke(task.Result);
                        return task.Result;
                    case TaskStatus.Faulted:
                        failure?.Invoke(task.Exception as E);

                        Debug.Assert(task.Exception != null, "task.Exception != null");
                        throw task.Exception;

                    default:
                        return task.Result;
                }
            });
        }

        public static Task<R> Callback<T, R, E>([NotNull] this Task<T> task,
            [CanBeNull] Func<T, R> success,
            [CanBeNull] Action<E> failure)
            where E : Exception
        {
            return task.ContinueWithOnMainThread(t =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        return success != null ? success(task.Result) : default(R);
                    case TaskStatus.Faulted:
                        if (failure != null)
                        {
                            failure(task.Exception as E);
                        }

                        Debug.Assert(task.Exception != null, "task.Exception != null");
                        throw task.Exception;

                    default:
                        return default(R);
                }
            });
        }

        public static Task<T> Success<T>(this Task<T> task, [NotNull] Action<T> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            return Callback<T, Exception>(task, success, null);
        }
        
        public static Task<R> Success<T, R>(this Task<T> task, [NotNull] Func<T, R> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            return Callback<T, R, Exception>(task, success, null);
        }

        public static Task<R> Failure<R, E>(this Task<R> task, [NotNull] Action<E> failure) where E : Exception
        {
            if (failure == null) throw new ArgumentNullException(nameof(failure));
            return Callback(task, null, failure);
        }
        
        public static Task<R> Failure<R>(this Task<R> task, [NotNull] Action<Exception> failure)
        {
            return task.Failure<R, Exception>(failure);
        }

        public static Task Callback<E>(this Task task, [NotNull] ICallbackHandler<E> callback) where E : Exception
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            return Callback<E>(task, callback.OnSuccess, callback.OnFailure);
        }

        public static Task Callback<E>(this Task task, [CanBeNull] Action success, [CanBeNull] Action<E> failure)
            where E : Exception
        {
            return task.ContinueWithOnMainThread(t =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        success?.Invoke();
                        return;
                    case TaskStatus.Faulted:
                        if (failure != null)
                        {
                            failure(task.Exception as E);
                            break;
                        }
                        else
                        {
                            Debug.Assert(task.Exception != null, "task.Exception != null");
                            throw task.Exception;
                        }
                    default:
                        return;
                }
            });
        }

        public static Task Success(this Task task, [NotNull] Action success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            return Callback<Exception>(task, success, null);
        }

        public static Task Failure<E>(this Task task, [NotNull] Action<E> failure) where E : Exception
        {
            if (failure == null) throw new ArgumentNullException(nameof(failure));
            return Callback(task, null, failure);
        }

        public static Task Failure(this Task task, [NotNull] Action<Exception> failure)
        {
            return task.Failure<Exception>(failure);
        }
    }
}