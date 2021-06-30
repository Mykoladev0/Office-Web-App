import React, { Component } from 'react';
import { Route } from 'react-router-dom';
// import './App.css';

// import logo from './logo.svg';
import Shows from './components/Shows';
import { Layout } from './components/Layout';
import ShowEvents from './components/ShowEvents';
import ShowRegistration from './components/ShowRegistration';
import ShowFinalization from './components/ShowFinalization';

interface AppState {
  currentShow: any;
}

export default class App extends Component<any, AppState> {
  public displayName = App.name;

  public state: AppState = {
    currentShow: null,
  };

  public constructor(props: any) {
    super(props);
    this.handleShowChange = this.handleShowChange.bind(this);
  }

  public render() {
    const { currentShow } = this.state;
    return (
      <Layout currentShow={currentShow}>
        <Route
          exact={true}
          path="/"
          render={props => <Shows handleSetCurrentShowFn={this.handleShowChange} {...props} />}
        />
        {/* <Route exact={true} path="/" component={Shows} /> */}
        {/* <Route path="/counter" component={Counter} /> */}
        <Route path="/ShowEvents" component={ShowEvents} />
        <Route path="/ShowRegistration" component={ShowRegistration} />
        <Route path="/ShowFinalization" component={ShowFinalization} />
      </Layout>
      // <div className="App">
      //   <header className="App-header">
      //     <img src={logo} className="App-logo" alt="logo" />
      //     <h1 className="App-title">Welcome to React</h1>
      //   </header>
      //   <p className="App-intro">
      //     To get started, edit <code>src/App.tsx</code> and save to reload.
      //   </p>
      // </div>
    );
  }
  private handleShowChange(showToActivate: any) {
    //will need to check to see if currentshow is different and there are unsaved changes
    this.setState({ currentShow: showToActivate });
  }
}

// export default App;
