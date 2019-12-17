using System;

namespace BlazorState.Hooks.Types
{
    [Serializable]
    public class StatePropertyNotFoundException : Exception
    {
        public StatePropertyNotFoundException(string message)
            : base(message)
        {
        }
    }
}
