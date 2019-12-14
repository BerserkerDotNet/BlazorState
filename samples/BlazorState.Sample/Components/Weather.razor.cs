using BlazorState.Redux.Blazor;
using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorState.Sample.Components
{
    public class WeatherConnected
    {
        public static RenderFragment Get()
        {
            var c = new WeatherConnected();
            return ComponentConnector.Connect<Weather, RootState, WeatherProps>(c.MapStateToProps, c.MapDispatchToProps, c.Init);
        }

        private async Task Init(IStore<RootState> store)
        {
            await store.Dispatch<FetchWeather>();
        }

        private void MapStateToProps(RootState state, WeatherProps props)
        {
            props.Forecasts = state?.Weather?.Forecasts;
        }

        private void MapDispatchToProps(IStore<RootState> store, WeatherProps props)
        {
        }
    }
}
