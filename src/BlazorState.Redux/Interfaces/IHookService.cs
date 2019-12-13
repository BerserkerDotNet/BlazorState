using System;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Interfaces
{
    public interface IHookService
    {
        // Not synced to the global store
        (T, Action<T>) UseState<T>(T initialState, ComponentBase component);

        void ComponentRendered(ComponentBase component);
    }
}
