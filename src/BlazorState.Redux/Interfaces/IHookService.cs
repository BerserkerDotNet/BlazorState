using System;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Interfaces
{
    public interface IHookService
    {
        (T, Action<T>) UseState<T>(T initialState, ComponentBase component);

        void ComponentRendered(ComponentBase component);

        void ComponentDisposed(ComponentBase component);
    }
}
