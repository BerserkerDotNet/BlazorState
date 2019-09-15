using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorState.Redux.Blazor
{
    public class ComponentConnected<TComponent, TState, TProps> : ComponentBase, IDisposable
        where TComponent : ComponentBase
        where TProps : new()
    {
        private TProps _props;

        [Inject]
        protected IStore<TState> Store { get; set; }

        [Parameter]
        public Action<TState, TProps> MapStateToProps { get; set; }

        [Parameter]
        public Action<IStore<TState>, TProps> MapDispatchToProps { get; set; }

        [Parameter]
        public Func<IStore<TState>, Task> Init { get; set; }

        public void Dispose()
        {
            Store.OnStateChanged -= OnStateChanged;
        }

        protected override void OnInitialized()
        {
            InitializeProps();
            Store.OnStateChanged += OnStateChanged;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (MapStateToProps == null || MapDispatchToProps == null)
            {
                throw new ArgumentNullException($"Connect requires both {nameof(MapStateToProps)} and ${nameof(MapDispatchToProps)} to be set.");
            }

            if (Init != null)
            {
                await Init(Store);
            }
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

        private void InitializeProps()
        {
            if (_props == null)
            {
                _props = new TProps();
                MapDispatchToProps(Store, _props);
                MapStateToProps(Store.State, _props);
            }
        }
    }
}
