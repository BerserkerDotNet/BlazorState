using System;
using BlazorState.Redux.Blazor.Navigation;
using BlazorState.Redux.Configuration;
using BlazorState.Redux.DevTools;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Redux.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddReduxStore<TRootState>(this IServiceCollection services, Action<ReduxStoreConfig<TRootState>> cfg)
            where TRootState : new()
        {
            var reducerMapping = ReducerMappingBuilder<TRootState>.Create();
            var config = new ReduxStoreConfig<TRootState>(services, reducerMapping);
            cfg(config);
            var rootReducer = reducerMapping.Build();

            if (config.UseDevTools)
            {
                services.AddSingleton<IDevToolsInterop, ReduxDevToolsInterop>();
            }
            else
            {
                services.AddSingleton<IDevToolsInterop, NullDevToolsInterop>();
            }

            if (config.LocationProperty is object)
            {
                services.AddSingleton<INavigationTracker<TRootState>>(s => new NavigationTracker<TRootState>(config.LocationProperty, s.GetService<NavigationManager>()));
            }
            else
            {
                services.AddSingleton<INavigationTracker<TRootState>, NullNavigationTracker<TRootState>>();
            }

            var storeActivator = config.StoreActivator ?? (s => new Store<TRootState>(
                rootReducer,
                s.GetService<IActionResolver>(),
                s.GetService<IStateStorage>(),
                s.GetService<INavigationTracker<TRootState>>(),
                s.GetService<IDevToolsInterop>()));

            services.AddSingleton(storeActivator);
            services.AddSingleton<IActionResolver, BlazorActionResolver>();
            services.AddSingleton<IStateStorage, NullStateStorage>();
        }
    }
}
