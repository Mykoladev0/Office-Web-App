/* eslint-disable jsx-a11y/label-has-for */
/* eslint-disable jsx-a11y/label-has-associated-control */
import React, { PureComponent } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import { formatMessage, FormattedMessage } from 'umi/locale';
import DogNameSearch from '@/components/DogNameSearch/DogNameSearch';

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

import { ChartCard } from '@/components/Charts';

const FormItem = Form.Item;
const { Option } = Select;
const { Link } = Anchor;
const { Search } = Input;

@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  statusStart: form.statusStart,
  colorList: form.colors,
  registration: form.registration,
  startRegistration: form.startRegistration,
  startLitterRegistration: form.startLitterRegistration,
  damNameList: form.damNameList,
  sireNameList: form.sireNameList,
  searchDamName: form.searchDamName,
  ownerName: form.ownerName,
  searchOwnerName: form.searchOwnerName,
  ownerDetails: form.ownerDetails,
  spinner: form.spinner,
  pedigreePaymentQuote: form.pedigreePaymentQuote,
  submitSuccess: form.submitSuccess,
  damName: '',
  selectedDam: null,
  selectedSire: null,
  sireName: '',
  isRegStarted: false,
  submitting: loading.effects['form/submitRegularForm'],
}))
@Form.create()
class LitterRegistrationForm extends PureComponent {
  state = {
    buttonType: 'start',
    currentRegId: '',
    currentDogId: '',
    updated: true,
    dogType: '',
    damName: '',
    sireName: '',
    showStartSubmission: true,
    overnightRequested: false,
    rushRequested: false,
    isInternational: false,
    isFrozenSemenUsed: false,
    isRegStarted: false,
    isEditing: false,
    selectedDam: null,
    selectedSire: null,
    showSubmitModal: false
  };

  componentDidMount() {
    const { dispatch, location } = this.props;
    dispatch({
      type: 'form/getBreeds',
    });
    dispatch({
      type: 'form/getColors',
    });
    if (location.state && location.state.id) {
      this.setState({ isEditing: true });
      dispatch({
        type: 'list/getLitterById',
        payload: {
          id: location.state.id,
        },
      });
    } else {
      dispatch({
        type: 'form/setStatus',
      });
    }
  }

  componentWillReceiveProps(nextProps) {
    const { editLitterList } = nextProps.list;
    const { updated } = this.state;
    const { location } = nextProps;
    const { form } = this.props;
    const { currentUsers } = nextProps.list;
    // if (!nextProps.spinner) {
    //   this.setState({
    //     isRegStarted: false,
    //   });
    // }
    const userRole = currentUsers && currentUsers.abkcRolesUserBelongsTo[1];
    if (nextProps.submitSuccess && (userRole === "Owners" || userRole === "Representatives")) {
      this.setState({ showSubmitModal: true });
    }
    if (updated && editLitterList && location.state && location.state.id) {
      const dataToUpdate = {
        damId: editLitterList.damInfo.dogName,
        sireName: editLitterList.sireInfo.dogName,
        damName: editLitterList.damInfo.dogName,
        sireId: editLitterList.sireInfo.dogName,
        dateOfBreeding: editLitterList.dateOfBreeding ? moment(editLitterList.dateOfBreeding) : '',
        dateOfLitterBirth: editLitterList.dateOfLitterBirth
          ? moment(editLitterList.dateOfLitterBirth)
          : '',
        dateSemenCollected: editLitterList.dateSemenCollected
          ? moment(editLitterList.dateSemenCollected)
          : '',
        breed: editLitterList.breed && editLitterList.breed.id,
        numberOfMalesBeingRegistered: editLitterList.numberOfMalesBeingRegistered,
        numberOfFemalesBeingRegistered: editLitterList.numberOfFemalesBeingRegistered,
        frozenSemenUsed: editLitterList.frozenSemenUsed.toString(),
        typeOfRequest:
          (editLitterList.isInternational && 'isInternational') ||
          (editLitterList.overnightRequested && 'overnightRequested') ||
          (editLitterList.rushRequested && 'rushRequested') ||
          '',
      };
      form.setFieldsValue({}, () => form.setFieldsValue(dataToUpdate));
      this.setState({
        updated: false,
        isEditing: false,
        isRegStarted: true,
        damName: editLitterList.damInfo.dogName,
        sireName: editLitterList.sireInfo.dogName,
      });
    }
  }

