using BlazorState.Hooks.Extensions;
using BlazorState.Redux.Extensions;
using BlazorState.Redux.Storage;
using BlazorState.Sample.Components.Props;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Reducers;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorState.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var state = new PersistedCounterState { User = new UserModel("John", "Doe") };
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton(state);
            builder.Services.AddHooks();
            builder.Services.AddReduxStore<RootState>(cfg =>
            {
                cfg.UseReduxDevTools();
                cfg.UseLocalStorage();
                cfg.TrackUserNavigation(s => s.Location);

                cfg.Map<CountReducer, int>(s => s.Count);
                cfg.Map<WeatherReducer, WeatherState>(s => s.Weather);

                cfg.RegisterActionsFromAssemblyContaining<FetchWeather>();
            });

            await builder.Build().RunAsync();
        }
    }
}
