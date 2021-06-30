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
  LocaleProvider,
  Col,
  Row,
  Divider,
  Spin,
  Anchor,
  Modal
} from 'antd';
import enGB from 'antd/lib/locale-provider/en_GB';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import styles from './style.less';
import { ChartCard } from '@/components/Charts';

const FormItem = Form.Item;
const { Option } = Select;
const { Link } = Anchor;
@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  statusStart: form.statusStart,
  colorList: form.colors,
  registration: form.registration,
  startRegistration: form.startRegistration,
  startPuppyRegistration: form.startPuppyRegistration,
  originalTableId: form.originalTableId,
  damNameList: form.damNameList,
  sireNameList: form.sireNameList,
  searchDamName: form.searchDamName,
  ownerName: form.ownerName,
  searchOwnerName: form.searchOwnerName,
  ownerDetails: form.ownerDetails,
  spinner: form.spinner,
  pedigreePaymentQuote: form.pedigreePaymentQuote,
  submitSuccess: form.submitSuccess,
  submitting: loading.effects['form/submitRegularForm']
}))
@Form.create()
class PuppyRegistrationForm extends PureComponent {
  state = {
    buttonType: 'start',
    currentRegId: '',
    currentDogId: '',
    updated: true,
    ownerName: '',
    coOwnerName: '',
    isOwner: false,
    isCoOwner: false,
    OwnerId: '',
    coOwnerId: '',
    showStartSubmission: true,
    overnightRequested: false,
    rushRequested: false,
    isInternational: false,
    isCoOwnerSearching: false,
    isOwnerSearching: false,
    isAbkcSearching: false,
    isEditing: false
  };

