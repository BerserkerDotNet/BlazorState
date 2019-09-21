using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.JSInterop;

namespace BlazorState.Redux.DevTools
{
    public class ReduxDevToolsInterop : IDevToolsInterop
    {
        private static IDevToolsInterop _toolsInstance;
        private static bool _toolsReady = false;
        private readonly IJSRuntime _jSRuntime;
        private List<(IAction action, object state)> _messages = new List<(IAction, object)>();

        public ReduxDevToolsInterop(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;

            // TODO: Refactor. Weird hack to pass interop instance to JS after it initializes
            _toolsInstance = this;
        }

        public event EventHandler<JumpToStateEventArgs> OnJumpToStateChanged;

        [JSInvokable]
        public static async ValueTask DevToolsReady()
        {
            if (_toolsInstance is null)
            {
                Console.WriteLine("DevToolsInterop instance has not been initialized!");
                return;
            }

            await _toolsInstance.OnToolsReady();
        }

        public ValueTask SendInitial(object state)
        {
            if (!_toolsReady)
            {
                _messages.Add((null, state));
                return new ValueTask(Task.CompletedTask);
            }
            else
            {
                return SendInitialInternal(state);
            }
        }

        public ValueTask Send(IAction action, object state)
        {
            if (!_toolsReady)
            {
                _messages.Add((action, state));
                return new ValueTask(Task.CompletedTask);
            }
            else
            {
                return SendInternal(action, state);
            }
        }

        public async ValueTask OnToolsReady()
        {
            await _jSRuntime.InvokeVoidAsync("window.BlazorRedux.setInteropInstance", DotNetObjectReference.Create(this));
            _toolsReady = true;
            foreach (var message in _messages)
            {
                if (message.action is null)
                {
                    await SendInitialInternal(message.state);
                }
                else
                {
                    await SendInternal(message.action, message.state);
                }
            }
        }

        [JSInvokable]
        public void ReceiveMessage(DevToolsMessage message)
        {
            if (string.Equals(message.Type, "DISPATCH", StringComparison.OrdinalIgnoreCase))
            {
                OnJumpToStateChanged?.Invoke(this, new JumpToStateEventArgs(message.State));
            }
        }

        private ValueTask SendInitialInternal(object state)
        {
            return _jSRuntime.InvokeVoidAsync("window.BlazorRedux.sendInitial", state);
        }

        private ValueTask SendInternal(IAction action, object state)
        {
            return _jSRuntime.InvokeVoidAsync("window.BlazorRedux.send", action.ToString(), action, state);
        }
    }
}
