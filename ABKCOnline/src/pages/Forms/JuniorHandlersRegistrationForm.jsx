/* eslint-disable jsx-a11y/label-has-for */
/* eslint-disable jsx-a11y/label-has-associated-control */
import React, { PureComponent } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import { formatMessage, FormattedMessage } from 'umi/locale';
import {
  Form,
  Input,
  DatePicker,
  Select,
  Button,
  Card,
  Row,
  Divider,
  Col,
  Anchor,
  Spin,
  Modal
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import styles from './style.less';

const FormItem = Form.Item;
const { Option } = Select;
const { Link } = Anchor;

@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  colorList: form.colors,
  startRegistration: form.startRegistration,
  statusStart: form.statusStart,
  startJuniorHandler: form.startJuniorHandler,
  hideStartButton: form.hideStartButton,
  spinner: form.spinner,
  editList: form.editList,
  pedigreePaymentQuote: form.pedigreePaymentQuote,
  submitSuccess: form.submitSuccess,
  submitting: loading.effects['form/submitRegularForm']
}))
@Form.create()
class JuniorHandlersRegistrationForm extends PureComponent {
  state = {
    updated: true,
    overnightRequested: false,
    rushRequested: false,
    isInternational: false,
    buttonType: '',
    currentRegId: '',
    isRegistrationStarted: false,
    isEditing: false,
    showSubmitModal: false,
  };

  componentDidMount() {
    const { dispatch, location } = this.props;
    if (location.state && location.state.id) {
      this.setState({ isEditing: true });
      dispatch({
        type: 'list/getJuniorHandlersById',
        payload: {
          id: location.state.id
        }
      });
    } else {
      dispatch({
        type: 'form/setStatus'
      });
    }
  }

  componentWillReceiveProps(nextProps) {
    const { editJuniorHandlersList } = nextProps.list;
    const { updated } = this.state;
    const { location } = nextProps;
    const { form } = this.props;
    const { currentUsers } = nextProps.list;
    const userRole = currentUsers && currentUsers.abkcRolesUserBelongsTo[1];
    if (nextProps.submitSuccess && (userRole === "Owners" || userRole === "Representatives")) {
      this.setState({ showSubmitModal: true });
    }
    if (!nextProps.spinner) {
      this.setState({
        isRegistrationStarted: false
      });
    }
    if (
      updated &&
      editJuniorHandlersList &&
      location.state &&
      location.state.id
    ) {
      form.setFieldsValue({
        firstName: editJuniorHandlersList.firstName,
        lastName: editJuniorHandlersList.lastName,
        address1: editJuniorHandlersList.address1,
        address2: editJuniorHandlersList.address2,
        address3: editJuniorHandlersList.address3,
        city: editJuniorHandlersList.city,
        state: editJuniorHandlersList.state,
        zip: editJuniorHandlersList.zip,
        country: editJuniorHandlersList.country,
        international: editJuniorHandlersList.international ? 'Yes' : 'No',
        email: editJuniorHandlersList.email,
        phone: editJuniorHandlersList.phone,
        cell: editJuniorHandlersList.cell,
        dateOfBirth: moment(editJuniorHandlersList.dateOfBirth),
        parentFirstName: editJuniorHandlersList.parentFirstName,
        parentLastName: editJuniorHandlersList.parentLastName,
        certificateNumber: editJuniorHandlersList.certificateNumber
          ? editJuniorHandlersList.certificateNumber
          : '',
        typeOfRequest:
          (editJuniorHandlersList.isInternational && 'isInternational') ||
          (editJuniorHandlersList.overnightRequested && 'overnightRequested') ||
          (editJuniorHandlersList.rushRequested && 'rushRequested') ||
          ''
      });
      this.setState({
        updated: false,
        isEditing: false
      });
    }
  }

  handleTypeSelect = value => {
    if (value === 'isInternational') {
      this.setState({
        isInternational: true,
        overnightRequested: false,
        rushRequested: false
      });
    } else if (value === 'overnightRequested') {
      this.setState({
        isInternational: false,
        overnightRequested: true,
        rushRequested: false
      });
    } else if (value === 'rushRequested') {
      this.setState({
        isInternational: false,
        overnightRequested: false,
        rushRequested: true
      });
    }
  };

  handleOk = () => {
    const { dispatch } = this.props;
    this.setState({
      showSubmitModal: false
    });
    dispatch({
      type: 'form/redirectPage'
    });
  }

  info = value => {
    const { overnightRequested, rushRequested, isInternational } = this.state;
    return {
      lastName: value.lastName,
      firstName: value.firstName,
      address1: value.address1,
      address2: value.address2,
      address3: value.address3,
      city: value.city,
      state: value.state,
      zip: value.zip,
      country: value.country,
      international: value.international === 'Yes' ? true : false,
      email: value.email,
      phone: value.phone,
      cell: value.cell,
      dateOfBirth: moment(value.dateOfBirth).format(),
      parentFirstName: value.parentFirstName,
      parentLastName: value.parentLastName,
      certificateNumber: value.certificateNumber,
      overnightRequested: overnightRequested,
      rushRequested: rushRequested,
      isInternational: isInternational
    };
  };

