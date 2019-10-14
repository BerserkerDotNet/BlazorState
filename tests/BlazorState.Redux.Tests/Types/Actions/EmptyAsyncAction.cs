using System.Threading.Tasks;
using BlazorState.Redux.Interfaces;

namespace BlazorState.Redux.Tests.Types.Actions
{
    public class EmptyAsyncAction : IAsyncAction
    {
        private readonly IAsyncAction _action;

        public EmptyAsyncAction(IAsyncAction action)
        {
            _action = action;
        }

        public Task Execute(IDispatcher dispatcher)
        {
            return _action.Execute(dispatcher);
        }
    }
}
