using System;

namespace ReGenSDK
{
    public static class Util
    {
        /// <summary>
        /// Doesn't actually memoize.
        /// This just remembers the first result produced and returns that regardless of argument.
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public static Func<T, U> Memoized<T, U>(this Func<T, U> f)
        {
            bool didMemoize = false;
            U capture = default;
            return arg =>
            {
                if (didMemoize)
                    return capture;
                else
                {
                    didMemoize = true;
                    capture = f(arg);
                    return capture;
                }
            };
        } 
    }
}