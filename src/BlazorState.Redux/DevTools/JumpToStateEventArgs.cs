using System;

namespace BlazorState.Redux.DevTools
{
    public class JumpToStateEventArgs : EventArgs
    {
        public JumpToStateEventArgs(string stateJson)
        {
            StateJson = stateJson;
        }

        public string StateJson { get; private set; }
    }
}
