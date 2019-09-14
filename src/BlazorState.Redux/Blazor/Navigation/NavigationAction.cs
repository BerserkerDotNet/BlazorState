using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Blazor.Navigation
{
    public class NavigationAction : IAction
    {
        public string Url { get; set; }
    }
}
