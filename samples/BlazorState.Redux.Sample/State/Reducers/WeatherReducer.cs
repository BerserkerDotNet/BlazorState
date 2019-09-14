using BlazorState.Redux.Interfaces;
using BlazorState.Redux.Sample.State.Actions;
using BlazorState.Redux.Sample.State.Types;

namespace BlazorState.Redux.Sample.State.Reducers
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
