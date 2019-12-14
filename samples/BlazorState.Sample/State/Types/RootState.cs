namespace BlazorState.Sample.State.Types
{
    public class RootState
    {
        public string Location { get; set; }

        public int Count { get; set; }

        public WeatherState Weather { get; set; }
    }
}
