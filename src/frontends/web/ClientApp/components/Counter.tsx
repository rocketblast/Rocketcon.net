import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState }  from '../store';
import * as CounterStore from '../store/Counter';
import * as WeatherForecasts from '../store/WeatherForecasts';
import 'whatwg-fetch'

type CounterProps =
    CounterStore.CounterState
    & typeof CounterStore.actionCreators
    & RouteComponentProps<{}>;

class Counter extends React.Component<CounterProps, {}> {
    public render() {
        return <div>
            <h1>Counter</h1>

            <p>This is a simple example of a React component.</p>

            <p>Current count: <strong>{ this.props.count }</strong></p>

            <button onClick={() => { this.props.increment() }}>Increment</button>
            <button onClick={() => { this.props.decrement() }}>Decrement</button>

            <p>
                <button onClick={() => {
                    //https://github.com/dotnet/orleans/blob/master/src/OrleansProviders/Storage/MemoryStorage.cs
                    fetch('api/value/' + this.props.count).then(function (data) {
                        var test = data.json();
                        console.log("response:", data);
                        console.log("json response", test)
                    });
                }}>Fetch data</button>

                <button onClick={() => {
                    fetch('api/value/' + this.props.count + '?value=testdata', {
                        method: "POST"
                    }).then(function (data) {
                        var test = data.json();
                        console.log("response:", data);
                        console.log("json response", data)
                    });
                }}>Send data</button>
            </p>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.counter, // Selects which state properties are merged into the component's props
    CounterStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Counter) as typeof Counter;