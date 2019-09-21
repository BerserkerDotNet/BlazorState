# BlazorState.Redux

As the name suggests it is a port of [React-Redux][1] library to Blazor/.NET world.

[![Build Status](https://berserkerdotnet.visualstudio.com/BlazorState.Redux/_apis/build/status/BerserkerDotNet.BlazorState.Redux?branchName=master)](https://berserkerdotnet.visualstudio.com/BlazorState.Redux/_build/latest?definitionId=8&branchName=master)

[![Nuget](https://buildstats.info/nuget/BlazorState.Redux?v=1.0.0)](https://www.nuget.org/packages/BlazorState.Redux)

[![Nuget](https://buildstats.info/nuget/BlazorState.Redux.Storage?v=1.0.0)](https://www.nuget.org/packages/BlazorState.Redux.Storage)

- [BlazorState.Redux](#blazorstateredux)
  - [Why should I care?](#why-should-i-care)
  - [Installing](#installing)
  - [Shaping the state of the application](#shaping-the-state-of-the-application)
  - [Boilerplate code](#boilerplate-code)
  - [Defining components](#defining-components)
  - [Making some actions](#making-some-actions)
  - [Reducers](#reducers)
  - [Mapping reducers to state](#mapping-reducers-to-state)
  - [Render component](#render-component)
  - [DevTools](#devtools)
  - [Async actions](#async-actions)
  - [Location tracking](#location-tracking)
  - [Persisting and restoring state](#persisting-and-restoring-state)

## Why should I care?
Redux is a popular library for managing state in SPA applications.
Key benefits of using Redux:
1. Single source of truth. The whole state of the application is in one place, the store. 
2. It helps to enforce [unidirectional data flow][2] in the application, thus making state mutations more predictable and easier to understand.
3. It helps separate presentational components from container components (state aware components).
4. Great DevTools makes it easy to see how the state of the application is changing in time.

More on Redux [here][3].

Refer to [samples][4] in the repository for usage examples.

## Installing
Install `BlazorState.Redux` from NuGet
   ```powershell
   Install-Package BlazorState.Redux
   ```
   or
   ```bash
   dotnet add package BlazorState.Redux
   ```

## Shaping the state of the application

The state in Redux is a tree. Here is an example of the state object for a sample application:
```csharp
    public class RootState
    {
        public int Count { get; set; }

        public WeatherState Weather { get; set; }
    }
```
where `WeatherState` is:
```csharp
    public class WeatherState
    {
        public WeatherState(IEnumerable<WeatherForecast> forecasts)
        {
            Forecasts = forecasts;
        }

        public IEnumerable<WeatherForecast> Forecasts { get; private set; }
    }
```
**Note:** Name `RootState` is completely arbitrary and can be anything.

In Redux, state is immutable, meaning that if it needs to be mutated a new state object is created. For that purpose, `WeatherState` does not expose setter of its properties, and doesn't not have a default constructor. 

`RootState` itself is handled by a out of the box reducer and don't must have a default constructor.

## Boilerplate code

Now, when state shape is defined, some configuration needs to be put in place for Redux to work.

In `Startup.cs` add the following line to the `ConfigureServices` method:
```csharp
services.AddReduxStore<RootState>(cfg =>
{
    // TODO: Configure reducers and actions
});
```

This registers all services needed for Redux to function properly. Also, it provides a configurator that can be used to configure reducers, actions, and other options.
See `Configuration` section below.

Finally, in `App.razor` add a `BlazorRedux` component before `Router` component.

```html
<BlazorRedux />
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

For the component to be found `@using BlazorState.Redux.Blazor` statement needs to be added to `_Imports.razor`.

`BlazorRedux` component is responsible for bootstrapping Redux store and dev tools, if enabled.

## Defining components

As in `React-Redux` there are same two kinds of component in `Blazor-Redux`.
First, is presentational component, this component does not have access to the application state and takes all it needs from `props` object that is passed to it. Similarly, the outgoing communication is also done through callbacks, that are passed with `props`. In simple words, this component only renders data that is passed to it.

Here is an example of the `Counter` presentational component:
```html
<h1>Counter</h1>

<p>Current count: @Props.Count</p>

<button class="btn btn-primary" @onclick="Props.IncrementByOne">+1</button>
<button class="btn btn-primary" @onclick="() => Props.IncrementBy.InvokeAsync(5)">+5</button>
<button class="btn btn-primary" @onclick="() => Props.IncrementBy.InvokeAsync(10)">+10</button>

<button class="btn btn-primary" @onclick="Props.DecrementByOne">-1</button>
<button class="btn btn-primary" @onclick="() => Props.DecrementBy.InvokeAsync(5)">-5</button>
<button class="btn btn-primary" @onclick="() => Props.DecrementBy.InvokeAsync(10)">-10</button>

<button class="btn btn-danger" @onclick="Props.Reset">Reset</button>

@code {
    [Parameter] public CounterProps Props { get; set; }
}
```
where `CounterProps` is

```csharp
public class CounterProps
{
    public int Count { get; set; }

    public EventCallback IncrementByOne { get; set; }

    public EventCallback<int> IncrementBy { get; set; }

    public EventCallback DecrementByOne { get; set; }

    public EventCallback<int> DecrementBy { get; set; }

    public EventCallback Reset { get; set; }
}
```

Second, is container component, or connected component. It is called connected, because it knows about the store and it is responsible for mapping the state to the props and dispatching actions in response to presentational component callbacks.

Here is an example of the `CounterConnected` component:
```csharp
public class CounterConnected
{
    public static RenderFragment Get()
    {
        var c = new CounterConnected();
        return ComponentConnector.Connect<Counter, RootState, CounterProps>(c.MapStateToProps, c.MapDispatchToProps);
    }

    private void MapStateToProps(RootState state, CounterProps props)
    {
        props.Count = state?.Count ?? 0;
    }

    private void MapDispatchToProps(IStore<RootState> store, CounterProps props)
    {
        props.IncrementByOne = EventCallback.Factory.Create(this, () =>
        {
            store.Dispatch(new IncrementByOneAction());
        });

        props.IncrementBy = EventCallback.Factory.Create<int>(this, amount =>
        {
            store.Dispatch(new IncrementByAction { Amount = amount });
        });

        props.DecrementByOne = EventCallback.Factory.Create(this, () =>
        {
            store.Dispatch(new DecrementByOneAction());
        });

        props.DecrementBy = EventCallback.Factory.Create<int>(this, amount =>
        {
            store.Dispatch(new DecrementByAction { Amount = amount });
        });

        props.Reset = EventCallback.Factory.Create(this, () =>
        {
            store.Dispatch(new ResetCountAction());
        });
    }
}
```
This is a lot of code, so, lets unpack what's going on.

There is a static `Get` method that returns a `RenderFragment`. This method uses `ComponentConnector.Connect` method to connect container component to the store. This method accepts the following 2 parameters:
1. MapStateToProps - method responsible for mapping state object to presentational component `props` object. This method is called every time the state changes.
2. MapDispatchToProp - method responsible for mapping presentational components callbacks to dispatch methods. This method is called once, when component initializes.

## Making some actions
Actions are payloads of information that send data from your application to your store. They are the only source of information for the store. You send them to the store using `store.Dispatch()`.

Example of `IncrementByAction` action:
```csharp
public class IncrementByAction : IAction
{
    public int Amount { get; set; }
}
```

Actions must implement `IAction` interface, otherwise they are Plain Old CLR Objects classes.

## Reducers
Reducers specify how the application's state changes in response to actions sent to the store.
Here is an example of the `CountReducer`:
```csharp
public class CountReducer : IReducer<int>
{
    public int Reduce(int state, IAction action)
    {
        switch (action)
        {
            case IncrementByOneAction _:
                return state + 1;
            case DecrementByOneAction _:
                return state - 1;
            case IncrementByAction a:
                return state + a.Amount;
            case DecrementByAction a:
                return state - a.Amount;
            case ResetCountAction _:
                return 0;
            default:
                return state;
        }
    }
}
```

Reducer must implement `IReducer<TState>` interface, where `TState` is a type of state this particular reducer handles.
In the example above, the state this reducer handles is of type `int`, but it can be any C# object.
It is important to remember that reducer is a pure function, it should not produce side effects. Reducer must always return new state and should never modify existing state.
For example, `WeatherReducer` will look like this:
```csharp
public class WeatherReducer : IReducer<WeatherState>
{
    public WeatherState Reduce(WeatherState state, IAction action)
    {
        switch (action)
        {
            case ReceiveWeatherForecastsAction a:
                return new WeatherState(a.Forecasts);
            default:
                return state;
        }
    }
}
```

## Mapping reducers to state

Once reducers defined, they need to be mapped to the corresponding state property that they handle. This is done in `Startup.cs` using config object of the `AddReduxStore` method:
```csharp
services.AddReduxStore<RootState>(cfg =>
{
    cfg.Map<CountReducer, int>(s => s.Count);
    cfg.Map<WeatherReducer, WeatherState>(s => s.Weather);
});
```

Note: Reducer must have a default parameterless constructor.

## Render component

When all necessary components are in place, it is time to place component(s) on the page.
This is typically done by using static `Get` method on the connected component. See section `Defining components` for more details. This method returns `RenderFragment` that can be directly rendered on the page. Clean and simple.

Here is an example of the `Counter` page rendering `Counter` component.
```razor
@page "/counter"

@CounterCmp

@code {
    RenderFragment CounterCmp = CounterConnected.Get();
}
```

## DevTools
To configure DevTools, simply add `cfg.UseReduxDevTools();` to the configuration callback in `Startup.cs`:
```csharp
services.AddReduxStore<RootState>(cfg =>
{
    cfg.UseReduxDevTools();
    cfg.Map<CountReducer, int>(s => s.Count);
    cfg.Map<WeatherReducer, WeatherState>(s => s.Weather);
});
```

Assuming that ReduxDevTools is installed, open your browser of choice developer tools and enjoy time travel debugging and other goodness of ReduxDevTools.

## Async actions
Actions we've seen so far were synchronous, but sooner or later application needs to make asynchronous request for data to the server. For such a case BlazorState.Redux has a concept of async actions.

Here is an example of async action fetching weather information from the server:
```csharp
public class FetchWeather : IAsyncAction
{
    private readonly HttpClient _http;

    public FetchWeather(HttpClient http)
    {
        _http = http;
    }

    public async Task Execute(IDispatcher dispatcher)
    {
        var forecasts = await _http.GetJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        dispatcher.Dispatch(new ReceiveWeatherForecastsAction
        {
            Forecasts = forecasts
        });
    }
}
```

Async action must implement `IAsyncAction` or `IAsyncAction<TParam>` interface. Both interfaces have only one method, `Execute`, the difference is that `IAsyncAction<TParam>` expect a second parameter. This parameter is a user-defined object of any type.
For example:
```csharp
public class DeleteIdentity : IAsyncAction<IdentityViewModel>
{
    private readonly HttpClient _client;

    public DeleteIdentity(HttpClient client)
    {
        _client = client;
    }

    public async Task Execute(IDispatcher dispatcher, IdentityViewModel identity)
    {
        await _client.Delete<IdentityViewModel>(identity.Id);
        await dispatcher.Dispatch<FetchIdentities>();
    }
}
```

To dispatch async action, use the `store.Dispatch<TAsyncAction>` method.
```csharp
private async Task Init(IStore<RootState> store)
{
    await store.Dispatch<FetchWeather>();
}
```

Last, but not least, actions must be registered in `Startup.cs`.
There are two options:
1. Register each action separately. `cfg.RegisterAsyncAction<WeatherForecast>();`
2. Register all actions in assembly. `cfg.RegisterActionsFromAssemblyContaining<FetchWeather>();`

```csharp
services.AddReduxStore<RootState>(cfg =>
{
    cfg.RegisterActionsFromAssemblyContaining<FetchWeather>();
});
```

## Location tracking
In some cases it is desirable to store current page address in the state. Once such case might be to navigate user back to the same page where he left during last session.
This is supported by Redux out of the box, and can be configured by adding `cfg.TrackUserNavigation(s => s.Location);` to `Startup.cs`:
```csharp
services.AddReduxStore<RootState>(cfg =>
{
    cfg.TrackUserNavigation(s => s.Location);
});
```
`s => s.Location` is the property on the RootState. This property must be of type string.
When configured, special `NavigationAction` will be dispatched every time user navigates to another page.
![Navigation Action Example](assets/img/NavigationActionExample.png)

If state is persisted, Redux will handle navigating user back to the page stored in the state on the first request.

## Persisting and restoring state
This is done automatically by Redux, the only action required is configuring state storage.
BlazorState.Redux has out of the box implementation that uses browser Local Storage to persist and restore state. To use it, install `Blazor.Redux.Storage` NuGet package and add `cfg.UseLocalStorage();` to the config callback in `Startup.cs`:

```csharp
services.AddReduxStore<RootState>(cfg =>
{
    cfg.UseLocalStorage();
});
```

[1]: https://react-redux.js.org/introduction/quick-start
[2]: https://redux.js.org/basics/data-flow
[3]: https://redux.js.org/introduction/core-concepts
[4]: https://github.com/BerserkerDotNet/BlazorState.Redux/tree/master/samples/BlazorState.Redux.Sample
