using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Blazor
{
    public abstract class ConnectedComponentBase<TState, TProps> : ComponentBase, IDisposable
        where TProps : new()
    {
        protected TProps Props { get; private set; }

        [Inject]
        protected IStore<TState> Store { get; set; }

        public void Dispose()
        {
            Store.OnStateChanged -= OnStateChanged;
        }

        protected abstract void MapStateToProps(TState state, TProps props);

        protected abstract void MapDispatchToProps(IStore<TState> store, TProps props);

        protected virtual Task Init(IStore<TState> store)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            Store.OnStateChanged += OnStateChanged;
            await InitializeProps();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            MapStateToProps(Store.State, Props);
            this.StateHasChanged();
        }

        private async ValueTask InitializeProps()
        {
            if (Props == null)
            {
                Props = new TProps();
                MapDispatchToProps(Store, Props);
                MapStateToProps(Store.State, Props);
            }

            await Init(Store);
        }
    }
}
