using System;
using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorState.Redux.Blazor
{
    public class BlazorRedux : ComponentBase
    {
        [Inject]
        protected IStoreInitializer StoreInitializer { get; set; }

        [Parameter]
        public bool DevTools { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (DevTools)
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

                if (DevTools)
                {
                    await StoreInitializer.InitializeDevTools();
                }
            }
        }
    }
}
