import LogRocket from 'logrocket';
/**
 * Helper function that watches the authenticate state, then applies it
 * as a boolean (authenticated) as well as attaches the userInfo data.
 */
async function checkAuthentication() {
  const authenticated = await this.props.auth.isAuthenticated();
  if (authenticated !== this.state.authenticated) {
    if (authenticated && !this.state.userInfo) {
      const userInfo = await this.props.auth.getUser();
      LogRocket.identify(userInfo.email, {
        name: userInfo.name,
        email: userInfo.email,

        // Add your own custom user variables here, ie:
        subscriptionType: 'pro',
      });
      this.setState({ authenticated, userInfo });
    } else {
      this.setState({ authenticated });
    }
  }
}

/* eslint-disable import/prefer-default-export */
export { checkAuthentication };
