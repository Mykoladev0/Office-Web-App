import React from "react";
// import { useDispatch } from 'react-redux';
import { Input, Button, Form, Anchor } from "antd";
import "./forgot-password.css";
import { useTranslation } from "react-i18next";

const ForgotPasswordPage: React.FC = () => {
  // Initalize dispatcher
  // const dispatch = useDispatch();

  // useState hook for error message
  const { Link } = Anchor;
  const { t } = useTranslation(undefined, { useSuspense: false });

  const handleSubmit = (values: any) => {
    console.log("Success:", values);
  };

  return (
    <div className="form-container forgot">
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
                <h2>{t("forgotPasswordHeader")}</h2>
                <span>
                  Please fill up the below information<br></br>to recover your
                  password
                </span>
              </div>
              <Form.Item
                name="username"
                rules={[
                  {
                    required: true,
                    message: "Please input your username or email!"
                  }
                ]}
              >
                <Input placeholder="Email or Username" />
              </Form.Item>
            </div>
            <Form.Item>
              <Button type="primary" className="btn-submit" htmlType="submit">
                <span>Recover Password</span>
              </Button>
            </Form.Item>
          </Form>
          <div style={{ textAlign: "center" }}>
            <Anchor>
              <Link
                href="#components-anchor-demo-basic"
                title="Back to login"
              />
            </Anchor>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;
