import React from "react";
import {
  BrowserRouter as Router,
  Route,
  Switch,
  Redirect
} from "react-router-dom";
import LoginPage from "./components/login/login";
import { PrivateRoute } from "./components/private-route";
import mainPage from "./components/main/main";
import ForgotPasswordPage from "./components/forgot-password/forgot-password";
import SignupPage from "./components/signup/signup";
import OtpPage from "./components/otp/otp";
import ShowDetail from "./components/main/shows/showDetail/showDetail";
//TODO: IMPLEMENTING AUTH
const Routing: React.FC = () => {
  return (
    <Router>
      <Switch>
        <PrivateRoute exact path="/" component={mainPage} />
        <PrivateRoute exact path="/show-detail/:id?" component={ShowDetail} />
        <Route path="/login/:id_token?" component={LoginPage} />
        <Route path="/forgot-password" component={ForgotPasswordPage} />
        <Route path="/otp" component={OtpPage} />
        <Route path="/signup/:id_token?" component={SignupPage} />
        <Redirect to="/" />
      </Switch>
    </Router>
  );
};

export default Routing;
