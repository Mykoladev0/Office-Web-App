import React, { useState, useEffect } from "react";
// import { useDispatch } from 'react-redux';
import { Button, Form, Anchor } from "antd";
import "./otp.css";
import { useTranslation } from "react-i18next";
import OtpInput from "react-optinput";
import "react-optinput/bundle.css";

const OtpPage: React.FC = () => {
  // Initalize dispatcher
  // const dispatch = useDispatch();
  const [counter, setCounter] = useState(120);
  useEffect(() => {
    counter > 0 && setTimeout(() => setCounter(counter - 1), 1000);
  }, [counter]);
  // useState hook for error message
  const { Link } = Anchor;
  const { t } = useTranslation(undefined, { useSuspense: false });
  const handleSubmit = (values: any) => {
    console.log("Success:", values);
  };

  return (
    <div className="form-container otp">
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
                <h2>{t("otpHeader")}</h2>
                <span>
                  We have sent you one time password<br></br>on your mobile
                </span>
              </div>
              <div className="count">
                {counter > 60
                  ? `1:${counter - 60 < 10 ? `0${counter - 60}` : counter - 60}`
                  : `0:${counter}`}
              </div>
              {/* <div>Countdown: {counter}</div> */}
              <OtpInput
                codeLength={4}
                onInputChange={(value: string) => {
                  console.log(value);
                }}
              />
            </div>
            <div className="form-footer">
              <span>Didnt recieve the OTP? </span>
              <Anchor>
                <Link href="#components-anchor-demo-basic" title="RESEND" />
              </Anchor>
            </div>

            <Form.Item>
              <Button type="primary" className="btn-submit" htmlType="submit">
                <span>Verify</span>
              </Button>
            </Form.Item>
          </Form>
        </div>
      </div>
    </div>
  );
};

export default OtpPage;
