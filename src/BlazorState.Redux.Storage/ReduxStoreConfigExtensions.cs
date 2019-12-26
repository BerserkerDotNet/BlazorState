using System.Linq;
using BlazorState.Redux.Configuration;
using BlazorState.Redux.Interfaces;
using BlazorStorage.Extensions;
using BlazorStorage.Interfaces;
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

            config.Services.AddStorage();
            config.Services.AddSingleton<IStateStorage>(s => new LocalStorageProvider(key, s.GetService<ILocalStorage>()));
        }
    }
}
