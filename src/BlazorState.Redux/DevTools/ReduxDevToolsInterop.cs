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

        public async ValueTask Init(object state)
        {
            try
            {
                await _jSRuntime.InvokeVoidAsync("window.BlazorRedux.sendInitial", state, DotNetObjectReference.Create(this));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failied to send initial state to dev tools. {ex.Message}");
            }
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
