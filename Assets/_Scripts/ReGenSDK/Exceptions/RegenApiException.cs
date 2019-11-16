using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ReGenSDK.Exceptions
{
    public class RegenApiException : RegenException
    {
        public RegenApiException()
        {
        }

        public RegenApiException(string message) : base(message)
        {
        }

        public RegenApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegenApiException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}