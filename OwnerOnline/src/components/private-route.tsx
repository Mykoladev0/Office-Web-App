import React from "react";
import { Route, Redirect } from "react-router-dom";
import * as authenticationService from "../services/authenticationService";

export const PrivateRoute = ({ component: Component, ...rest }: any) => (
  <Route
    {...rest}
    render={props =>
      authenticationService.userAuthenticated() ? (
        <Component {...props} />
      ) : (
        <Redirect
          to={{ pathname: "/login", state: { from: props.location } }}
        />
      )
    }
  />
);
