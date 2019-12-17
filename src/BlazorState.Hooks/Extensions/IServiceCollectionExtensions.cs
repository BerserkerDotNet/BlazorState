using BlazorState.Hooks.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorState.Hooks.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddHooks(this IServiceCollection services)
        {
            services.AddSingleton<IHooksService, HooksService>();
        }
    }
}
