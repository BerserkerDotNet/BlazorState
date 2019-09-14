using BlazorState.Redux.Sample.State.Types;
using System.Collections.Generic;

namespace BlazorState.Redux.Sample.Components
{
    public class WeatherProps
    {

        public IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
