using BlazorState.Redux.Interfaces;
using BlazorState.Redux.Sample.State.Types;

namespace BlazorState.Redux.Sample.State.Actions
{
    public class ReceiveWeatherForecastsAction : IAction
    {
        public WeatherForecast[] Forecasts { get; set; }
    }
}
