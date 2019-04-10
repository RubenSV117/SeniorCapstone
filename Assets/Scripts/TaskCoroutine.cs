using System;
using System.Threading.Tasks;

public static class TaskCoroutineExtensions
{
    public static CoroutineTask<R> AsCoroutine<R>(this Task<R> task)
    {
        return new CoroutineTaskImpl<R>(task);
    }

    public static CoroutineTask AsCoroutine(this Task task)
    {
        return new CoroutineTaskImpl(task);
    }
}


public abstract class CoroutineTask : UnityEngine.CustomYieldInstruction
{
    protected AggregateException exception;
    protected bool isFaulted = false;
    protected bool isCanceled = false;
    protected bool isCompleted = false;

    public AggregateException Exception => Exception;
    public bool IsFaulted => isFaulted;
    public bool IsCanceled => isCanceled;
    public bool IsCompleted => isCompleted;
}

public abstract class CoroutineTask<T> : CoroutineTask
{
    protected T result;

    public T Result => result;
}

public class CoroutineTaskImpl<T> : CoroutineTask<T>
{
    readonly Task<T> t;

    public CoroutineTaskImpl(Task<T> task)
    {
        t = task;
    }

    public override bool keepWaiting
    {
        get
        {
            if (t.IsFaulted)
            {
                isFaulted = true;
                exception = t.Exception;
                return false;
            }
            else if (t.IsCanceled)
            {
                isCanceled = true;
                return false;
            }
            else if (t.IsCompleted)
            {
                isCompleted = true;
                result = t.Result;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

public class CoroutineTaskImpl : CoroutineTask
{
    readonly Task t;

    public CoroutineTaskImpl(Task task)
    {
        t = task;
    }

    public override bool keepWaiting
    {
        get
        {
            if (t.IsFaulted)
            {
                isFaulted = true;
                exception = t.Exception;
                return false;
            }
            else if (t.IsCanceled)
            {
                isCanceled = true;
                return false;
            }
            else if (t.IsCompleted)
            {
                isCompleted = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}