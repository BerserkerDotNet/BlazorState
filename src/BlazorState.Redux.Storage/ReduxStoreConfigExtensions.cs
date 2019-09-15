using System.Linq;
using Blazor.Extensions.Storage;
using BlazorState.Redux.Configuration;
using BlazorState.Redux.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Redux.Storage
{
    public static class ReduxStoreConfigExtensions
    {
        public static void UseLocalStorage<TState>(this ReduxStoreConfig<TState> config, string key = "AppState")
            where TState : new()
        {
            var storageRegistrations = config.Services.Where(s => s.ServiceType == typeof(IStateStorage)).ToArray();
            foreach (var storageRegistration in storageRegistrations)
            {
                config.Services.Remove(storageRegistration);
            }

            config.Services.AddSingleton<LocalStorage>();
            config.Services.AddSingleton<IStateStorage>(s => new LocalStorageProvider(key, s.GetService<LocalStorage>()));
        }
    }
}