  handleOk = () => {
    const { dispatch } = this.props;
    this.setState({
      showSubmitModal: false
    });
    dispatch({
      type: 'form/redirectPage'
    });
  }

  handleTypeSelect = value => {
    if (value === 'isInternational') {
      this.setState({
        isInternational: true,
        overnightRequested: false,
        rushRequested: false,
      });
    } else if (value === 'overnightRequested') {
      this.setState({
        isInternational: false,
        overnightRequested: true,
        rushRequested: false,
      });
    } else if (value === 'rushRequested') {
      this.setState({
        isInternational: false,
        overnightRequested: false,
        rushRequested: true,
      });
    }
  };

  onUploadDocument = (e, id, val) => {
    const { dispatch } = this.props;
    const fileset = e.target.files;
    dispatch({
      type: 'form/submitSupportinngDoc',
      payload: {
        id,
        file: fileset[0],
        documentType: val,
        registrationType: 'litter',
      },
    });
  };

  getButtonId = (e, id, regId) => {
    this.setState(
      {
        buttonType: id,
        currentRegId: regId,
      },
      () => {
        this.handleSubmitDraft();
      }
    );
  };

  info = (value, currentRegId) => {
    const {
      overnightRequested,
      rushRequested,
      isInternational,
      selectedDam,
      selectedSire,
    } = this.state;
    if (currentRegId) {
      return {
        dateOfBreeding: value.dateOfBreeding,
        dateOfLitterBirth: value.dateOfLitterBirth,
        breed: {
          id: value.breed,
        },
        numberOfMalesBeingRegistered: value.numberOfMalesBeingRegistered,
        numberOfFemalesBeingRegistered: value.numberOfFemalesBeingRegistered,
        frozenSemenUsed: value.frozenSemenUsed,
        dateSemenCollected: value.dateSemenCollected,
        id: currentRegId,
        overnightRequested,
        rushRequested,
        isInternational,
      };
    }
    return {
      dogInfo: {
        sireId: selectedSire.originalTableId, // value.sireId,
        damId: selectedDam.originalTableId, // value.damId,
      },
    };
  };

