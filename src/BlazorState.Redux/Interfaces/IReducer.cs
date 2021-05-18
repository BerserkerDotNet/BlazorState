namespace BlazorState.Redux.Interfaces
{
    public interface IReducer<TState>
    {
        TState Reduce(TState state, IAction action);
    }
}
