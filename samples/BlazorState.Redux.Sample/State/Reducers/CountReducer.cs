using BlazorState.Redux.Interfaces;
using BlazorState.Redux.Sample.State.Actions;

namespace BlazorState.Redux.Sample.State.Reducers
{
    public class CountReducer : IReducer<int>
    {
        public int Reduce(int state, IAction action)
        {
            switch (action)
            {
                case IncrementByOneAction _:
                    return state + 1;
                case DecrementByOneAction _:
                    return state - 1;
                case IncrementByAction a:
                    return state + a.Amount;
                case DecrementByAction a:
                    return state - a.Amount;
                case ResetCountAction _:
                    return 0;
                default:
                    return state;
            }
        }
    }
}
