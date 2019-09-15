using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IStoreInitializer
    {
        ValueTask Initialize();

        ValueTask InitializeDevTools();
    }
}