  handleSubmitDraft = e => {
    const { dispatch, form } = this.props;
    const { buttonType, currentRegId } = this.state;
    const role = localStorage.getItem('user-role');

    if (buttonType === 'submit') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value);
        if (!err) {
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitJuniorHandlersRegistrationForm',
              payload: {
                ...this.info(value),
                id: this.props.list.editJuniorHandlersList.id
              },
              content: {
                currentRegId,
                callSubmit: true,
                registrationType: 'juniorHandler'
              }
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'juniorHandler'
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: {
                ...this.info(value),
                id: this.props.list.editJuniorHandlersList.id
              },
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'juniorHandler',
                regArray,
                draftApi: 'editJuniorHandlersRegistrationForms'
              }
            });
          }
        }
      });
    } else if (buttonType === 'later') {
      form.validateFields(['firstName', 'lastName'], err => {
        const value = form.getFieldsValue();
        this.info(value);
        if (!err) {
          dispatch({
            type: 'form/draftJuniorHandlersRegistrationForm',
            payload: {
              ...this.info(value),
              id: this.props.list.editJuniorHandlersList.id
            }
          });
        }
      });
    } else if (buttonType === 'start') {
      form.validateFields(['firstName', 'lastName'], errors => {
        const value = form.getFieldsValue();
        this.info(value);
        if (!errors) {
          this.setState({ isRegistrationStarted: true });
          dispatch({
            type: 'form/startJuniorHandlersRegistrationForm',
            payload: this.info(value)
          });
        }
      });
    }
  };

  getButtonId = (e, id, regId) => {
    this.setState(
      {
        buttonType: id,
        currentRegId: regId
      },
      () => {
        this.handleSubmitDraft();
      }
    );
  };

  handleCancel = () => {
    this.setState({ showSubmitModal: false });
  };

  render() {
    const {
      submitting,
      statusStart,
      startRegistration,
      location,
      list,
      pedigreePaymentQuote
    } = this.props;
    const { showStatus } = list;
    const { updated, isRegistrationStarted, isEditing, showSubmitModal } = this.state;
    const {
      form: { getFieldDecorator }
    } = this.props;
    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 8 },
        md: { span: 8 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
        md: { span: 16 }
      }
    };

    const submitFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 10 }
      }
    };

    let regId = '';
    if (statusStart && startRegistration) {
      regId = startRegistration.id;
    }
    // if (status && registration) {
    //   regId = registration.id;
    // }
    else if (location.state) {
      regId = location.state.id;
    }

    return (
      // eslint-disable-next-line react/jsx-filename-extension
      <PageHeaderWrapper
        className={styles.formHeader}
        title={<FormattedMessage id="form.junior.handlers.header.text" />}
      >
        <Card bordered={false}>
          <Anchor affix={false}>
            <Link href="/registration-list" title="Back" />
          </Anchor>
          <Spin spinning={isEditing}>
            <Form hideRequiredMark style={{ marginTop: 8 }}>
              <div>
                <Row gutter={16}>
                  <h2>
                    <FormattedMessage id="form.junior.handlers.header.sub1.text" />
                  </h2>
                  <Divider />
                </Row>
                <Spin spinning={this.state.isRegistrationStarted}>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.firstName.label" />}
                      >
                        {getFieldDecorator('firstName', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.firstName.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.firstName.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.lastName.label" />}
                      >
                        {getFieldDecorator('lastName', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.lastName.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.lastName.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={
                          <FormattedMessage id="form.dateOfBirth.junior.handlers.label" />
                        }
                      >
                        {getFieldDecorator('dateOfBirth', {
                          rules: [
                            {
                              required: true
                            }
                          ]
                        })(
                          <DatePicker
                            style={{ width: '100%' }}
                            placeholder={formatMessage({
                              id: 'form.dateOfBirth.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={12} md={12} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.parent.first.name" />}
                      >
                        {getFieldDecorator('parentFirstName', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.frozen.semen.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.parent.first.name'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={12} md={12} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.parent.last.name" />}
                      >
                        {getFieldDecorator('parentLastName', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.frozen.semen.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.parent.last.name'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.address1.label" />}
                      >
                        {getFieldDecorator('address1', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.address.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.address1.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.address2.label" />}
                      >
                        {getFieldDecorator('address2', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.address.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.address2.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.address3.label" />}
                      >
                        {getFieldDecorator('address3', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.address.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.address3.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.city.label" />}
                      >
                        {getFieldDecorator('city', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.city.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.city.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.state.label" />}
                      >
                        {getFieldDecorator('state', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.state.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.state.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.country.label" />}
                      >
                        {getFieldDecorator('country', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.country.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.country.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.zip.label" />}
                      >
                        {getFieldDecorator('zip', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.zip.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.zip.label'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={
                          <FormattedMessage id="form.international.label" />
                        }
                      >
                        {getFieldDecorator('international', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.international.required'
                              })
                            }
                          ]
                        })(
                          <Select
                            showSearch
                            placeholder={formatMessage({
                              id: 'form.international.label'
                            })}
                            optionFilterProp="children"
                            filterOption={(input, option) =>
                              option.props.children
                                .toLowerCase()
                                .indexOf(input.toLowerCase()) >= 0
                            }
                          >
                            <Option value="true">Yes</Option>
                            <Option value="false">No</Option>
                          </Select>
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.mail.placeholder" />}
                      >
                        {getFieldDecorator('email', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.email.required'
                              })
                            },
                            {
                              type: 'email',
                              message: formatMessage({
                                id: 'validation.email.wrong-format'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.mail.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={
                          <FormattedMessage id="form.phone-number.label" />
                        }
                      >
                        {getFieldDecorator('phone', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.phone-number.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.phone-number.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.cell" />}
                      >
                        {getFieldDecorator('cell', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.frozen.semen.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({ id: 'form.cell' })}
                          />
                        )}
                      </FormItem>
                    </Col>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={
                          <FormattedMessage id="form.type.requested.label" />
                        }
                      >
                        {getFieldDecorator('typeOfRequest', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.frozen.semen.required'
                              })
                            }
                          ]
                        })(
                          <Select
                            showSearch
                            placeholder={formatMessage({
                              id: 'form.frozen.semen.placeholder'
                            })}
                            optionFilterProp="children"
                            onChange={this.handleTypeSelect}
                            filterOption={(input, option) =>
                              option.props.children
                                .toLowerCase()
                                .indexOf(input.toLowerCase()) >= 0
                            }
                          >
                            <Option value="isInternational">
                              International
                            </Option>
                            <Option value="overnightRequested">
                              OverNight Requested
                            </Option>
                            <Option value="rushRequested">
                              Rush Requested
                            </Option>
                          </Select>
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                  <Row gutter={16}>
                    <Col lg={8} md={8} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={
                          <FormattedMessage id="form.certification.number" />
                        }
                      >
                        {getFieldDecorator('certificateNumber', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.breed.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.certification.number'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                </Spin>
                <FormItem {...submitFormLayout} style={{ marginTop: 32 }}>
                  {!statusStart && updated && (
                    <Button
                      type="primary"
                      id="start"
                      htmlType="submit"
                      loading={submitting}
                      onClick={() =>
                        this.setState({ buttonType: 'start' }, () => {
                          this.handleSubmitDraft();
                        })
                      }
                    >
                      <FormattedMessage id="form.start.registration" />
                    </Button>
                  )}
                  {showStatus && !updated && (
                    <React.Fragment>
                      <Button
                        type="primary"
                        id="submit"
                        htmlType="button"
                        loading={submitting}
                        onClick={e => this.getButtonId(e, 'submit', regId)}
                      >
                        <FormattedMessage id="form.submit.now" />
                      </Button>
                      <Button
                        type="primary"
                        id="later"
                        style={{ marginLeft: 8 }}
                        htmlType="button"
                        loading={submitting}
                        onClick={e => this.getButtonId(e, 'later', regId)}
                      >
                        <FormattedMessage id="form.draft.registration" />
                      </Button>
                    </React.Fragment>
                  )}
                </FormItem>
              </div>
              <Modal
                title="Update Registration Fee"
                className={styles.updateFeeModal}
                visible={showSubmitModal}
                onOk={this.handleSubmit}
                onCancel={this.handleCancel}
                footer={[
                  // <Button key="submit" type="primary" onClick={() => { this.setState({ showSubmitModal: false }, () => <Redirect to="/registration-list" />) }}>
                  // <Link href="/registration-list">
                  <Button key="submit" type="primary" onClick={this.handleOk}>
                    <FormattedMessage id="menu.btn.ok" />
                  </Button>
                  // </Link>
                ]}
              >
                <div>
                  <Row>
                    <Col lg={12} md={12} sm={24}>
                      <label>
                        <FormattedMessage id="SubTotal" />:
                      </label>
                      <label>
                        {pedigreePaymentQuote && pedigreePaymentQuote.subTotal}
                      </label>
                    </Col>
                  </Row>
                  <Row>
                    <Col lg={12} md={12} sm={24}>
                      <label>
                        <FormattedMessage id="Transaction Fee" />:
                      </label>
                      <label>
                        {pedigreePaymentQuote && pedigreePaymentQuote.transactionFee}
                      </label>
                    </Col>
                  </Row>
                </div>
              </Modal>
            </Form>
          </Spin>
        </Card>
      </PageHeaderWrapper>
    );
  }
}

export default JuniorHandlersRegistrationForm;