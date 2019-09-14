using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorState.Redux.DevTools
{
    public class ReduxDevTools : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenElement(1, "script");
            builder.AddAttribute(2, "src", "/js/reduxdevtools.js");
            builder.CloseElement();
        }
    }
}
