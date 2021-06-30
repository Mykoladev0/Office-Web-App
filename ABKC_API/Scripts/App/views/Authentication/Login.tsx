import React, { Component } from 'react';
import isEmail from 'validator/lib/isEmail';

//okta libraries
import OktaAuth from '@okta/okta-auth-js';
import { withAuth } from '@okta/okta-react';

// @material-ui/core components
import { WithStyles, withStyles } from '@material-ui/core/styles';
import InputAdornment from '@material-ui/core/InputAdornment';

// @material-ui/icons
import Face from '@material-ui/icons/Face';
import Email from '@material-ui/icons/Email';
import LockOutline from '@material-ui/icons/LockOutline';

import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';
import { Button, CustomInput } from '../../../../Vendor/mr-pro/components';
import { Card, CardBody, CardHeader, CardFooter } from '../../../../Vendor/mr-pro/components/Card';

import loginPageStyle from '../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/views/loginPageStyle.jsx';
import { BaseProps } from '../../Models';

import config from '../../../App/app.config';
import { Danger } from '../../../../Vendor/mr-pro/components/Typography';

export interface LoginCompProps extends WithStyles<typeof loginPageStyle>, BaseProps {}

export interface LoginCompState {
  cardAnimation: string;
  sessionToken: any;
  loginErrorMessage: string;
  username: string;
  password: string;
  touched: {
    email: boolean;
    password: boolean;
  };
}
const clientId = config.client_id,
  responseType = 'id_token', //id_token
  responseMode = 'fragment',
  scopes = 'openid',
  redirectUri = config.redirect_uri,
  state = 'WM6D', //Protects against cross-site request forgery (CSRF).
  nonce = 'YsG76jo'; //A string included in the returned ID Token. Use it to associate a client session with an ID Token, and to mitigate replay attacks.
const facebookUrl: string = `https://dev-436111.oktapreview.com/oauth2/v1/authorize?idp=0oafqeirob6xTBzfr0h7&client_id=${clientId}&response_type=${responseType}&response_mode=${responseMode}&scope=${scopes}&redirect_uri=${redirectUri}&state=${state}&nonce=${nonce}`;

