using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IStateStorage
    {
        ValueTask Save<T>(T state);

        ValueTask<T> Get<T>();
    }
}
