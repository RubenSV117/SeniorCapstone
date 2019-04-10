using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
/// <summary>
/// This class handles success and failure callbacks for various tasks
/// </summary>
/// <typeparam name="T">The result type if successful</typeparam>
/// <typeparam name="E">The exception type if failed</typeparam>
public interface ICallbackHandler<T, E> where E : Exception
{
    /// <summary>
    /// This is called when the action succeeds
    /// </summary>
    /// <param name="result">the result object if any</param>
    void OnSuccess(T result);

    /// <summary>
    /// This is called when the action fails 
    /// </summary>
    /// <param name="exception">The exception that is thrown if any</param>
    void OnFailure(E exception);
}

/// <summary>
/// This class handles success and failure callbacks for various tasks
/// </summary>
/// <typeparam name="E">The exception type if failed</typeparam>
public interface ICallbackHandler<E> where E : Exception
{
    /// <summary>
    /// This is called when the action succeeds
    /// </summary>
    /// <param name="result">the result object if any</param>
    void OnSuccess();

    /// <summary>
    /// This is called when the action fails 
    /// </summary>
    /// <param name="exception">The exception that is thrown if any</param>
    void OnFailure(E exception);
}

public static class TaskCallbackExtensions
{
    
    public static Task<R> WithCallback<R, E>(this Task<R> task, ICallbackHandler<R, E> callback) where E : Exception
    {
        return WithCallback<R, E>(task, callback.OnSuccess, callback.OnFailure);
    }
    
    public static Task<R> WithCallback<R, E>(this Task<R> task, Action<R> successCallback, Action<E> failureCallback) where E : Exception
    {
        if (successCallback != null)
            task.ContinueWith(t => successCallback(task.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

        if (failureCallback != null)
            task.ContinueWith(t => failureCallback((E)t.Exception.GetBaseException()), TaskContinuationOptions.OnlyOnFaulted);

        return task;
    }
    
    public static Task<R> WithSuccess<R>(this Task<R> task, Action<R> successCallback)
    {
        return WithCallback<R, Exception>(task, successCallback, null);
    }
    
    public static Task<R> WithFailure<R, E>(this Task<R> task, Action<E> failureCallback) where E : Exception
    {
        return WithCallback(task, null, failureCallback);
    }
    
    public static Task WithCallback<E>(this Task task, ICallbackHandler<E> callback) where E : Exception
    {
        return WithCallback<E>(task, callback.OnSuccess, callback.OnFailure);
    }
    
    public static Task WithCallback<E>(this Task task, Action successCallback, Action<E> failureCallback) where E : Exception
    {
        if (successCallback != null)
            task.ContinueWith(t => successCallback(), TaskContinuationOptions.OnlyOnRanToCompletion);

        if (failureCallback != null)
            task.ContinueWith(t => failureCallback((E)t.Exception.GetBaseException()), TaskContinuationOptions.OnlyOnFaulted);

        return task;
    }
    
    public static Task WithSuccess(this Task task, Action successCallback)
    {
        return WithCallback<Exception>(task, successCallback, null);
    }
    
    public static Task WithFailure<E>(this Task task, Action<E> failureCallback) where E : Exception
    {
        return WithCallback(task, null, failureCallback);
    }
}

public static class FirebaseExtensions
{
    public static AuthError GetAuthError(this FirebaseException e)
    {
        return (AuthError)e.ErrorCode;
    }
}