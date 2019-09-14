using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Sample.State.Actions
{
    public class IncrementByAction : IAction
    {
        public int Amount { get; set; }
    }
}
