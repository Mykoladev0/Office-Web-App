import React, { useEffect } from "react";
// import { useDispatch } from 'react-redux';
import { Input, Button, Form, Anchor } from "antd";
import { GoogleOutlined, FacebookFilled } from "@ant-design/icons";
import "./signup.css";
import { useTranslation } from "react-i18next";
import * as authenticationService from "../../services/authenticationService";
import { useHistory } from "react-router-dom";

const SignupPage: React.FC = (props: any) => {
  const history = useHistory();
  const { Link } = Anchor;
  const { t } = useTranslation(undefined, { useSuspense: false });
  useEffect(() => {
    if (props.location.hash) {
      authenticationService.setToken(props.location.hash.split("=")[1]);
      history.push("/");
    }
  }, []);// eslint-disable-line react-hooks/exhaustive-deps
  const handleSubmit = (values: any) => {
    authenticationService
      .signUp(values.firstname, values.lastname, values.email)
      .then(res => {
        history.push("/");
      });
  };

  return (
    <div className="form-container sign-up">
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
            onFinish={handleSubmit}
          >
            <div className="loginFormControl">
              <div className="title-area">
                <h2>{t("signupPageHeader")}</h2>
                <span>Create an account</span>
              </div>
              <Form.Item
                name="firstname"
                rules={[
                  {
                    required: true,
                    message: "Please input your First Name!"
                  }
                ]}
              >
                <Input placeholder="First Name" />
              </Form.Item>
              <Form.Item
                name="lastname"
                rules={[
                  {
                    required: true,
                    message: "Please input your Last Name!"
                  }
                ]}
              >
                <Input placeholder="Last Name" />
              </Form.Item>
              <Form.Item
                name="email"
                rules={[
                  {
                    required: true,
                    message: "Please input your Email Address!"
                  }
                ]}
              >
                <Input placeholder="Email Address" />
              </Form.Item>
            </div>

            <div className="description">
              <span>
                by clicking on below "Sign Up" button means you agree to our{" "}
                <a href="#components-anchor-demo-basic">Terms & Conditions</a>{" "}
                and <a href="#components-anchor-demo-basic"> Privacy Policy</a>
              </span>
            </div>

            <Form.Item>
              <Button type="primary" className="btn-submit" htmlType="submit">
                <span>Sign Up</span>
              </Button>
            </Form.Item>
          </Form>
          <div className="social-login">
            <span className="social-login-label">Or Sign Up with:</span>
            <a href="https://dev-436111.oktapreview.com/oauth2/v1/authorize?idp=0oaqsd8mlnPISnVJB0h7&client_id=0oafq76jfqRMZNgiR0h7&response_type=id_token&response_mode=fragment&scope=openid&redirect_uri=http://localhost:3000/login&state=WM6D&nonce=Ytk76jo">
              <GoogleOutlined className="google-icon" />
            </a>
            <a href="https://dev-436111.oktapreview.com/oauth2/v1/authorize?idp=0oafqeirob6xTBzfr0h7&client_id=0oafq76jfqRMZNgiR0h7&response_type=id_token&response_mode=fragment&scope=openid&redirect_uri=http://localhost:3000/signup&state=WM6D&nonce=Ytk76jo">
              <FacebookFilled className="facebook-icon" />
            </a>
          </div>

          <hr></hr>
          <div className="form-footer">
            <span>Already have an account?</span>
            <Anchor>
              <Link href="#components-anchor-demo-basic" title="Log In" />
            </Anchor>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SignupPage;