  handleSubmitDraft = e => {
    const { dispatch, form } = this.props;
    const { buttonType, currentRegId, currentDogId } = this.state;
    const role = localStorage.getItem('user-role');

    if (buttonType === 'start') {
      form.validateFields(['damId', 'sireId'], errors => {
        const value = form.getFieldsValue();
        this.info(value, currentRegId);
        if (!errors) {
          dispatch({
            type: 'form/startLitterRegistration',
            payload: this.info(value, currentDogId, currentRegId),
          });
          this.setState({ showStartSubmission: false, isRegStarted: true });
        }
      });
    } else if (buttonType === 'later') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, currentRegId);
        if (!err) {
          dispatch({
            type: 'form/draftLitterRegistrationForm',
            payload: this.info(value, currentRegId),
            content: {
              currentRegId,
              redirect: true,
              callSubmit: true,
              registrationType: 'litter',
            },
          });
        }
      });
    } else if (buttonType === 'submit') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, currentRegId);
        if (!err) {
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitLitterRegistrationForm',
              payload: this.info(value, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'litter',
              },
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'litter',
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: this.info(value, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'litter',
                regArray,
                draftApi: 'draftLitterRegistrationForms',
              },
            });
          }
        }
      });
    }
  };

  handleCancel = () => {
    this.setState({ showSubmitModal: false });
  };

  disabledDate = current => current && current > moment().endOf('day');

  render() {
    const {
      submitting,
      breedsList,
      status,
      statusStart,
      registration,
      startLitterRegistration,
      searchDamName,
      location,
      damNameList,
      sireNameList,
      pedigreePaymentQuote
    } = this.props;
    const {
      updated,
      damName,
      sireName,
      selectedDam,
      selectedSire,
      dogType,
      showStartSubmission,
      isRegStarted,
      isEditing,
      showSubmitModal
    } = this.state;
    let optionsSire = [];
    let optionsDam = [];

    if (searchDamName === damName && damNameList.length && dogType === 'dam') {
      optionsDam = damNameList
        .slice(0, 5)
        .map(d => <Option key={d.originalTableId}>{d.dogName}</Option>);
    } else if (searchDamName === damName && sireNameList.length && dogType === 'sire') {
      optionsSire = sireNameList
        .slice(0, 5)
        .map(d => <Option key={d.originalTableId}>{d.dogName}</Option>);
    } else {
      optionsSire = [];
      optionsDam = [];
    }

    let regId = '';
    if (statusStart && startLitterRegistration) {
      regId = startLitterRegistration.id;
      // dogId = startLitterRegistration.dogInfo && startLitterRegistration.dogInfo.id
    }
    if (status && registration) {
      regId = registration.id;
    } else if (location.state) {
      regId = location.state.id;
    }
    // supporting documents
    const documents = [
      { type: 'sireOwnerSignature', required: true },
      { type: 'sireCoOwnerSignature', required: false },
      { type: 'damOwnerSignature', required: true },
      { type: 'damCoOwnerSignature', required: true },
    ];
    const {
      form: { getFieldDecorator },
    } = this.props;
    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 7 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 12 },
        md: { span: 10 },
      },
    };
    const formLabelItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 7 },
        md: { span: 7 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 12 },
        md: { span: 12 },
      },
    };

    const submitFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 10 },
      },
    };

    return (
      <PageHeaderWrapper
        className={styles.formHeader}
        title={<FormattedMessage id="form.litter.header.text" />}
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
                    <FormattedMessage id="form.litter.header.sub1.text" />
                  </h2>
                  <Divider />
                </Row>
                {!isRegStarted && (
                  <Row gutter={24}>
                    <Col className={styles.searchButton}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.sireName.label" />}
                      >
                        <DogNameSearch
                          genderFilter="Male"
                          handleSelectionFn={value => {
                            this.setState({
                              selectedSire: value,
                              sireName: value ? value.dogName : '',
                            });
                          }}
                          prompt={formatMessage({
                            id: 'form.sireName.placeholder',
                          })}
                        />
                      </FormItem>
                    </Col>
                    <Col className={styles.searchButton}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.damName.label" />}
                      >
                        <DogNameSearch
                          genderFilter="Female"
                          handleSelectionFn={value =>
                            this.setState({
                              selectedDam: value,
                              damName: value ? value.dogName : '',
                            })
                          }
                          prompt={formatMessage({
                            id: 'form.damName.placeholder',
                          })}
                        />
                      </FormItem>
                    </Col>
                    {!statusStart && showStartSubmission && updated && (
                      <FormItem {...submitFormLayout} style={{ marginTop: 32 }}>
                        <Button
                          type="primary"
                          id="start"
                          disabled={selectedDam === null && selectedSire === null}
                          style={{ marginLeft: 8 }}
                          htmlType="submit"
                          loading={submitting}
                          onClick={e => this.getButtonId(e, 'start')}
                        >
                          <FormattedMessage id="form.start.registration" />
                        </Button>
                      </FormItem>
                    )}
                  </Row>
                )}
                {isRegStarted && (
                  <Row gutter={0}>
                    <Col span={12}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.damName.label" />}
                      >
                        <Input readonly value={damName} />
                      </FormItem>
                    </Col>
                    <Col span={12}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.sireName.label" />}
                      >
                        <Input readonly value={sireName} />
                      </FormItem>
                    </Col>
                  </Row>
                )}
              </div>
              {statusStart || !updated ? (
                <div>
                  <div>
                    <Row gutter={16}>
                      <h2>
                        <FormattedMessage id="form.litter.header.sub2.text" />
                      </h2>
                      <Divider />
                    </Row>
                    <Row gutter={24}>
                      <Col lg={10} md={10} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.date.of.breeding.label" />}
                        >
                          {getFieldDecorator('dateOfBreeding', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.date.of.breeding.required',
                                }),
                              },
                            ],
                          })(
                            <DatePicker
                              style={{ width: '100%' }}
                              placeholder={formatMessage({
                                id: 'form.dateOfBirth.placeholder',
                              })}
                            />
                          )}
                        </FormItem>
                      </Col>
                      <Col lg={14} md={14} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.frozen.semen.label" />}
                        >
                          {getFieldDecorator('frozenSemenUsed', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.frozen.semen.required',
                                }),
                              },
                            ],
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.frozen.semen.placeholder',
                              })}
                              optionFilterProp="children"
                              onSelect={val => this.setState({ isFrozenSemenUsed: val })}
                              filterOption={(input, option) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >=
                                0
                              }
                            >
                              <Option value={true}>Yes</Option>
                              <Option value={false}>No</Option>
                            </Select>
                          )}
                        </FormItem>
                      </Col>
                    </Row>
                    <Row gutter={24}>
                      <Col lg={10} md={10} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.breed.label" />}
                        >
                          {getFieldDecorator('breed', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.breed.required',
                                }),
                              },
                            ],
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.breed.placeholder',
                              })}
                              optionFilterProp="children"
                              filterOption={(input, option) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >=
                                0
                              }
                            >
                              {breedsList.map(val => (
                                <Option key={val.id} value={val.id}>
                                  {val.breed}
                                </Option>
                              ))}
                            </Select>
                          )}
                        </FormItem>
                      </Col>
                      <Col lg={14} md={14} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.registered.male.label" />}
                        >
                          {getFieldDecorator('numberOfMalesBeingRegistered', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.registered.male.required',
                                }),
                              },
                            ],
                          })(
                            <Input
                              placeholder={formatMessage({
                                id: 'form.registered.male.placeholder',
                              })}
                            />
                          )}
                        </FormItem>
                      </Col>
                    </Row>
                    <Row gutter={24}>
                      <Col lg={10} md={10} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.dateOfBirth.litter.label" />}
                        >
                          {getFieldDecorator('dateOfLitterBirth', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.dateOfBirth.required',
                                }),
                              },
                            ],
                          })(
                            <DatePicker
                              style={{ width: '100%' }}
                              placeholder={formatMessage({
                                id: 'form.dateOfBirth.placeholder',
                              })}
                              disabledDate={this.disabledDate}
                            />
                          )}
                        </FormItem>
                      </Col>
                      <Col lg={14} md={14} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.registered.female.label" />}
                        >
                          {getFieldDecorator('numberOfFemalesBeingRegistered', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.registered.female.required',
                                }),
                              },
                            ],
                          })(
                            <Input
                              placeholder={formatMessage({
                                id: 'form.registered.female.placeholder',
                              })}
                            />
                          )}
                        </FormItem>
                      </Col>
                    </Row>
                    <Row gutter={24}>
                      <Col lg={10} md={10} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.type.requested.label" />}
                        >
                          {getFieldDecorator('typeOfRequest', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.type.requested.required',
                                }),
                              },
                            ],
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.frozen.semen.placeholder',
                              })}
                              optionFilterProp="children"
                              onChange={this.handleTypeSelect}
                              filterOption={(input, option) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >=
                                0
                              }
                            >
                              <Option value="none">None</Option>
                              <Option value="isInternational">International</Option>
                              <Option value="overnightRequested">Overnight requested</Option>
                              <Option value="rushRequested">Rush requested</Option>
                            </Select>
                          )}
                        </FormItem>
                      </Col>
                      {this.state.isFrozenSemenUsed && (
                        <Col lg={14} md={14} sm={24}>
                          <FormItem
                            {...formLabelItemLayout}
                            label={<FormattedMessage id="form.semen.date.label" />}
                          >
                            {getFieldDecorator('dateSemenCollected', {
                              rules: [
                                {
                                  required: true,
                                  message: formatMessage({
                                    id: 'validation.semen.date.required',
                                  }),
                                },
                              ],
                            })(
                              <DatePicker
                                style={{ width: '100%' }}
                                placeholder={formatMessage({
                                  id: 'form.dateOfBirth.placeholder',
                                })}
                              />
                            )}
                          </FormItem>
                        </Col>
                      )}
                    </Row>
                    <div>
                      <Row gutter={16}>
                        <h2>
                          <FormattedMessage id="form.upload.document" />
                        </h2>
                        <p>
                          <FormattedMessage id="form.upload.text" />
                        </p>
                        <Divider />
                      </Row>
                      <Row gutter={24} className={styles.UploadContent}>
                        {documents.map(val => (
                          <Col lg={8} md={8} sm={24} key={val.type}>
                            <ChartCard
                              bordered={false}
                              title={val.type}
                              footer={
                                <input
                                  name="docFile"
                                  required={
                                    this.state.buttonType === 'submit' ? val.required : false
                                  }
                                  type="file"
                                  onChange={e => this.onUploadDocument(e, regId, val.type)}
                                />
                              }
                              contentHeight={46}
                            />
                          </Col>
                        ))}
                      </Row>
                    </div>
                    <Row>
                      <FormItem {...submitFormLayout} style={{ marginTop: 32 }}>
                        <Button
                          type="primary"
                          id="submit"
                          style={{ marginLeft: 8 }}
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
                      </FormItem>
                    </Row>
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
                </div>
              ) : (
                  ''
                )}
            </Form>
          </Spin>
        </Card>
      </PageHeaderWrapper>
    );
  }
}

export default LitterRegistrationForm;