using BlazorState.Sample.State.Types;
using System.Collections.Generic;

namespace BlazorState.Sample.Components
{
    public class WeatherProps
    {

        public IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
