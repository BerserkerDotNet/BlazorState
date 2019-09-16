using System;
using System.Threading.Tasks;
using BlazorState.Redux.DevTools;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorState.Redux.Blazor
{
    public class BlazorRedux : ComponentBase
    {
        [Inject]
        protected IStoreInitializer StoreInitializer { get; set; }

        [Inject]
        protected IDevToolsInterop DevTools { get; set; }

        protected bool UseDevTools => !(DevTools is NullDevToolsInterop);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (UseDevTools)
            {
                builder.OpenElement(1, "script");
                builder.AddAttribute(2, "src", "/js/reduxdevtools.js");
                builder.CloseElement();
            }

            base.BuildRenderTree(builder);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await StoreInitializer.Initialize();

                if (UseDevTools)
                {
                    await StoreInitializer.InitializeDevTools();
                }
            }
        }
    }
}
