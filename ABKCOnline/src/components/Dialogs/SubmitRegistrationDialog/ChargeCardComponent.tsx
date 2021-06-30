import React, { Component } from 'react';
import { CardElement, injectStripe } from 'react-stripe-elements';
import { Button, Col, notification, Row, Spin } from 'antd';

export interface IChargeCardComponentProps {
  stripe: any;
  handleSuccessPayment: (token) => void;
}

export interface IChargeCardComponentState {
  processing: boolean;
  ccComplete: boolean;
}

class ChargeCardComponent extends Component<IChargeCardComponentProps, IChargeCardComponentState> {
  public state: IChargeCardComponentState = { processing: false, ccComplete: false };

  public constructor(props: IChargeCardComponentProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { processing, ccComplete } = this.state;
    return (
      <div>
        <h2>Card details</h2>
        <br />
        <Row>
          {processing && (
            <div style={{ textAlign: 'center' }}>
              Verifying Payment Information
              <Spin size="large" />
            </div>
          )}
        </Row>
        <Row>
          <Col offset={4} span={8}>
            <CardElement
              // onBlur={handleBlur}
              onChange={evt => this.setState({ ccComplete: evt.complete })}
              // onFocus={handleFocus}
              // onReady={handleReady}
              // {...createOptions(this.props.fontSize)}
            />
          </Col>
        </Row>

        <Button disabled={!ccComplete} onClick={this.submit}>
          Authorize Payment
        </Button>
      </div>
    );
  }

  private submit = async ev => {
    const { handleSuccessPayment } = this.props;
    this.setState({ processing: true });
    const { token } = await this.props.stripe.createToken({ name: 'User Name' });
    // const response = await fetch('/charge', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'text/plain' },
    //   body: token.id,
    // });
    this.setState({ processing: false });
    if (token !== null) {
      notification.success({
        message: 'Payment Authorized',
        duration: 10,
        description:
          'The payment option supplied has been approved, click next to finalize the registrations.',
        onClose: () => {
          // move to the next step
          handleSuccessPayment(token);
        },
      });
    }
  };
}

const StripeComponent = injectStripe(ChargeCardComponent);
export default StripeComponent;
