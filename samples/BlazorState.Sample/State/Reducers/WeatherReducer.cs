using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Types;

namespace BlazorState.Sample.State.Reducers
{
    public class WeatherReducer : IReducer<WeatherState>
    {
        public WeatherState Reduce(WeatherState state, IAction action)
        {
            switch (action)
            {
                case ReceiveWeatherForecastsAction a:
                    return new WeatherState(a.Forecasts);
                default:
                    return state;
            }
        }
    }
}
