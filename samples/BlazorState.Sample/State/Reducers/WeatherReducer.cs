using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Types;
using System;
using System.Collections.Generic;

namespace BlazorState.Sample.State.Reducers
{
    public class WeatherReducer : IReducer<WeatherState>
    {
        private static Random random = new Random();

        public WeatherState Reduce(WeatherState state, IAction action)
        {
            switch (action)
            {
                case ReceiveWeatherForecastsAction a:
                    return new WeatherState(a.Forecasts);
                case AddRandomForecast a:
                    var forecasts = new List<WeatherForecast>(state.Forecasts);
                    forecasts.Add(new WeatherForecast
                    {
                        Date = DateTime.Today.AddDays(random.Next(1, 30)),
                        Summary = $"There is {random.Next(0, 100)}% chance of rain.",
                        TemperatureC = random.Next(10, 40)
                    });
                    return new WeatherState(forecasts);
                default:
                    return state;
            }
        }
    }
}
