using System;
using BlazorState.Redux.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorState.Redux.Blazor.Navigation
{
    public class NavigationTracker<TState> : INavigationTracker<TState>
    {
        private readonly Func<TState, string> _property;
        private readonly NavigationManager _navigation;
        private IDispatcher _dispatcher;
        private bool _isTimeTraveling = false;

        public NavigationTracker(Func<TState, string> property, NavigationManager navigation)
        {
            _property = property;
            _navigation = navigation;
        }

        public void Dispose()
        {
            _navigation.LocationChanged -= OnLocationChanged;
        }

        public void Start(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _navigation.LocationChanged += OnLocationChanged;
        }

        public void Navigate(TState state)
        {
            try
            {
                if (state == null)
                {
                    return;
                }

                _isTimeTraveling = true;
                _navigation.NavigateTo(_property(state));
            }
            finally
            {
                _isTimeTraveling = false;
            }
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (!_isTimeTraveling)
            {
                _dispatcher.Dispatch(new NavigationAction
                {
                    Url = e.Location
                });
            }
        }
    }
}
