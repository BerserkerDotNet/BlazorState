using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.JSInterop;

namespace BlazorState.Redux.DevTools
{
    public class ReduxDevToolsInterop : IDevToolsInterop
    {
        private readonly IJSRuntime _jSRuntime;

        public ReduxDevToolsInterop(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public event EventHandler<JumpToStateEventArgs> OnJumpToStateChanged;

        public ValueTask Init(object state)
        {
            return _jSRuntime.InvokeVoidAsync("window.BlazorRedux.sendInitial", state, DotNetObjectReference.Create(this));
        }

        public ValueTask Send(IAction action, object state)
        {
            return _jSRuntime.InvokeVoidAsync("window.BlazorRedux.send", action.ToString(), action, state);
        }

        [JSInvokable]
        public void ReceiveMessage(DevToolsMessage message)
        {
            if (string.Equals(message.Type, "DISPATCH", StringComparison.OrdinalIgnoreCase))
            {
                OnJumpToStateChanged?.Invoke(this, new JumpToStateEventArgs(message.State));
            }
        }
    }
}
