using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux
{
    public class NullStateStorage : IStateStorage
    {
        public ValueTask<T> Get<T>()
        {
            return new ValueTask<T>(Task.FromResult<T>(default));
        }

        public ValueTask Save<T>(T state)
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
