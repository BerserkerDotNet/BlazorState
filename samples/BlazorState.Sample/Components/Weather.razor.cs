using BlazorState.Redux.Blazor;
using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorState.Sample.Components
{
    public class WeatherConnected : ConnectedComponent<Weather, RootState, WeatherProps>
    {
        protected async override Task Init(IStore<RootState> store)
        {
            await store.Dispatch<FetchWeather>();
        }

        protected override void MapStateToProps(RootState state, WeatherProps props)
        {
            props.Forecasts = state?.Weather?.Forecasts;
        }

        protected override void MapDispatchToProps(IStore<RootState> store, WeatherProps props)
        {
            props.AddRandomForecast = EventCallback.Factory.Create(this, () =>
            {
                store.Dispatch(new AddRandomForecast());
            });
        }
    }
}
