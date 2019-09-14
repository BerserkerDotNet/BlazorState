namespace BlazorState.Redux.Interfaces
{
    public interface IActionResolver
    {
        T Resolve<T>();
    }
}
