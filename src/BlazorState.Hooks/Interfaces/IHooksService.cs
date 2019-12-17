using System;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Hooks.Interfaces
{
    public interface IHooksService
    {
        (T, Action<T>) UseState<T>(T initialState, IComponent component);

        void ComponentRendered(IComponent component);

        void ComponentDisposed(IComponent component);
    }
}
