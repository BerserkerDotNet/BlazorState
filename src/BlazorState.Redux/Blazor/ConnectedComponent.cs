using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorState.Redux.Blazor
{
    public abstract class ConnectedComponent<TComponent, TState, TProps> : ComponentBase, IDisposable
        where TComponent : ComponentBase
        where TProps : new()
    {
        private TProps _props;

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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<TComponent>(1);
            builder.AddAttribute(2, "Props", _props);
            builder.CloseComponent();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            MapStateToProps(Store.State, _props);
            this.StateHasChanged();
        }

        private async ValueTask InitializeProps()
        {
            if (_props == null)
            {
                _props = new TProps();
                MapDispatchToProps(Store, _props);
                MapStateToProps(Store.State, _props);
            }

            await Init(Store);
        }
    }
}
