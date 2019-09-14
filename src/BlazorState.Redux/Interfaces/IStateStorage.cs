using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IStateStorage
    {
        ValueTask Save<T>(string key, T state);

        ValueTask<T> Get<T>(string key);
    }
}
