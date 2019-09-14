using System;
using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IStore<TState> : IDispatcher
    {
        event EventHandler<EventArgs> OnStateChanged;

        TState State { get; }

        ValueTask Initialize();
    }
}
