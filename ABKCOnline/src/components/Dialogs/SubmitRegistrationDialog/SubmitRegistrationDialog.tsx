import { Avatar, Button, Icon, List, message, Modal, Spin, Steps } from 'antd';
import { Elements, StripeProvider } from 'react-stripe-elements';
import StripeComponent from './ChargeCardComponent';
import VerifyPaymentComponent from './VerifyPaymentComponent';
import {
  compose,
  countBy,
  curry,
  descend,
  evolve,
  add,
  forEachObjIndexed,
  groupBy,
  head,
  lift,
  map,
  reduce,
  mergeRight,
  prop,
  sort,
  tail,
  values,
} from 'ramda';
import React, { Component } from 'react';

import styles from './SubmitRegistrationDialog.less';
import { checkRegistrationsForValidity, getPaymentQuote } from '@/services/submitRegistrationApi';

const Step = Steps.Step;

const sumBy = property =>
  lift(reduce((current, val) => evolve({ [property]: add(val[property]) }, current)))(head, tail);

export interface ISubmitRegistrationDialogProps {
  draftRegistrations: any[];
  handleCloseFn: (isComplete: boolean) => void;
}

export interface ISubmitRegistrationDialogState {
  currentStep: number;
  reviewedDraftRegistrations: any[];
  paymentQuote: any;
  showProcessing: boolean;
  showQuoteProcessing: boolean;
  paymentToken: any;
  paymentCompleted: boolean;
  paymentResult: any;
  submissionsToSubmitCount: number;
}

export default class SubmitRegistrationDialog extends Component<
  ISubmitRegistrationDialogProps,
  ISubmitRegistrationDialogState
