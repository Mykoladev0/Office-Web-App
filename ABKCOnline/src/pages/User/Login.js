import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import { Checkbox, Alert, Icon } from 'antd';
import Login from '@/components/Login';
import styles from './Login.less';

const { Tab, UserName, Password, Mobile, Captcha, Submit } = Login;

@connect(({ login, loading }) => ({
  login,
  submitting: loading.effects['login/login'],
}))
class LoginPage extends Component {
  state = {
    type: 'account',
    autoLogin: true,
  };

  onTabChange = type => {
    this.setState({ type });
  };

  onGetCaptcha = () =>
    new Promise((resolve, reject) => {
      this.loginForm.validateFields(['mobile'], {}, (err, values) => {
        if (err) {
          reject(err);
        } else {
          const { dispatch } = this.props;
          dispatch({
            type: 'login/getCaptcha',
            payload: values.mobile,
          })
            .then(resolve)
            .catch(reject);
        }
      });
    });

  handleSubmit = (err, values) => {
    // const { type } = this.state;
    if (!err) {
      const { dispatch } = this.props;
      dispatch({
        type: 'login/login',
        payload: {
          ...values,
        },
      });
    }
  };

  changeAutoLogin = e => {
    this.setState({
      autoLogin: e.target.checked,
    });
  };

  renderMessage = content => (
    <Alert style={{ marginBottom: 24 }} message={content} type="error" showIcon />
  );

  render() {
    const { login, submitting } = this.props;
    const { type, autoLogin } = this.state;
    return (
      <div className={styles.main}>

        <div className="loginLeft">
          <img src={require("../../assets//img/abkc_logo.png")} alt="logo" />
        </div>
        <div className="loginRight">
          <Login
            defaultActiveKey={type}
            onTabChange={this.onTabChange}
            onSubmit={this.handleSubmit}
            ref={form => {
              this.loginForm = form;
            }}
          >
            {/* <div key="account" tab={formatMessage({ id: 'app.login.tab-login-credentials' })}> */}
            <div className={styles.loginFormControl}>
              <div className={styles.textCenter}>
                <h1 className={styles.titleLogin}><FormattedMessage id="app.login.tab-login-credentials" /></h1>
              </div>
              {login.status === 'error' &&
                login.type === 'account' &&
                !submitting &&
                this.renderMessage(formatMessage({ id: 'app.login.message-invalid-credentials' }))}
              <UserName
                name="username"
                placeholder={`${formatMessage({ id: 'app.login.userName' })}`}
                rules={[
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.userName.required' }),
                  },
                ]}
              />
              <Password
                name="password"
                placeholder={`${formatMessage({ id: 'app.login.password' })}`}
                rules={[
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.password.required' }),
                  },
                ]}
                onPressEnter={() => this.loginForm.validateFields(this.handleSubmit)}
              />
            </div>
            {/* <Tab key="mobile" tab={formatMessage({ id: 'app.login.tab-login-mobile' })}>
            {login.status === 'error' &&
              login.type === 'mobile' &&
              !submitting &&
              this.renderMessage(
                formatMessage({ id: 'app.login.message-invalid-verification-code' })
              )}
            <Mobile
              name="mobile"
              placeholder={formatMessage({ id: 'form.phone-number.placeholder' })}
              rules={[
                {
                  required: true,
                  message: formatMessage({ id: 'validation.phone-number.required' }),
                },
                {
                  pattern: /^1\d{10}$/,
                  message: formatMessage({ id: 'validation.phone-number.wrong-format' }),
                },
              ]}
            />
            <Captcha
              name="captcha"
              placeholder={formatMessage({ id: 'form.verification-code.placeholder' })}
              countDown={120}
              onGetCaptcha={this.onGetCaptcha}
              getCaptchaButtonText={formatMessage({ id: 'form.get-captcha' })}
              getCaptchaSecondText={formatMessage({ id: 'form.captcha.second' })}
              rules={[
                {
                  required: true,
                  message: formatMessage({ id: 'validation.verification-code.required' }),
                },
              ]}
            />
          </Tab> */}
            {/* <div>
            <Checkbox checked={autoLogin} onChange={this.changeAutoLogin}>
              <FormattedMessage id="app.login.remember-me" />
            </Checkbox>
            <a style={{ float: 'right' }} href="">
              <FormattedMessage id="app.login.forgot-password" />
            </a>
          </div> */}
            <Submit loading={submitting}>
              <FormattedMessage id="app.login.login" />
            </Submit>

            <div className="optionsLogin">
              <div>
                <a href="">
                  <FormattedMessage id="app.login.forgot-password" />
                </a>
              </div>
              <div>
                {/* <FormattedMessage id="app.login.sign-in-with" />
                <Icon type="alipay-circle" className={styles.icon} theme="outlined" />
                <Icon type="taobao-circle" className={styles.icon} theme="outlined" />
                <Icon type="weibo-circle" className={styles.icon} theme="outlined" /> */}
                <Link className={styles.register} to="/user/register">
                  <FormattedMessage id="app.login.signup" />
                </Link>
              </div>
            </div>
          </Login>
        </div>


      </div>
    );
  }
}

export default LoginPage;
