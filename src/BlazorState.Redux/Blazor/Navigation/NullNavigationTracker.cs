using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Blazor.Navigation
{
    public class NullNavigationTracker<TState> : INavigationTracker<TState>
    {
        public void Dispose()
        {
        }

        public void Navigate(TState state)
        {
        }

        public void Start(IDispatcher dispatcher)
        {
        }
    }
}
