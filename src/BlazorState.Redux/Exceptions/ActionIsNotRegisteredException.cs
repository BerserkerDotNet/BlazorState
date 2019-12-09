using System;

namespace BlazorState.Redux.Exceptions
{
    [Serializable]
    public class ActionIsNotRegisteredException : Exception
    {
        public ActionIsNotRegisteredException()
        {
        }

        public ActionIsNotRegisteredException(string message)
            : base(message)
        {
        }

        public ActionIsNotRegisteredException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ActionIsNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
