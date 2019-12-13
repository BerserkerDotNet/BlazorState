using System;
using System.Collections.Generic;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Hooks
{
    // TODO: add component to have hooks enabled
    public class HooksService : IHookService
    {
        // TODO: can have multiple state objects for component
        private Dictionary<ComponentBase, ComponentState> _stateStore = new Dictionary<ComponentBase, ComponentState>();

        public (T, Action<T>) UseState<T>(T initialState, ComponentBase component)
        {
            if (!_stateStore.ContainsKey(component))
            {
                _stateStore.Add(component, new ComponentState());
            }

            var componentState = _stateStore[component];
            componentState.Add(initialState);
            var currentIdxClouser = componentState.CurrentIndex;
            return (componentState.GetNext<T>(), s => SetState(s, component, componentState, currentIdxClouser));
        }

        public void ComponentRendered(ComponentBase component)
        {
            if (_stateStore.ContainsKey(component))
            {
                var componentState = _stateStore[component];
                componentState.Finilize();
            }
        }

        private void SetState<T>(T newState, ComponentBase component, ComponentState componentState, int idx)
        {
            componentState.SetAtIndex(newState, idx);

            // StateHasChanged
            ((IHandleEvent)component).HandleEventAsync(new EventCallbackWorkItem(null), null);
        }

        private void DoNothing()
        {
        }
    }

    public class ComponentState
    {
        private List<object> _state = new List<object>();
        private bool _hasBeenInitialized = false;

        public int CurrentIndex { get; private set; } = 0;

        public void Add<T>(T state)
        {
            if (!_hasBeenInitialized)
            {
                _state.Add(state);
            }
        }

        public T GetNext<T>()
        {
            if (CurrentIndex >= _state.Count)
            {
                throw new ArgumentOutOfRangeException("useState is not registered");
            }

            return (T)_state[CurrentIndex++];
        }

        public void SetAtIndex<T>(T newState, int idx)
        {
            if (idx >= _state.Count)
            {
                throw new ArgumentOutOfRangeException("useState is not registered");
            }

            _state[idx] = newState;
        }

        public void Finilize()
        {
            _hasBeenInitialized = true;
            CurrentIndex = 0;
        }
    }

    public class HookedComponentBase : ComponentBase
    {
        [Inject]
        private IHookService Service { get; set; }

        protected (T, Action<T>) UseState<T>(T initialState)
        {
            VerifyServiceIsNotNull();
            return Service.UseState(initialState, this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            VerifyServiceIsNotNull();
            Service.ComponentRendered(this);
        }

        private void VerifyServiceIsNotNull()
        {
            if (Service is null)
            {
                throw new ArgumentNullException("Hooks service is not initialized!");
            }
        }
    }
}
