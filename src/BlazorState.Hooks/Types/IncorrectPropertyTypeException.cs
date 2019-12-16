using System;

namespace BlazorState.Hooks.Types
{
    [Serializable]
    public class IncorrectPropertyTypeException : Exception
    {
        public IncorrectPropertyTypeException(string message)
            : base(message)
        {
        }
    }
}