> {
  public state: ISubmitRegistrationDialogState = {
    currentStep: 0,
    reviewedDraftRegistrations: [],
    paymentQuote: null,
    showProcessing: false,
    showQuoteProcessing: false,
    paymentToken: null,
    paymentCompleted: false,
    paymentResult: null,
    submissionsToSubmitCount: 0,
  };

  private steps = [
    {
      title: 'Review Registrations',
      icon: <Icon type="check-circle" />,
    },
    {
      title: 'Payment Summary',
      content: 'Second-content',
      icon: <Icon type="solution" />,
    },
    {
      title: 'Payment Information',
      content: 'Second-content',
      icon: <Icon type="credit-card" />,
    },
    {
      title: 'Verifying Payment',
      content: 'Second-content',
      icon: null,
    },
    {
      title: 'Receipt',
      content: 'Last-content',
      icon: <Icon type="smile-o" />,
    },
  ];

  public constructor(props: ISubmitRegistrationDialogProps) {
    super(props);
  }
  public async componentDidMount() {
    this.setState({ showProcessing: true });
    const { draftRegistrations } = this.props;
    const results = await checkRegistrationsForValidity(draftRegistrations);
    const reviewedDraftRegistrations = results.map(r => {
      const found = draftRegistrations.find(
        f => f.id === r.registrationId && f.registrationType === r.registrationType
      );
      return mergeRight(r, found);
    });
    // const reviewedDraftRegistrations = this.sortRegistrationsByValid(reviewed);
    const submissionsToSubmitCount = reviewedDraftRegistrations.filter(r => r.isValid === true)
      .length;
    if (submissionsToSubmitCount > 0) {
      this.getPaymentQuote(reviewedDraftRegistrations);
    }
    this.setState({ reviewedDraftRegistrations, showProcessing: false, submissionsToSubmitCount });
  }

  public render(): JSX.Element {
    const { handleCloseFn } = this.props;
    const { currentStep } = this.state;

    return (
      <Modal
        width={1000}
        visible={true}
        title={'Submit Registrations'}
        closable={true}
        center={true}
        onCancel={() => handleCloseFn(false)}
        destroyOnClose={true}
        footer={this.footer(currentStep)}
      >
        <Steps current={currentStep}>
          {this.steps.map(item => (
            <Step
              key={item.title}
              title={item.title}
              icon={currentStep === 3 && item.icon === null ? <Icon type="loading" /> : item.icon}
            />
          ))}
        </Steps>
        <div className={styles.stepsContent}>{this.getContentView(currentStep)}</div>
      </Modal>
    );
  }

  private getContentView = currentStep => {
    const { paymentQuote, paymentToken } = this.state;
    switch (currentStep) {
      case 0:
        return this.registrationReviewContent();
      case 1:
        return this.paymentSummaryContent();
      case 2:
        return (
          <StripeProvider apiKey="pk_test_qfSKg5QcBLmgb84H7zVaXoKV">
            <Elements>
              <StripeComponent
                handleSuccessPayment={token => this.setState({ paymentToken: token })}
              />
            </Elements>
          </StripeProvider>
        );
      case 3:
        return (
          <VerifyPaymentComponent
            token={paymentToken}
            paymentQuote={paymentQuote}
            handlePaymentSuccessFn={paymentResult => {
              this.setState({ paymentResult });
              this.next();
            }}
          />
        );
      case 4:
        return <div>Receipt Summary and description about what happens next will go here</div>;
      default:
        return null;
    }
  };
  private footer = currentStep => {
    const { handleCloseFn } = this.props;
    const { paymentToken, paymentCompleted, submissionsToSubmitCount } = this.state;
    return (
      <div className="steps-action">
        {submissionsToSubmitCount === 0 && (
          <Button onClick={() => handleCloseFn(false)}>Close Submission Wizard</Button>
        )}

        {currentStep > 0 && currentStep !== 4 && (
          <Button style={{ marginLeft: 8 }} onClick={() => this.prev()}>
            Previous
          </Button>
        )}

        {submissionsToSubmitCount > 0 && currentStep < this.steps.length - 1 && (
          <Button
            type="primary"
            disabled={
              (currentStep === 2 && paymentToken === null) ||
              (currentStep === 3 && !paymentCompleted)
            }
            onClick={() => this.next()}
          >
            Next
          </Button>
        )}
        {submissionsToSubmitCount > 0 && currentStep === this.steps.length - 1 && (
          <Button type="primary" onClick={() => handleCloseFn(true)}>
            Close Payment Dialog
          </Button>
        )}
      </div>
    );
  };

  private registrationReviewContent = () => {
    const { reviewedDraftRegistrations, showProcessing } = this.state;
    return (
      <div>
        <h3 style={{ margin: '16px 0' }}>Draft Registrations</h3>
        {showProcessing ? (
          <div style={{ textAlign: 'center' }}>
            <Spin size="large" />
          </div>
        ) : (
          <List
            className={styles.listHeight}
            size="small"
            //   header={<div>Header</div>}
            //   footer={<div>Footer</div>}
            bordered={false}
            dataSource={reviewedDraftRegistrations}
            renderItem={item => (
              <List.Item actions={[]}>
                {item.isValid ? (
                  <List.Item.Meta
                    avatar={
                      <Avatar shape="circle" size="default">
                        <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                      </Avatar>
                    }
                    title={`${item.registrationType}: ${item.name}`}
                    description={'Registration is valid for submission'}
                  />
                ) : (
                  <List.Item.Meta
                    avatar={
                      <Avatar shape="circle" size="default">
                        <Icon type="exclamation-circle" theme="twoTone" twoToneColor="#cc0808" />
                      </Avatar>
                    }
                    title={`${item.registrationType}: ${item.name}`}
                    description={`Registration cannot be submitted because ${item.message}`}
                  />
                )}
              </List.Item>
            )}
          />
        )}
      </div>
    );
  };

  private paymentSummaryContent = () => {
    const { paymentQuote, showQuoteProcessing } = this.state;
    const byRegistration = groupBy(prop('registrationType'))(paymentQuote.registrations);
    const summary = [];
    {
      forEachObjIndexed((value, key) => {
        let found = summary.find(s => s.registrationType === key);
        if (!found) {
          found = {
            count: value.length,
            amount: value.reduce((a, b) => (b.amount == null ? a : a + b.amount), 0),
            registrationType: key,
          };
          summary.push(found);
        }
        // const sum: any = sumBy(prop('amount'))(value);
      }, byRegistration);
    }
    return (
      <div>
        <h3 style={{ margin: '16px 0' }}>Draft Registrations</h3>
        {showQuoteProcessing ? (
          <div style={{ textAlign: 'center' }}>
            <Spin size="large" />
          </div>
        ) : (
          <div>
            <ul>
              {summary.map(s => {
                return (
                  <li key={s.registrationType}>{`${s.count} ${
                    s.registrationType
                  } registrations totalling $${s.amount.toFixed(2)}`}</li>
                );
              })}
            </ul>
            <br />
            <ul>
              <li>{`SubTotal: $${paymentQuote.subTotal.toFixed(2)}`}</li>
              <li>{`Transaction Fee: $${paymentQuote.transactionFee.toFixed(2)}`}</li>
              <li>{`Total To Be Charged: $${(
                paymentQuote.transactionFee + paymentQuote.subTotal
              ).toFixed(2)}`}</li>
            </ul>
          </div>
        )}
      </div>
    );
  };

  private next = () => {
    const { currentStep } = this.state;
    this.setState({ currentStep: currentStep + 1 });
  };
  private prev = () => {
    const { currentStep } = this.state;
    this.setState({ currentStep: currentStep - 1 });
  };

  private getPaymentQuote = (reviewedDraftRegistrations) => {
    // const { reviewedDraftRegistrations } = this.state;
    this.setState({ showQuoteProcessing: true }, () => {
      getPaymentQuote(reviewedDraftRegistrations).then(paymentQuote =>
        this.setState({ paymentQuote, showQuoteProcessing: false })
      );
    });
  };

  // private sortRegistrationsByValid = regs =>
  //   curry(regs =>
  //     compose(
  //       values,
  //       map(
  //         compose(
  //           head,
  //           sort(descend(prop('id')))
  //         )
  //       ),
  //       groupBy(m => m.isValid)
  //     )(regs)
  //   );
}
