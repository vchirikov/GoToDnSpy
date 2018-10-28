using System;

namespace GoToDnSpy
{
    [Serializable]
    public class GoToDnSpyException : Exception
    {
        public GoToDnSpyException() : base()
        {
        }

        public GoToDnSpyException(string message) : base(message)
        {
        }

        public GoToDnSpyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GoToDnSpyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

    }
}
