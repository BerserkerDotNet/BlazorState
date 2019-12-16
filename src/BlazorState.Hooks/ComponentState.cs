using System;
using System.Collections.Generic;

namespace BlazorState.Hooks
{
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
                throw new ArgumentOutOfRangeException($"useState is not registered. Requested: {CurrentIndex}; Registered: {_state.Count}");
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
            _state.TrimExcess();
        }
    }
}
