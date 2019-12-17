using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorState.Sample.State.Actions
{
    public class FetchWeather : IAsyncAction
    {
        private readonly HttpClient _http;

        public FetchWeather(HttpClient http)
        {
            _http = http;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var forecasts = await _http.GetJsonAsync<WeatherForecast[]>("sample-data/weather.json");
            dispatcher.Dispatch(new ReceiveWeatherForecastsAction
            {
                Forecasts = forecasts
            });
        }
    }
}
