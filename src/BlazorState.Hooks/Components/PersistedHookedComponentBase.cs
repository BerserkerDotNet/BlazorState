using System;
using System.Linq.Expressions;
using System.Reflection;
using BlazorState.Hooks.Types;

namespace BlazorState.Hooks
{
    public abstract class PersistedHookedComponentBase<TState> : HookedComponentBase
        where TState : class
    {
        protected (T, Action<T>) UseState<T>(Expression<Func<TState, T>> propertyMap)
        {
            var state = GetStateProperty();
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            var memberExp = propertyMap.Body as MemberExpression;
            if (memberExp is null)
            {
                throw new ArgumentException($"{nameof(propertyMap)} is not of type {nameof(MemberExpression)}.");
            }

            var property = memberExp.Member as PropertyInfo;
            if (property is null)
            {
                throw new ArgumentException("Couldn't extract property from member expression.");
            }

            var initialValue = property.GetValue(state);
            var (prop, setProp) = UseState((T)initialValue);

            Action<T> persistedSetProp = p =>
            {
                property.SetValue(state, p);
                setProp(p);
            };

            return (prop, persistedSetProp);
        }

        protected virtual TState GetStateProperty()
        {
            const string defaultStatePropertyName = "Props";
            var stateProperty = GetType().GetProperty(defaultStatePropertyName);
            if (stateProperty is null)
            {
                throw new StatePropertyNotFoundException($"Couldn't find property '{defaultStatePropertyName}' on component {GetType()}");
            }

            if (stateProperty.PropertyType != typeof(TState))
            {
                throw new IncorrectPropertyTypeException($"Expected '{defaultStatePropertyName}' to be of type {typeof(TState)}, but found {stateProperty.PropertyType}");
            }

            return (TState)stateProperty.GetValue(this);
        }
    }
}
