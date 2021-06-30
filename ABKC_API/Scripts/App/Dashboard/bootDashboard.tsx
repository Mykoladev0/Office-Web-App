import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter, Router, Route, Switch } from 'react-router-dom';
// import { createBrowserHistory } from 'history';
import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';

import LoginPage from '../views/Authentication/Login';

import { indexRoutes } from '../toplevelroutes';
import config from '../app.config';
import { Security, SecureRoute, ImplicitCallback } from '@okta/okta-react';

//const hist = createBrowserHistory();

const onAuthRequired = ({ history }) => history.push('/login'); //redirect to login if a route needs a login

export function renderDashboard() {
  // This code starts up the React app when it runs in a browser. It sets up the routing
  // configuration and injects the app into a DOM element.
  //const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;

  LogRocket.init('bju6uk/abkc_dev');
  setupLogRocketReact(LogRocket);

  ReactDOM.render(
    // <AppContainer>//used for react-hot-loader
    // <BrowserRouter children={ routes } basename={ baseUrl } />,
    // </AppContainer>,
    //was Router on the following line<Router history={hist}>
    <BrowserRouter>
      <Security
        issuer={config.issuer}
        client_id={config.client_id}
        redirect_uri={config.redirect_uri}
        onAuthRequired={onAuthRequired}
      >
        <Switch>
          <Route path="/login" render={() => <LoginPage baseUrl={config.url} />} />
          <Route path="/implicit/callback" component={ImplicitCallback} />
          {/*for redirect back from okta*/}
          {indexRoutes.map((prop, key) => {
            //will need to change this to listen for secure route prop in top level routes
            //and return a SecureRoute if necessary
            return <SecureRoute path={prop.path} component={prop.component} key={key} />;
          })}
        </Switch>
      </Security>
    </BrowserRouter>,
    document.getElementById('react-dashboard')
  );
}
