using System;
using System.Collections.Generic;
using BlazorState.Hooks.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Hooks
{
    public class HooksService : IHooksService
    {
        private Dictionary<IComponent, ComponentState> _stateStore = new Dictionary<IComponent, ComponentState>();

        public (T, Action<T>) UseState<T>(T initialState, IComponent component)
        {
            if (!_stateStore.ContainsKey(component))
            {
                _stateStore.Add(component, new ComponentState());
            }

            var componentState = _stateStore[component];
            componentState.Add(initialState);
            var currentIdxClouser = componentState.CurrentIndex;
            return (componentState.GetNext<T>(), s => SetState(s, componentState, currentIdxClouser));
        }

        public void ComponentRendered(IComponent component)
        {
            if (_stateStore.ContainsKey(component))
            {
                var componentState = _stateStore[component];
                componentState.Finilize();
            }
        }

        public void ComponentDisposed(IComponent component)
        {
            if (_stateStore.ContainsKey(component))
            {
                _stateStore.Remove(component);
            }
        }

        private void SetState<T>(T newState, ComponentState componentState, int idx)
        {
            componentState.SetAtIndex(newState, idx);
        }
    }
}
