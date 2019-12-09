using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Tests.Types.Actions
{
    public class EmptyAsyncActionWithParameter : IAsyncAction<EmptyState>
    {
        private readonly IAsyncAction<EmptyState> _action;

        public EmptyAsyncActionWithParameter(IAsyncAction<EmptyState> action)
        {
            _action = action;
        }

        public Task Execute(IDispatcher dispatcher, EmptyState parameter)
        {
            return _action.Execute(dispatcher, parameter);
        }
    }
}
