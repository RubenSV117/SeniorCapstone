using System;

namespace ReGenSDK.Tasks
{
    /// <summary>
    /// This class handles success and failure callbacks for various tasks
    /// </summary>
    /// <typeparam name="E">The exception type if failed</typeparam>
    public interface ICallbackHandler<in E> where E : Exception
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

    /// <summary>
    /// This class handles success and failure callbacks for various tasks
    /// </summary>
    /// <typeparam name="T">The result type if successful</typeparam>
    /// <typeparam name="E">The exception type if failed</typeparam>
    public interface ICallbackHandler<in T, in E> where E : Exception
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
}