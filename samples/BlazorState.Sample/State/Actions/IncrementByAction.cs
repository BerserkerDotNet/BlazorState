using BlazorState.Redux.Interfaces;

namespace BlazorState.Sample.State.Actions
{
    public class IncrementByAction : IAction
    {
        public int Amount { get; set; }
    }
}
