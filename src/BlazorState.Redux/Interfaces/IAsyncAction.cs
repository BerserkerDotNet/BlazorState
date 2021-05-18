using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IAsyncAction<TProperty> : IAction
    {
        Task Execute(IDispatcher dispatcher, TProperty property);
    }

    public interface IAsyncAction : IAction
    {
        Task Execute(IDispatcher dispatcher);
    }
}
