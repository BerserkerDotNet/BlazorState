using System;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorState.Redux.Hooks
{
    public class HookedComponentBase : ComponentBase, IDisposable
    {
        [Inject]
        private IHookService Service { get; set; }

        public virtual void Dispose()
        {
            if (Service is object)
            {
                Service.ComponentDisposed(this);
            }
        }

        protected (T, Action<T>) UseState<T>(T initialState)
        {
            VerifyServiceIsNotNull();
            return Service.UseState(initialState, this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            VerifyServiceIsNotNull();
            Service.ComponentRendered(this);
        }

        private void VerifyServiceIsNotNull()
        {
            if (Service is null)
            {
                throw new ArgumentNullException("Hooks service is not initialized!");
            }
        }
    }
}
