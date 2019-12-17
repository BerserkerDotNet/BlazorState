using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Types;

namespace BlazorState.Sample.State.Actions
{
    public class ReceiveWeatherForecastsAction : IAction
    {
        public WeatherForecast[] Forecasts { get; set; }
    }
}
