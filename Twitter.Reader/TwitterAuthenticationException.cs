using System;
using System.Runtime.Serialization;

namespace Twitter.Reader
{
    [Serializable]
    public class TwitterAuthenticationException : Exception
    {
        public TwitterAuthenticationException()
        {
        }

        public TwitterAuthenticationException(string message) : base(message)
        {
        }

        public TwitterAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TwitterAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}