class LoginComp extends React.Component<LoginCompProps, LoginCompState> {
  public state: LoginCompState = {
    cardAnimation: 'cardHidden',
    sessionToken: null,
    loginErrorMessage: null,
    username: '',
    password: '',
    touched: {
      email: false,
      password: false,
    },
  };
  private oktaAuth: OktaAuth = null;
  public constructor(props: LoginCompProps) {
    super(props);
    const configSettings = {
      url: props.baseUrl,
      clientId: config.client_id,
      redirectUri: config.redirect_uri,
      issuer: config.issuer,
    };
    this.oktaAuth = new OktaAuth(configSettings);

    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleUserNameChange = this.handleUserNameChange.bind(this);
    this.handlePasswordChange = this.handlePasswordChange.bind(this);
    this.attemptLogin = this.attemptLogin.bind(this);
    this.handleBlur = this.handleBlur.bind(this);
  }
  public componentDidMount() {
    // we add a hidden class to the card and after 700 ms we delete it and the transition appears
    setTimeout(
      function() {
        this.setState({ cardAnimation: '' });
      }.bind(this),
      700
    );
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const { cardAnimation, username, password, loginErrorMessage } = this.state;
    if (this.state.sessionToken) {
      this.props.auth.redirect({ sessionToken: this.state.sessionToken });
      return null;
    }
    // const canAttemptLogin: boolean = password && password.length > 4 && isEmail(username);
    const errors = {
      email: username.length === 0 || !isEmail(username),
      password: password.length < 4,
    };

    const shouldMarkError = field => {
      const hasError = errors[field];
      const shouldShow = this.state.touched[field];
      return hasError ? shouldShow : false;
    };
    const canAttemptLogin = !Object.keys(errors).some(x => errors[x]);
    // const errorMessage = this.state.error ? (
    //   <span className="error-message">{this.state.error}</span>
    // ) : null;
    return (
      <div className={classes.content}>
        <div className={classes.container}>
          <GridContainer justify="center">
            <GridItem xs={12} sm={6} md={4}>
              <form>
                <Card login className={classes[cardAnimation]}>
                  <CardHeader
                    className={`${classes.cardHeader} ${classes.textCenter}`}
                    color="rose"
                  >
                    <h4 className={classes.cardTitle}>Login</h4>
                    <div className={classes.socialLine}>
                      <a href={facebookUrl}>
                        <Button color="transparent" justIcon className={classes.customButtonClass}>
                          <i className={'fab fa-facebook-square'} />
                        </Button>
                      </a>
                      <Button color="transparent" justIcon className={classes.customButtonClass}>
                        <i className={'fab fa-google-plus'} />
                      </Button>
                      {/* {['fab fa-facebook-square', 'fab fa-google-plus'].map(
                        (prop, key) => {
                          return (
                            <Button
                              color="transparent"
                              justIcon
                              key={key}
                              className={classes.customButtonClass}
                            >
                              <i className={prop} />
                            </Button>
                          );
                        }
                      )} */}
                    </div>
                  </CardHeader>
                  <CardBody>
                    {/* <CustomInput
                      labelText="UserName..."
                      id="userName"
                      formControlProps={{ fullWidth: true }}
                      inputProps={{
                        onChange: evt => this.handleUserNameChange(evt.target.value),
                        endAdornment: (
                          <InputAdornment position="end">
                            <Face className={classes.inputAdornmentIcon} />
                          </InputAdornment>
                        ),
                      }}
                    /> */}
                    <CustomInput
                      labelText="Email..."
                      id="email"
                      error={shouldMarkError('email')}
                      formControlProps={{
                        fullWidth: true,
                      }}
                      inputProps={{
                        onChange: evt => this.handleUserNameChange(evt.target.value),
                        onBlur: this.handleBlur('email'),
                        endAdornment: (
                          <InputAdornment position="end">
                            <Email className={classes.inputAdornmentIcon} />
                          </InputAdornment>
                        ),
                      }}
                    />
                    <CustomInput
                      labelText="Password"
                      id="password"
                      error={shouldMarkError('password')}
                      formControlProps={{
                        fullWidth: true,
                      }}
                      inputProps={{
                        onChange: evt => this.handlePasswordChange(evt.target.value),
                        onBlur: this.handleBlur('password'),
                        endAdornment: (
                          <InputAdornment position="end">
                            <LockOutline className={classes.inputAdornmentIcon} />
                          </InputAdornment>
                        ),
                        onKeyPress: e => {
                          if (e.key === 'Enter' && e.target.value.length >= 3) {
                            this.attemptLogin();
                          }
                        },
                        type: 'password',
                      }}
                    />
                    {loginErrorMessage && (
                      <h4 className={`${classes.textCenter}`}>
                        <Danger>{loginErrorMessage}</Danger>
                      </h4>
                    )}
                  </CardBody>
                  <CardFooter className={classes.justifyContentCenter}>
                    <Button
                      disabled={!canAttemptLogin}
                      color="rose"
                      simple
                      size="lg"
                      block
                      onClick={this.handleSubmit}
                    >
                      Let's Go
                    </Button>
                  </CardFooter>
                </Card>
              </form>
            </GridItem>
          </GridContainer>
        </div>
      </div>
    );
  }

  private handleBlur = field => evt => {
    this.setState({
      loginErrorMessage: null,
      touched: { ...this.state.touched, [field]: true },
    });
  };

  private attemptLogin() {
    this.oktaAuth
      .signIn({
        username: this.state.username,
        password: this.state.password,
      })
      .then(res => {
        this.oktaAuth.token
          .getWithoutPrompt({
            sessionToken: res.sessionToken,
            scopes: ['openid', 'email', 'profile'],
            // state: '8rFzn3MH5q',
            // nonce: '51GePTswrm',
          })
          .then(tokenOrTokens => {
            // manage token or tokens
            console.log(tokenOrTokens);
            this.setState({
              sessionToken: res.sessionToken,
            });
          })
          .catch(err => {
            // handle OAuthError
            console.error(err);
          });
      })
      .catch(err => {
        this.setState({ loginErrorMessage: err.message });
        // tslint:disable-next-line:no-console
        console.log(err.statusCode + ' error', err);
      });
  }
  private handleSubmit(evt) {
    evt.preventDefault();
    this.attemptLogin();
  }
  private handleUserNameChange(username: string) {
    this.setState({ username });
  }
  private handlePasswordChange(password: string) {
    this.setState({ password });
  }
}
export default withAuth(withStyles(loginPageStyle)(LoginComp));
