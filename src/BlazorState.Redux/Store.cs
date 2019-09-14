using System;
using System.Threading.Tasks;
using BlazorState.Redux.DevTools;
using BlazorState.Redux.Interfaces;
using Newtonsoft.Json;

namespace BlazorState.Redux
{
    public class Store<TState> : IStore<TState>, IDisposable
    {
        private readonly IReducer<TState> _rootReducer;
        private readonly IActionResolver _actionResolver;
        private readonly IStateStorage _storage;
        private readonly INavigationTracker<TState> _navigationTracker;
        private readonly IDevToolsInterop _devToolsInterop;
        private bool _isInitialized = false;

        public Store(IReducer<TState> rootReducer, IActionResolver actionResolver, IStateStorage storage, INavigationTracker<TState> navigationTracker, IDevToolsInterop devToolsInterop)
        {
            _rootReducer = rootReducer;
            _actionResolver = actionResolver;
            _storage = storage;
            _navigationTracker = navigationTracker;
            _devToolsInterop = devToolsInterop;
        }

        public event EventHandler<EventArgs> OnStateChanged;

        public TState State { get; private set; }

        public async ValueTask Initialize()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _navigationTracker.Start(this);
                var state = await _storage.Get<TState>("AppState");
                SetState(state);
                await _devToolsInterop.Init(state);
                _devToolsInterop.OnJumpToStateChanged += InteropOnJumpToStateChanged;
            }
        }

        public void Dispatch(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            SetState(_rootReducer.Reduce(State, action));
            _storage.Save("AppState", State);
            _devToolsInterop.Send(action, State);
        }

        public async Task Dispatch<TAsyncAction, TProperty>(TProperty property)
            where TAsyncAction : IAsyncAction<TProperty>
        {
            var action = _actionResolver.Resolve<TAsyncAction>();
            await action.Execute(this, property);
        }

        public async Task Dispatch<TAsyncAction>()
            where TAsyncAction : IAsyncAction
        {
            var action = _actionResolver.Resolve<TAsyncAction>();
            await action.Execute(this);
        }

        public void Dispose()
        {
            _devToolsInterop.OnJumpToStateChanged -= InteropOnJumpToStateChanged;
        }

        private void InteropOnJumpToStateChanged(object sender, JumpToStateEventArgs e)
        {
            var state = string.IsNullOrEmpty(e.StateJson) ? default : JsonConvert.DeserializeObject<TState>(e.StateJson);
            SetState(state);
            _navigationTracker.Navigate(state);
        }

        private void SetState(TState state)
        {
            State = state;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
