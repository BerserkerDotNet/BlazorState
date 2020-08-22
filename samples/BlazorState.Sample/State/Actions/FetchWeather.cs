using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Types;
using System.Net.Http;
using System.Net.Http.Json;
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
            var forecasts = await _http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
            dispatcher.Dispatch(new ReceiveWeatherForecastsAction
            {
                Forecasts = forecasts
            });
        }
    }
}
