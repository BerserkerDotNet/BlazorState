using BlazorState.Redux.Blazor;
using BlazorState.Redux.Interfaces;
using BlazorState.Sample.State.Actions;
using BlazorState.Sample.State.Types;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Sample.Components
{
    public class CounterConnected : ConnectedComponent<Counter, RootState, CounterProps>
    {
        protected override void MapStateToProps(RootState state, CounterProps props)
        {
            props.Count = state?.Count ?? 0;
        }

        protected override void MapDispatchToProps(IStore<RootState> store, CounterProps props)
        {
            props.IncrementByOne = EventCallback.Factory.Create(this, () =>
            {
                store.Dispatch(new IncrementByOneAction());
            });

            props.IncrementBy = EventCallback.Factory.Create<int>(this, amount =>
            {
                store.Dispatch(new IncrementByAction { Amount = amount });
            });

            props.DecrementByOne = EventCallback.Factory.Create(this, () =>
            {
                store.Dispatch(new DecrementByOneAction());
            });

            props.DecrementBy = EventCallback.Factory.Create<int>(this, amount =>
            {
                store.Dispatch(new DecrementByAction { Amount = amount });
            });

            props.Reset = EventCallback.Factory.Create(this, () =>
            {
                store.Dispatch(new ResetCountAction());
            });
        }
    }
}
