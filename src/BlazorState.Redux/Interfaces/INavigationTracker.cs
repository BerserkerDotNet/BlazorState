using System;

namespace BlazorState.Redux.Interfaces
{
    public interface INavigationTracker<TState> : IDisposable
    {
        void Start(IDispatcher dispatcher);

        void Navigate(TState state);
    }
}
