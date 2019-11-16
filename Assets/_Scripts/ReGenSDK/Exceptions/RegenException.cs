using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ReGenSDK.Exceptions
{
    public class RegenException : Exception
    {
        public RegenException()
        {
        }

        public RegenException(string message) : base(message)
        {
        }

        public RegenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegenException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}