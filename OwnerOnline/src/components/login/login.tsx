import React, { useEffect } from "react";
import { Checkbox, Input, Button, Form, Anchor } from "antd";
import { GoogleOutlined, FacebookFilled } from "@ant-design/icons";
import "./login.css";
import { useTranslation } from "react-i18next";
import * as authenticationService from "../../services/authenticationService";
import { useHistory } from "react-router-dom";
import { TEST_ACTION } from "../../store/test/action-types";
import { useDispatch } from "react-redux";

const LoginPage: React.FC = (props: any) => {
  const history = useHistory();
  const { Link } = Anchor;
  const dispatch = useDispatch();
  const { t } = useTranslation(undefined, { useSuspense: false });
  useEffect(() => {
    if (props.location.hash) {
      authenticationService.setToken(props.location.hash.split("=")[1]);
      dispatch({ type: TEST_ACTION, payload: {} });
      history.push("/");
    }
  }, []);// eslint-disable-line react-hooks/exhaustive-deps
  const handleLogin = (values: any) => {
    authenticationService.login(values.username, values.password).then(res => {
      history.push("/");
    });
  };

  return (
    <div className="form-container login">
      <div className="main">
        <div className="logo">
          <img src={require("../../assets/img/abkc_logo.png")} alt="logo" />
        </div>
        <div className="form-area">
          <Form
            name="basic"
            initialValues={{
              remember: true
            }}
            onFinish={handleLogin}
          >
            <div className="loginFormControl">
              <div className="title-area">
                <h2>{t("loginPageHeader")}</h2>
                <span>with your ABKC account</span>
              </div>
              <Form.Item
                name="username"
                rules={[
                  {
                    required: true,
                    message: "Please input your username!"
                  }
                ]}
              >
                <Input placeholder="Email or Username" />
              </Form.Item>

              <Form.Item
                name="password"
                rules={[
                  {
                    required: true,
                    message: "Please input your password!"
                  }
                ]}
              >
                <Input.Password placeholder="Password" />
              </Form.Item>
            </div>
            <div className="optionsLogin">
              <div>
                <Form.Item name="remember" valuePropName="checked">
                  <Checkbox>
                    <span className="remember">Remember me</span>
                  </Checkbox>
                </Form.Item>
              </div>
              <div>
                <Anchor>
                  <Link href="forgot-password" title="Forgot password?" />
                </Anchor>
              </div>
            </div>
            <Form.Item>
              <Button type="primary" className="btn-submit" htmlType="submit">
                <span>Login</span>
              </Button>
            </Form.Item>
          </Form>
          <div className="social-login">
            <span className="social-login-label">or login with:</span>
            <a href="https://dev-436111.oktapreview.com/oauth2/v1/authorize?idp=0oaqsd8mlnPISnVJB0h7&client_id=0oafq76jfqRMZNgiR0h7&response_type=id_token&response_mode=fragment&scope=openid&redirect_uri=http://localhost:3000/login&state=WM6D&nonce=Yth76jo">
              <GoogleOutlined className="google-icon" />
            </a>
            <a href="https://dev-436111.oktapreview.com/oauth2/v1/authorize?idp=0oafqeirob6xTBzfr0h7&client_id=0oafq76jfqRMZNgiR0h7&response_type=id_token&response_mode=fragment&scope=openid&redirect_uri=http://localhost:3000/login&state=WM6D&nonce=Yth76jo">
              <FacebookFilled className="facebook-icon" />
            </a>
          </div>

          <hr></hr>
          <div className="form-footer">
            <span>Don't have an account?</span>
            <Anchor>
              <Link href="signup" title="Sign up" />
            </Anchor>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
