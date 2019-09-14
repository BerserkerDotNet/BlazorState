using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Blazor
{
    public static class ComponentConnector
    {
        public static RenderFragment Connect<TComponent, TState, TProps>(
            Action<TState, TProps> mapStateToProps,
            Action<IStore<TState>, TProps> mapDispatchToProps)
                    where TComponent : ComponentBase
                    where TProps : new()
        {
            return Connect<TComponent, TState, TProps>(mapStateToProps, mapDispatchToProps, init: null);
        }

        public static RenderFragment Connect<TComponent, TState, TProps>(
            Action<TState, TProps> mapStateToProps,
            Action<IStore<TState>, TProps> mapDispatchToProps,
            Func<IStore<TState>, Task> init)
        where TComponent : ComponentBase
        where TProps : new()
        {
            var seq = 0;
            return new RenderFragment(builder =>
            {
                builder.OpenComponent<ComponentConnected<TComponent, TState, TProps>>(++seq);
                builder.AddAttribute(++seq, "MapStateToProps", mapStateToProps);
                builder.AddAttribute(++seq, "MapDispatchToProps", mapDispatchToProps);
                if (init != null)
                {
                    builder.AddAttribute(++seq, "Init", init);
                }

                builder.CloseComponent();
            });
        }
    }
}
