using System;
using System.Threading.Tasks;
using BlazorState.Redux.DevTools;
using BlazorState.Redux.Exceptions;
using BlazorState.Redux.Interfaces;
using BlazorState.Redux.Tests.Types;
using BlazorState.Redux.Tests.Types.Actions;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BlazorState.Redux.Tests
{
    [TestFixture]
    public class StoreTests
    {
        private Mock<IReducer<EmptyState>> _rootReducer;
        private Mock<IActionResolver> _actionResolver;
        private Mock<IStateStorage> _storageMock;
        private Mock<INavigationTracker<EmptyState>> _navigationMock;
        private Mock<IDevToolsInterop> _devToolsMock;
        private Store<EmptyState> _store;

        [SetUp]
        public void SetUp()
        {
            _rootReducer = new Mock<IReducer<EmptyState>>();
            _actionResolver = new Mock<IActionResolver>();
            _storageMock = new Mock<IStateStorage>();
            _navigationMock = new Mock<INavigationTracker<EmptyState>>();
            _devToolsMock = new Mock<IDevToolsInterop>();
            _store = new Store<EmptyState>(_rootReducer.Object, _actionResolver.Object, _storageMock.Object, _navigationMock.Object, _devToolsMock.Object);
        }

        [Test]
        public async Task ShouldInitializeOnlyOnce()
        {
            var state = new EmptyState();
            _storageMock.Setup(s => s.Get<EmptyState>())
                .ReturnsAsync(state);

            using var monitoredStore = _store.Monitor();

            await _store.Initialize();
            await _store.Initialize();

            _store.State.Should().BeSameAs(state);
            monitoredStore.Should().Raise(nameof(_store.OnStateChanged));
            monitoredStore.OccurredEvents.Should().HaveCount(1);
            _navigationMock.Verify(n => n.Start(_store), Times.Once());
            _navigationMock.Verify(n => n.Navigate(state), Times.Once());
            _storageMock.Verify(s => s.Get<EmptyState>(), Times.Once());
        }

        [Test]
        public async Task ShouldInitializeDevToolsOnlyOnce()
        {
            _devToolsMock.SetupAdd(d => d.OnJumpToStateChanged += It.IsAny<EventHandler<JumpToStateEventArgs>>());

            await _store.InitializeDevTools();
            await _store.InitializeDevTools();

            _devToolsMock.Verify(d => d.SendInitial(null), Times.Once());
            _devToolsMock.VerifyAdd(d => d.OnJumpToStateChanged += It.IsAny<EventHandler<JumpToStateEventArgs>>(), Times.Once());
        }

        [Test]
        public void DispatchShouldThrowIfActionIsNull()
        {
            _store.Invoking(s => s.Dispatch(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DispatchShouldTriggerReducerFlowAndUpdateState()
        {
            var initialState = new EmptyState();
            var newState = new EmptyState();
            var action = new EmptyAction();
            _rootReducer.Setup(r => r.Reduce(It.IsAny<EmptyState>(), It.IsAny<IAction>()))
                .Returns(newState);

            _store.Dispatch(action);

            _store.State.Should().BeSameAs(newState);
            _rootReducer.Verify(r => r.Reduce(It.IsAny<EmptyState>(), It.IsAny<IAction>()), Times.Once());
            _storageMock.Verify(s => s.Save(newState), Times.Once());
            _devToolsMock.Verify(s => s.Send(action, newState), Times.Once());
        }

        [Test]
        public void ShouldThrowOnExecuteAsyncActionIfActionIsNotRegistered()
        {
            _store.Awaiting(s => s.Dispatch<EmptyAsyncAction>())
                .Should().Throw<ActionIsNotRegisteredException>();
        }

        [Test]
        public async Task ShouldExecuteAsyncAction()
        {
            var actionMock = new Mock<IAsyncAction>();
            _actionResolver.Setup(r => r.Resolve<EmptyAsyncAction>())
                .Returns(new EmptyAsyncAction(actionMock.Object));

            await _store.Dispatch<EmptyAsyncAction>();

            actionMock.Verify(a => a.Execute(_store), Times.Once());
        }

        [Test]
        public async Task ShouldExecuteAsyncActionWithParameters()
        {
            var parameter = new EmptyState();
            var actionMock = new Mock<IAsyncAction<EmptyState>>();
            _actionResolver.Setup(r => r.Resolve<EmptyAsyncActionWithParameter>())
                .Returns(new EmptyAsyncActionWithParameter(actionMock.Object));

            await _store.Dispatch<EmptyAsyncActionWithParameter, EmptyState>(parameter);

            actionMock.Verify(a => a.Execute(_store, parameter), Times.Once());
        }

        [Test]
        public void ThrowOnExecuteAsyncActionWithParametrersIfActionIsNotRegistered()
        {
            _store.Awaiting(s => s.Dispatch<EmptyAsyncActionWithParameter, EmptyState>(new EmptyState()))
                .Should().Throw<ActionIsNotRegisteredException>();
        }

        [Test]
        public async Task ShouldReactToHistoricalDebugging()
        {
            var state = new EmptyState();
            var stateJson = JsonConvert.SerializeObject(state);

            await _store.Initialize();
            await _store.InitializeDevTools();
            _devToolsMock.Raise(d => d.OnJumpToStateChanged += null, new JumpToStateEventArgs(stateJson));

            _store.State.Should().NotBeNull();
            _navigationMock.Verify(n => n.Navigate(It.IsAny<EmptyState>()), Times.Exactly(2));
        }
    }
}
