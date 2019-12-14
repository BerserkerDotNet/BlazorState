using System.Collections.Generic;

namespace BlazorState.Sample.State.Types
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
