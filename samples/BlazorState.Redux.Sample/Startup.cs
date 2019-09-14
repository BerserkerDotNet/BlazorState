using BlazorState.Redux.Extensions;
using BlazorState.Redux.Sample.State.Actions;
using BlazorState.Redux.Sample.State.Reducers;
using BlazorState.Redux.Sample.State.Types;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Redux.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReduxStore<RootState>(cfg =>
            {
                cfg.UseReduxDevTools();
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
