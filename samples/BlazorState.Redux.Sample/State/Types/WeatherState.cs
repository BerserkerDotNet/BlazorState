using System.Collections.Generic;

namespace BlazorState.Redux.Sample.State.Types
{
    public class WeatherState
    {
        public WeatherState(IEnumerable<WeatherForecast> forecasts)
        {
            Forecasts = forecasts;
        }

        public IEnumerable<WeatherForecast> Forecasts { get; private set; }
    }
}
