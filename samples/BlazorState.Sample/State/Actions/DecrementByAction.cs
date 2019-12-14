using BlazorState.Redux.Interfaces;

namespace BlazorState.Sample.State.Actions
{
    public class DecrementByAction : IAction
    {
        public int Amount { get; set; }
    }
}
