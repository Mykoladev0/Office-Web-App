import React, { Component } from 'react';
import { Spin } from 'antd';
import { finalizeTransaction } from '@/services/submitRegistrationApi';

export interface IVerifyPaymentComponentProps {
  token: any;
  paymentQuote: any;
  handlePaymentSuccessFn: (paymentResult) => void;
}

export default class VerifyPaymentComponent extends Component<IVerifyPaymentComponentProps, any> {
  public state: any = {
    showProcessing: true,
    message: '',
  };
  public async componentDidMount() {
    const { paymentQuote, token, handlePaymentSuccessFn } = this.props;

    const result = await finalizeTransaction(
      paymentQuote.registrations,
      token.id,
      paymentQuote.subTotal + paymentQuote.transactionFee
    );
    if (result !== null) {
      handlePaymentSuccessFn(result);
      // this.setState({ showProcessing: false, message: 'Payment Successful' }, () =>
      //   handlePaymentSuccessFn(result)
      // );
    }
  }
  public render() {
    const { showProcessing } = this.state;
    return (
      showProcessing && (
        <div style={{ textAlign: 'center' }}>
          Completing Payment
          <Spin size="large" />
        </div>
      )
    );
  }
}
