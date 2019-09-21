using System;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux
{
    public class BlazorActionResolver : IActionResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public BlazorActionResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Resolve<T>()
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }
    }
}
