using System.Collections.Generic;
using System.Reflection;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux
{
    public class AggregateReducer<TState> : IReducer<TState>
        where TState : new()
    {
        private readonly Dictionary<string, object> _reducersMap;

        public AggregateReducer(Dictionary<string, object> reducersMap)
        {
            _reducersMap = reducersMap;
        }

        public TState Reduce(TState state, IAction action)
        {
            var stateType = typeof(TState);
            var newState = new TState();
            var properties = stateType.GetProperties();
            foreach (var property in properties)
            {
                if (_reducersMap.ContainsKey(property.Name))
                {
                    var reducer = _reducersMap[property.Name];
                    var currentValue = state == null ? null : property.GetValue(state);

                    // TODO: Cache?
                    var reducerType = reducer.GetType();
                    var newValue = reducerType.InvokeMember(nameof(IReducer<TState>.Reduce), BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, reducer, new object[] { currentValue, action });
                    property.SetValue(newState, newValue);
                }
            }

            return newState;
        }
    }
}
