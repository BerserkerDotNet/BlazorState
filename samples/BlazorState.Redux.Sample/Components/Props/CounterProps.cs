using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Sample.Components
{
    public class CounterProps
    {
        public int Count { get; set; }

        public EventCallback IncrementByOne { get; set; }

        public EventCallback<int> IncrementBy { get; set; }

        public EventCallback DecrementByOne { get; set; }

        public EventCallback<int> DecrementBy { get; set; }

        public EventCallback Reset { get; set; }
    }
}
