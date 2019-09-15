using System;

namespace BlazorState.Redux.Interfaces
{
    public interface IStore<TState> : IStoreInitializer, IDispatcher
    {
        event EventHandler<EventArgs> OnStateChanged;

        TState State { get; }
    }
}