  componentDidMount() {
    const { dispatch, location } = this.props;
    dispatch({
      type: 'form/getBreeds'
    });
    dispatch({
      type: 'form/getColors'
    });
    dispatch({
      type: 'form/setStatus'
    });
    if (location.state && location.state.id) {
      this.setState({ isEditing: true });
      dispatch({
        type: 'list/getPuppyById',
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
    const { updated, isOwner, isCoOwner } = this.state;
    const { form } = this.props;
    const { editPuppyList } = nextProps.list;
    const { ownerDetails, location } = nextProps;
    const { currentUsers } = nextProps.list;
    const userRole = currentUsers && currentUsers.abkcRolesUserBelongsTo[1];
    if (nextProps.submitSuccess && (userRole === "Owners" || userRole === "Representatives")) {
      this.setState({ showSubmitModal: true });
    }
    if (!nextProps.spinner) {
      this.setState({
        isOwnerSearching: false,
        isCoOwnerSearching: false,
        isAbkcSearching: false
      });
    }
    if (!!ownerDetails && isOwner) {
      form.setFieldsValue({
        lastName: ownerDetails && ownerDetails.lastName,
        firstName: ownerDetails && ownerDetails.firstName,
        address1: ownerDetails && ownerDetails.address1,
        address2: ownerDetails && ownerDetails.address2,
        address3: ownerDetails && ownerDetails.address3,
        city: ownerDetails && ownerDetails.city,
        state: ownerDetails && ownerDetails.state,
        zip: ownerDetails && ownerDetails.zip,
        country: ownerDetails && ownerDetails.country,
        international: ownerDetails && ownerDetails.international,
        email: ownerDetails && ownerDetails.email,
        phone: ownerDetails && ownerDetails.phone
      });
      this.setState({
        isOwner: false
      });
    }

    if (!!ownerDetails && isCoOwner) {
      form.setFieldsValue({
        lastName2: ownerDetails && ownerDetails.lastName,
        firstName2: ownerDetails && ownerDetails.firstName,
        address12: ownerDetails && ownerDetails.address1,
        address22: ownerDetails && ownerDetails.address2,
        address32: ownerDetails && ownerDetails.address3,
        city2: ownerDetails && ownerDetails.city,
        state2: ownerDetails && ownerDetails.state,
        zip2: ownerDetails && ownerDetails.zip,
        country2: ownerDetails && ownerDetails.country,
        international2: ownerDetails && ownerDetails.international,
        email2: ownerDetails && ownerDetails.email,
        phone2: ownerDetails && ownerDetails.phone
      });
      this.setState({
        isCoOwner: false
      });
    }
    if (
      editPuppyList &&
      editPuppyList.id &&
      updated &&
      location.state &&
      location.state.id
    ) {
      this.setState(
        {
          updated: false,
          isEditing: false,
          OwnerId: editPuppyList.newOwner && editPuppyList.newOwner.id,
          coOwnerId: editPuppyList.newCoOwner && editPuppyList.newCoOwner.id
        },
        () => {
          form.setFieldsValue({
            dogName:
              editPuppyList.dogInfo.dogName && editPuppyList.dogInfo.dogName,
            sellDate: editPuppyList.dogInfo.sellDate
              ? moment(editPuppyList.dogInfo.sellDate)
              : '',
            gender:
              editPuppyList.dogInfo.gender && editPuppyList.dogInfo.gender,
            microchipNumber:
              editPuppyList.dogInfo.microchipNumber &&
              editPuppyList.dogInfo.microchipNumber,
            microchipType:
              editPuppyList.dogInfo.microchipType &&
              editPuppyList.dogInfo.microchipType,
            colorId:
              editPuppyList.dogInfo.colorId && editPuppyList.dogInfo.colorId,
            OwnerId: editPuppyList.newOwner && editPuppyList.newOwner.id,
            lastName: editPuppyList.newOwner && editPuppyList.newOwner.lastName,
            firstName:
              editPuppyList.newOwner && editPuppyList.newOwner.firstName,
            address1: editPuppyList.newOwner && editPuppyList.newOwner.address1,
            address2: editPuppyList.newOwner && editPuppyList.newOwner.address2,
            address3: editPuppyList.newOwner && editPuppyList.newOwner.address3,
            city: editPuppyList.newOwner && editPuppyList.newOwner.city,
            state: editPuppyList.newOwner && editPuppyList.newOwner.state,
            zip: editPuppyList.newOwner && editPuppyList.newOwner.zip,
            country: editPuppyList.newOwner && editPuppyList.newOwner.country,
            international:
              editPuppyList.newOwner &&
              editPuppyList.newOwner.international.toString(),
            email: editPuppyList.newOwner && editPuppyList.newOwner.email,
            phone: editPuppyList.newOwner && editPuppyList.newOwner.phone,
            coOwnerId: editPuppyList.newCoOwner && editPuppyList.newCoOwner.id,
            lastName2:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.lastName,
            firstName2:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.firstName,
            address12:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.address1,
            address22:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.address2,
            address32:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.address3,
            city2: editPuppyList.newCoOwner && editPuppyList.newCoOwner.city,
            state2: editPuppyList.newCoOwner && editPuppyList.newCoOwner.state,
            zip2: editPuppyList.newCoOwner && editPuppyList.newCoOwner.zip,
            country2:
              editPuppyList.newCoOwner && editPuppyList.newCoOwner.country,
            international2:
              editPuppyList.newCoOwner &&
              editPuppyList.newCoOwner.international.toString(),
            email2: editPuppyList.newCoOwner && editPuppyList.newCoOwner.email,
            phone2: editPuppyList.newCoOwner && editPuppyList.newCoOwner.phone,
            typeOfRequest:
              (editPuppyList.isInternational && 'isInternational') ||
                (editPuppyList.overnightRequested && 'overnightRequested') ||
                editPuppyList.rushRequested
                ? 'rushRequested'
                : ''
          });
        }
      );
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

  info = (value, currentDogId, currentRegId) => {
    const {
      OwnerId,
      coOwnerId,
      overnightRequested,
      rushRequested,
      isInternational
    } = this.state;
    if (currentDogId && currentRegId && OwnerId && coOwnerId) {
      return {
        id: currentRegId,
        dogName: value.dogName,
        sellDate: moment(value.sellDate).format(),
        microchipNumber: value.microchipNumber && value.microchipNumber,
        microchipType: value.microchipType && value.microchipType,
        colorId: value.colorId,
        isInternational,
        overnightRequested,
        rushRequested,
        owner: {
          id: OwnerId,
          lastName: value.lastName,
          firstName: value.firstName,
          address1: value.address1,
          address2: value.address2,
          address3: value.address3,
          city: value.city,
          state: value.state,
          zip: value.zip,
          country: value.country,
          international: value.international,
          email: value.email,
          phone: value.phone
        },
        coOwner: {
          id: coOwnerId,
          lastName: value.lastName2,
          firstName: value.firstName2,
          address1: value.address12,
          address2: value.address22,
          address3: value.address32,
          city: value.city2,
          state: value.state2,
          zip: value.zip2,
          country: value.country2,
          international: value.international2,
          email: value.email2,
          phone: value.phone2
        }
      };
    }
    if (currentDogId && currentRegId && OwnerId) {
      return {
        id: currentRegId,
        dogName: value.dogName,
        sellDate: moment(value.sellDate).format(),
        microchipNumber: value.microchipNumber && value.microchipNumber,
        microchipType: value.microchipType && value.microchipType,
        colorId: value.colorId,
        isInternational,
        overnightRequested,
        rushRequested,
        owner: {
          id: OwnerId,
          lastName: value.lastName,
          firstName: value.firstName,
          address1: value.address1,
          address2: value.address2,
          address3: value.address3,
          city: value.city,
          state: value.state,
          zip: value.zip,
          country: value.country,
          international: value.international,
          email: value.email,
          phone: value.phone
        },
        coOwner: {
          id: coOwnerId,
          lastName: value.lastName2,
          firstName: value.firstName2,
          address1: value.address12,
          address2: value.address22,
          address3: value.address32,
          city: value.city2,
          state: value.state2,
          zip: value.zip2,
          country: value.country2,
          international: value.international2,
          email: value.email2,
          phone: value.phone2
        }
      };
    }
    if (currentDogId && currentRegId) {
      return {
        id: currentRegId,
        dogName: value.dogName,
        sellDate: moment(value.sellDate).format(),
        microchipNumber: value.microchipNumber && value.microchipNumber,
        microchipType: value.microchipType && value.microchipType,
        colorId: value.colorId,
        isInternational,
        overnightRequested,
        rushRequested,
        owner: {
          id: OwnerId,
          lastName: value.lastName,
          firstName: value.firstName,
          address1: value.address1,
          address2: value.address2,
          address3: value.address3,
          city: value.city,
          state: value.state,
          zip: value.zip,
          country: value.country,
          international: value.international,
          email: value.email,
          phone: value.phone
        },
        coOwner: {
          id: coOwnerId,
          lastName: value.lastName2,
          firstName: value.firstName2,
          address1: value.address12,
          address2: value.address22,
          address3: value.address32,
          city: value.city2,
          state: value.state2,
          zip: value.zip2,
          country: value.country2,
          international: value.international2,
          email: value.email2,
          phone: value.phone2
        }
      };
    }
    return {
      id: currentRegId,
      dogName: value.dogName,
      sellDate: moment(value.sellDate).format(),
      microchipNumber: value.microchipNumber && value.microchipNumber,
      microchipType: value.microchipType && value.microchipType,
      colorId: value.colorId,
      isInternational,
      overnightRequested,
      rushRequested,
      newOwner: {
        id: OwnerId,
        lastName: value.lastName,
        firstName: value.firstName,
        address1: value.address1,
        address2: value.address2,
        address3: value.address3,
        city: value.city,
        state: value.state,
        zip: value.zip,
        country: value.country,
        international: value.international,
        email: value.email,
        phone: value.phone
      },
      newCoOwner: {
        id: coOwnerId,
        lastName: value.lastName2,
        firstName: value.firstName2,
        address1: value.address12,
        address2: value.address22,
        address3: value.address32,
        city: value.city2,
        state: value.state2,
        zip: value.zip2,
        country: value.country2,
        international: value.international2,
        email: value.email2,
        phone: value.phone2
      }
    };
  };

  handleSubmitDraft = () => {
    const { dispatch, form } = this.props;
    const { buttonType, currentRegId, currentDogId } = this.state;
    const role = localStorage.getItem('user-role');

    if (buttonType === 'start') {
      form.validateFields(['abkcNumber'], errors => {
        const value = form.getFieldsValue();
        this.info(value, currentDogId, currentRegId);
        if (!errors) {
          this.setState({
            showStartSubmission: false,
            isAbkcSearching: true
          });
          dispatch({
            type: 'form/validateAbkcNumber',
            // payload: this.info(value, currentDogId, currentRegId),
            payload: { abkcNumber: value.abkcNumber }
          });
        }
      });
    } else if (buttonType === 'later') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, currentDogId, currentRegId);
        if (!err) {
          dispatch({
            type: 'form/draftPuppyRegistrationForm',
            payload: this.info(value, currentDogId, currentRegId),
            content: {
              currentRegId,
              redirect: true,
              callSubmit: true,
              registrationType: 'puppy'
            }
          });
        }
      });
    } else if (buttonType === 'submit') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, currentDogId, currentRegId);
        if (!err) {
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitPuppyRegistrationForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'puppy'
              }
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'puppy'
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'puppy',
                regArray,
                draftApi: 'draftPuppyRegistrationForms'
              }
            });
          }
        }
      });
    }
  };

  handleSearchOwner = () => {
    const { dispatch } = this.props;
    const { ownerName } = this.state;
    this.setState({ isOwnerSearching: true, ownerName: '' });
    if (ownerName) {
      dispatch({
        type: 'form/searchOwnerName',
        payload: ownerName
      });
    }
  };

  handleSearchCoOwner = () => {
    const { dispatch } = this.props;
    const { coOwnerName } = this.state;
    this.setState({ isCoOwnerSearching: true, coOwnerName: '' });
    if (coOwnerName) {
      dispatch({
        type: 'form/searchOwnerName',
        payload: coOwnerName
      });
    }
  };

  handleChange = value => {
    const { dispatch } = this.props;
    if (value) {
      dispatch({
        type: 'form/getOwnerDetails',
        payload: value
      });
      this.setState({
        isOwner: true,
        OwnerId: value
      });
    }
  };

  handleCoOwnerChange = value => {
    const { dispatch } = this.props;
    if (value) {
      dispatch({
        type: 'form/getOwnerDetails',
        payload: value
      });
      this.setState({
        isCoOwner: true,
        coOwnerId: value
      });
    }
  };

  disabledDate = current => current && current > moment().endOf('day');

  onUploadDocument = (e, id, val) => {
    const { dispatch } = this.props;
    const fileset = e.target.files;
    dispatch({
      type: 'form/submitSupportinngDoc',
      payload: {
        id,
        file: fileset[0],
        documentType: val,
        registrationType: 'puppy'
      }
    });
  };

  handleCancel = () => {
    this.setState({ showSubmitModal: false });
  };

  render() {
    const {
      submitting,
      colorList,
      statusStart,
      startPuppyRegistration,
      searchOwnerName,
      location,
      pedigreePaymentQuote
    } = this.props;
    const {
      updated,
      showStartSubmission,
      ownerName,
      isAbkcSearching,
      isEditing,
      showSubmitModal
    } = this.state;

    let ownerOptions = [];
    if (searchOwnerName === ownerName && this.props.ownerName.length) {
      ownerOptions = this.props.ownerName
        .slice(0, 5)
        .map(d => <Option key={d.id}>{d.fullName}</Option>);
    } else {
      ownerOptions = [];
    }

    let coOwnerOptions = [];
    if (this.props.ownerName && this.props.ownerName.length) {
      coOwnerOptions = this.props.ownerName
        .slice(0, 5)
        .map(d => <Option key={d.id}>{d.fullName}</Option>);
    } else {
      coOwnerOptions = [];
    }
    let regId = '0';
    if (statusStart && startPuppyRegistration) {
      regId = startPuppyRegistration.id;
    } else if (location.state) {
      regId = location.state.id;
    }

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
    const formLabelItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 4 },
        md: { span: 4 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 20 },
        md: { span: 20 }
      }
    };

    const draftFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 10 }
      }
    };
    const documents = [
      { type: 'sellerSignature', required: true },
      {
        type: 'coSellerSignature',
        required:
          startPuppyRegistration &&
            startPuppyRegistration.dogInfo &&
            startPuppyRegistration.dogInfo.coOwner
            ? true
            : false
      },
      { type: 'ownerSignature', required: true },
      { type: 'coOwnerSignature', required: false }
    ];
    const data = [];

    documents.map(val =>
      data.push({
        key: val.type,
        id: val.type,
        doc: val.type,
        required: val.required
      })
    );
    return (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.puppy.header.text" />}
        >
          <Card bordered={false}>
            <Anchor affix={false}>
              <Link href="/registration-list" title="Back" />
            </Anchor>
            <Spin spinning={isEditing}>
              <Form style={{ marginTop: 8 }}>
                <Spin spinning={isAbkcSearching}>
                  <div>
                    {(!statusStart || showStartSubmission) && (
                      <React.Fragment>
                        <Row gutter={16}>
                          <h2>
                            <FormattedMessage id="form.abkcNumber.info.label" />
                          </h2>
                          <Divider />
                        </Row>
                        <Row gutter={24}>
                          <Col lg={12} md={12} sm={24}>
                            <FormItem
                              {...formItemLayout}
                              label={
                                <FormattedMessage id="form.abkcNumber.label" />
                              }
                            >
                              {getFieldDecorator('abkcNumber', {
                                rules: [
                                  {
                                    required: true,
                                    message: formatMessage({
                                      id: 'validation.abkcNumber.required'
                                    })
                                  }
                                ]
                              })(
                                <Input
                                  placeholder={formatMessage({
                                    id: 'form.abkcNumber.placeholder'
                                  })}
                                />
                              )}
                            </FormItem>
                          </Col>
                        </Row>
                      </React.Fragment>
                    )}
                    <Row gutter={16}>
                      {/* {!statusStart && showStartSubmission && updated && ( */}
                      {(!statusStart || showStartSubmission) && (
                        <FormItem
                          {...draftFormLayout}
                          style={{ marginTop: 32 }}
                        >
                          <Button
                            type="primary"
                            id="start"
                            style={{ marginLeft: 8 }}
                            htmlType="button"
                            loading={submitting}
                            onClick={e => this.getButtonId(e, 'start')}
                          >
                            <FormattedMessage id="form.start.registration" />
                          </Button>
                        </FormItem>
                      )}
                      <Divider />
                    </Row>
                  </div>
                </Spin>
                {/* {statusStart || !updated ? ( */}
                {true ? (
                  <div>
                    <div>
                      <Row gutter={16}>
                        <h2>
                          <FormattedMessage id="form.dogInfo.text" />
                        </h2>
                        <Divider />
                      </Row>
                      <Row gutter={24}>
                        <Col lg={16} md={16} sm={24}>
                          <FormItem
                            {...formLabelItemLayout}
                            label={<FormattedMessage id="form.dogName.label" />}
                          >
                            {getFieldDecorator('dogName', {
                              rules: [
                                {
                                  // required: true,
                                  message: formatMessage({
                                    id: 'validation.dogName.required'
                                  })
                                }
                              ]
                            })(
                              <Input
                                placeholder={formatMessage({
                                  id: 'form.dogName.placeholder'
                                })}
                              />
                            )}
                          </FormItem>
                        </Col>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.sellDate.label" />
                            }
                          >
                            {getFieldDecorator('sellDate', {
                              rules: []
                            })(
                              <DatePicker
                                style={{ width: '100%' }}
                                placeholder={formatMessage({
                                  id: 'form.sellDate.placeholder'
                                })}
                                disabledDate={this.disabledDate}
                              />
                            )}
                          </FormItem>
                        </Col>
                      </Row>
                      <Row gutter={16}>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={<FormattedMessage id="form.color.label" />}
                          >
                            {getFieldDecorator('colorId', {
                              rules: []
                            })(
                              <Select
                                showSearch
                                placeholder={formatMessage({
                                  id: 'form.color.placeholder'
                                })}
                                optionFilterProp="children"
                                filterOption={(input, option) =>
                                  option.props.children
                                    .toLowerCase()
                                    .indexOf(input.toLowerCase()) >= 0
                                }
                              >
                                {colorList.map((val, key) => (
                                  <Option key={val} value={key}>
                                    {val}
                                  </Option>
                                ))}
                              </Select>
                            )}
                          </FormItem>
                        </Col>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.microChip.label" />
                            }
                          >
                            {getFieldDecorator('microchipNumber', {
                              rules: []
                            })(
                              <Input
                                placeholder={formatMessage({
                                  id: 'form.microChip.placeholder'
                                })}
                              />
                            )}
                          </FormItem>
                        </Col>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.microchipType.label" />
                            }
                          >
                            {getFieldDecorator('microchipType', {
                              rules: []
                            })(
                              <Input
                                placeholder={formatMessage({
                                  id: 'form.microchipType.placeholder'
                                })}
                              />
                            )}
                          </FormItem>
                        </Col>
                        <Divider />
                      </Row>
                    </div>
                    <div>
                      <Row gutter={16}>
                        <h2>
                          <FormattedMessage id="form.ownerInfo.text" />
                        </h2>
                        <Divider />
                      </Row>
                      <Row>
                        <h3>
                          <FormattedMessage id="form.select.existing.owner.label" />
                        </h3>
                      </Row>
                      <Row gutter={24}>
                        <Col
                          lg={16}
                          md={16}
                          sm={24}
                          className={styles.searchButton}
                        >
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.existinOwner.label" />
                            }
                          >
                            {getFieldDecorator('OwnerId', {
                              rules: []
                            })(
                              <Select
                                showSearch
                                placeholder={formatMessage({
                                  id: 'form.ownerName.placeholder'
                                })}
                                optionFilterProp="children"
                                filterOption={(input, option) =>
                                  option.props.children
                                    .toLowerCase()
                                    .indexOf(input.toLowerCase()) >= 0
                                }
                                onBlur={() => this.setState({ ownerName })}
                                onSearch={val =>
                                  this.setState({ ownerName: val })
                                }
                                onChange={this.handleChange}
                                value={this.state.ownerName}
                                defaultActiveFirstOption={false}
                                showArrow={false}
                                notFoundContent={null}
                                loading={this.state.isOwnerSearching}
                              >
                                {ownerOptions}
                              </Select>
                            )}
                          </FormItem>
                          {!this.state.isOwnerSearching && (
                            <FormItem>
                              <Button
                                type="primary"
                                icon="search"
                                disabled={this.state.ownerName.length < 3}
                                onClick={this.handleSearchOwner}
                              />
                            </FormItem>
                          )}
                        </Col>
                      </Row>
                      <Row gutter={24}>
                        {/* <Col lg={12} md={12} sm={24} /> */}
                        <Col lg={12} md={12} sm={24}>
                          <b>OR</b>
                        </Col>
                        <Col lg={24} md={24} sm={24}>
                          <h3>
                            <FormattedMessage id="form.add.new.owner" />
                          </h3>
                        </Col>
                      </Row>
                      <Row gutter={24}>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.firstName.label" />
                            }
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
                            label={
                              <FormattedMessage id="form.lastName.label" />
                            }
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
                      </Row>
                      <Row gutter={16}>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.address1.label" />
                            }
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
                            label={
                              <FormattedMessage id="form.address2.label" />
                            }
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
                            label={
                              <FormattedMessage id="form.address3.label" />
                            }
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
                            label={
                              <FormattedMessage id="form.mail.placeholder" />
                            }
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
                        <Divider />
                      </Row>
                    </div>
                    <div>
                      <Row gutter={16}>
                        <h2>
                          <FormattedMessage id="form.coOwnerInfo.text" />
                        </h2>
                        <Divider />
                      </Row>
                      <Row>
                        <h3>
                          <FormattedMessage id="form.select.existing.coowner.label" />
                        </h3>
                      </Row>
                      <Row gutter={24}>
                        <Col
                          lg={16}
                          md={16}
                          sm={24}
                          className={styles.searchButton}
                        >
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.existingCoOwner.label" />
                            }
                          >
                            {getFieldDecorator('coOwnerId', {
                              rules: []
                            })(
                              <Select
                                showSearch
                                placeholder={formatMessage({
                                  id: 'form.coOwnerName.placeholder'
                                })}
                                optionFilterProp="children"
                                filterOption={(input, option) =>
                                  option.props.children
                                    .toLowerCase()
                                    .indexOf(input.toLowerCase()) >= 0
                                }
                                onSearch={val => {
                                  this.setState({ coOwnerName: val })
                                }}
                                onBlur={() => this.setState({ coOwnerName: this.state.coOwnerName })}
                                value={this.state.coOwnerName}
                                defaultActiveFirstOption={false}
                                showArrow={false}
                                notFoundContent={null}
                                loading={this.state.isCoOwnerSearching}
                              >
                                {coOwnerOptions}
                              </Select>
                            )}
                          </FormItem>
                          {!this.state.isCoOwnerSearching && (
                            <FormItem>
                              <Button
                                type="primary"
                                icon="search"
                                disabled={this.state.coOwnerName.length < 3}
                                onClick={this.handleSearchCoOwner}
                              // onClick={() => { console.log('this.state.coOwnerName -->', this.state.coOwnerName) }}
                              />
                            </FormItem>
                          )}
                        </Col>
                      </Row>
                      <Row gutter={24}>
                        <Col lg={12} md={12} sm={24} />
                        <Col lg={12} md={12} sm={24}>
                          <b>OR</b>
                        </Col>
                        <Col lg={24} md={24} sm={24}>
                          <h3>
                            <FormattedMessage id="form.add.new.coowner" />
                          </h3>
                        </Col>
                      </Row>
                      <Row gutter={24}>
                        <Col lg={8} md={8} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.firstName.label" />
                            }
                          >
                            {getFieldDecorator('firstName2', {
                              rules: []
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
                            label={
                              <FormattedMessage id="form.lastName.label" />
                            }
                          >
                            {getFieldDecorator('lastName2', {
                              rules: []
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
                              <FormattedMessage id="form.phone-number.label" />
                            }
                          >
                            {getFieldDecorator('phone2', {
                              rules: []
                            })(
                              <Input
                                placeholder={formatMessage({
                                  id: 'form.phone-number.placeholder'
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
                              <FormattedMessage id="form.address1.label" />
                            }
                          >
                            {getFieldDecorator('address12', {
                              rules: []
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
                            label={
                              <FormattedMessage id="form.address2.label" />
                            }
                          >
                            {getFieldDecorator('address22', {
                              rules: []
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
                            label={
                              <FormattedMessage id="form.address3.label" />
                            }
                          >
                            {getFieldDecorator('address32', {
                              rules: []
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
                            {getFieldDecorator('city2', {
                              rules: []
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
                            {getFieldDecorator('state2', {
                              rules: []
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
                            {getFieldDecorator('country2', {
                              rules: []
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
                            {getFieldDecorator('zip2', {
                              rules: []
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
                            {getFieldDecorator('international2', {
                              rules: []
                            })(
                              <Select
                                showSearch
                                placeholder={formatMessage({
                                  id: 'form.international.placeholder'
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
                            label={
                              <FormattedMessage id="form.mail.placeholder" />
                            }
                          >
                            {getFieldDecorator('email2', {
                              rules: [
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
                        <Divider />
                      </Row>
                    </div>
                    <div>
                      <Row gutter={16}>
                        <h2>
                          <FormattedMessage id="form.registrationDetails.text" />
                        </h2>
                        <Divider />
                      </Row>
                      <Row>
                        <Col lg={13} md={13} sm={24}>
                          <FormItem
                            {...formItemLayout}
                            label={
                              <FormattedMessage id="form.type.requested.label" />
                            }
                          >
                            {getFieldDecorator('typeOfRequest', {})(
                              <Select
                                showSearch
                                placeholder={formatMessage({
                                  id: 'form.frozen.semen.placeholder'
                                })}
                                optionFilterProp="children"
                                defaultValue={{ key: 'lucy' }}
                                onChange={this.handleTypeSelect}
                                filterOption={(input, option) =>
                                  option.props.children
                                    .toLowerCase()
                                    .indexOf(input.toLowerCase()) >= 0
                                }
                              >
                                <Option value="none">None</Option>
                                <Option value="isInternational">
                                  International
                                </Option>
                                <Option value="overnightRequested">
                                  Overnight requested
                                </Option>
                                <Option value="rushRequested">
                                  Rush requested
                                </Option>
                              </Select>
                            )}
                          </FormItem>
                        </Col>
                      </Row>
                      <Divider />
                    </div>
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
                                    this.state.buttonType === 'submit'
                                      ? val.required
                                      : false
                                  }
                                  type="file"
                                  onChange={e =>
                                    this.onUploadDocument(e, regId, val.type)
                                  }
                                />
                              }
                              contentHeight={46}
                            />
                          </Col>
                        ))}
                      </Row>
                    </div>
                    <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
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
                        htmlType="button"
                        loading={submitting}
                        onClick={e => this.getButtonId(e, 'later', regId)}
                      >
                        <FormattedMessage id="form.draft.registration" />
                      </Button>
                    </FormItem>
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
      </LocaleProvider>
    );
  }
}

export default PuppyRegistrationForm;