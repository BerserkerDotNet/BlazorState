using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorState.Sample.Components
{
    public class WeatherProps
    {
        public IEnumerable<WeatherForecast> Forecasts { get; set; }

        public EventCallback AddRandomForecast { get; set; }
    }
}
