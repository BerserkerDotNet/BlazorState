using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.DevTools
{
    public class NullDevToolsInterop : IDevToolsInterop
    {
#pragma warning disable CS0067
        public event EventHandler<JumpToStateEventArgs> OnJumpToStateChanged;
#pragma warning restore CS0067

        public ValueTask SendInitial(object state)
        {
            return new ValueTask(Task.CompletedTask);
        }

        public void ReceiveMessage(DevToolsMessage message)
        {
        }

        public ValueTask Send(IAction action, object state)
        {
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask OnToolsReady()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
