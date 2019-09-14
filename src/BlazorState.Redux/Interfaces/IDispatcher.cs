using System.Threading.Tasks;

namespace BlazorState.Redux.Interfaces
{
    public interface IDispatcher
    {
        void Dispatch(IAction action);

        Task Dispatch<TAsyncAction, TProperty>(TProperty property)
            where TAsyncAction : IAsyncAction<TProperty>;

        Task Dispatch<TAsyncAction>()
            where TAsyncAction : IAsyncAction;
    }
}
