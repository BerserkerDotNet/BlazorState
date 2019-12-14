using BlazorState.Hooks.Extensions;
using BlazorState.Redux.Extensions;
using BlazorState.Redux.Storage;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Reducers;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHooks();
            services.AddReduxStore<RootState>(cfg =>
            {
                cfg.UseReduxDevTools();
                cfg.UseLocalStorage();
                cfg.TrackUserNavigation(s => s.Location);

                cfg.Map<CountReducer, int>(s => s.Count);
                cfg.Map<WeatherReducer, WeatherState>(s => s.Weather);

                cfg.RegisterActionsFromAssemblyContaining<FetchWeather>();
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
