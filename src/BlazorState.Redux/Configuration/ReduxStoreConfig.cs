using System;
using System.Linq;
using System.Linq.Expressions;
using BlazorState.Redux.Blazor.Navigation;
using BlazorState.Redux.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Redux.Configuration
{
    public class ReduxStoreConfig<TRootState>
         where TRootState : new()
    {
        private readonly ReducerMappingBuilder<TRootState> _reducerMapper;

        public ReduxStoreConfig(IServiceCollection services, ReducerMappingBuilder<TRootState> reducerMapper)
        {
            Services = services;
            _reducerMapper = reducerMapper;
        }

        public IServiceCollection Services { get; }

        internal bool UseDevTools { get; private set; }

        internal Func<TRootState, string> LocationProperty { get; private set; }

        internal Func<IServiceProvider, IStore<TRootState>> StoreActivator { get; private set; }

        public void UseReduxDevTools()
        {
            UseDevTools = true;
        }

        public void UseCustomStoreActivator(Func<IServiceProvider, IStore<TRootState>> storeActivator)
        {
            StoreActivator = storeActivator;
        }

        public void TrackUserNavigation(Expression<Func<TRootState, string>> property)
        {
            LocationProperty = property.Compile();
            Map<UserNavigationReducer, string>(property);
        }

        public void Map<TReducer, TProperty>(Expression<Func<TRootState, TProperty>> property)
            where TReducer : IReducer<TProperty>, new()
        {
            _reducerMapper.Map(property, new TReducer());
        }

        public void RegisterAsyncAction<TAsyncAction, TProperty>()
            where TAsyncAction : class, IAsyncAction<TProperty>
        {
            Services.AddTransient<TAsyncAction>();
        }

        public void RegisterAsyncAction<TAsyncAction>()
            where TAsyncAction : class, IAsyncAction
        {
            Services.AddTransient<TAsyncAction>();
        }

        public void RegisterActionsFromAssemblyContaining<TAsyncAction>()
            where TAsyncAction : class, IAsyncAction
        {
            var actionType = typeof(IAsyncAction);
            var actionTypeGeneric = typeof(IAsyncAction<>);
            var asyncActions = typeof(TAsyncAction).Assembly
                .GetTypes()
                .Where(t => t.IsClass && (actionType.IsAssignableFrom(t) || IsAssignableToGenericType(t, actionTypeGeneric)))
                .ToArray();

            foreach (var action in asyncActions)
            {
                Services.AddTransient(action);
            }
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

            var baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
