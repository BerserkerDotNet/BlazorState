using System;
using System.Collections.Generic;
using System.Linq;
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
            var properties = stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (_reducersMap.ContainsKey(property.Name) && property.SetMethod != null)
                {
                    var reducer = _reducersMap[property.Name];
                    var currentValue = state == null ? null : property.GetValue(state);

                    // TODO: Cache?
                    var reducerType = reducer.GetType();
                    if (!IsAssignableToGenericType(reducerType, typeof(IReducer<>)))
                    {
                        throw new ArgumentException($"Type {reducerType} is not assignable to {typeof(IReducer<TState>)}");
                    }

                    var newValue = reducerType.InvokeMember(nameof(IReducer<TState>.Reduce), BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, reducer, new object[] { currentValue, action });
                    property.SetValue(newState, newValue);
                }
            }

            return newState;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
