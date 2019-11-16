using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ReGenSDK.Exceptions
{
    public class RegenAuthenticationException : RegenException
    {
        public RegenAuthenticationException()
        {
        }

        public RegenAuthenticationException(string message) : base(message)
        {
        }

        public RegenAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegenAuthenticationException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}