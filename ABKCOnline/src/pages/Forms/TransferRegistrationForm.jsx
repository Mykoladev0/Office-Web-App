/* eslint-disable jsx-a11y/label-has-associated-control */
/* eslint-disable jsx-a11y/label-has-for */
import React, { PureComponent } from 'react';
import { routerRedux } from 'dva/router';
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

const { Link } = Anchor;
const FormItem = Form.Item;
const { Option } = Select;

@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  statusStart: form.statusStart,
  colorList: form.colors,
  registration: form.registration,
  startRegistration: form.startRegistration,
  startTransferRegistration: form.startTransferRegistration,
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
class TransferRegistrationForm extends PureComponent {
  state = {
    buttonType: 'start',
    currentRegId: '',
    currentDogId: '',
    updated: true,
    ownerName: '',
    coOwnerName: '',
    isOwner: true,
    isCoOwner: true,
    isDogInfo: true,
    OwnerId: '',
    coOwnerId: '',
    showStartSubmission: true,
    overnightRequested: false,
    rushRequested: false,
    isInternational: false,
    isSearchingAbkc: false,
    isOwnerSearching: false,
    isCoOwnerSearching: false,
    isEditing: false,
    showSubmitModal: false,
  };

  componentDidMount() {
    const { dispatch, location } = this.props;
    dispatch({
      type: 'form/getBreeds'
    });
    dispatch({
      type: 'form/getColors'
    });
    if (location.state && location.state.id) {
      this.setState({ isEditing: true });
      dispatch({
        type: 'list/getTransferRegById',
        payload: {
          id: location.state.id
        }
      });
    }
  }

  componentWillReceiveProps(nextProps) {
    const { updated, isOwner, isCoOwner, isDogInfo } = this.state;
    const { form } = this.props;
    const { editTransferRegList } = nextProps.list;
    const { ownerDetails, startTransferRegistration, location } = nextProps;
    const { currentUsers } = nextProps.list;
    const userRole = currentUsers && currentUsers.abkcRolesUserBelongsTo[1];
    if (nextProps.submitSuccess && (userRole === "Owners" || userRole === "Representatives")) {
      this.setState({ showSubmitModal: true });
    }
    if (!nextProps.spinner) {
      this.setState({
        isSearchingAbkc: false,
        isOwnerSearching: false,
        isCoOwnerSearching: false
      });
    }
    if (editTransferRegList && editTransferRegList.newOwner && editTransferRegList.newOwner.id) {
      this.setState({ isNewOwnerExist: true });
    }
    if (editTransferRegList && editTransferRegList.newCoOwner && editTransferRegList.newCoOwner.id) {
      this.setState({ isNewCoOwnerExist: true });
    }
    if (!!startTransferRegistration && startTransferRegistration.dogInfo && startTransferRegistration.dogInfo.id && isDogInfo) {
      form.setFieldsValue({}, () => {
        form.setFieldsValue({
          dogName: startTransferRegistration.dogInfo.dogName,
          sellDate: startTransferRegistration.sellDate
            ? moment(startTransferRegistration.sellDate)
            : '',
          colorId: startTransferRegistration.dogInfo.color,
          microchipNumber: startTransferRegistration.dogInfo.microchipNumber,
          microchipType: startTransferRegistration.dogInfo.microchipType
        });
      });
      this.setState({
        isDogInfo: false
      });
    }
    if (!!startTransferRegistration && startTransferRegistration.newOwner && startTransferRegistration.newOwner.id && isOwner) {
      form.setFieldsValue({
        id: startTransferRegistration.newOwner.id,
        lastName:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.lastName,
        firstName:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.firstName,
        address1:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.address1,
        address2:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.address2,
        address3:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.address3,
        city:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.city,
        state:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.state,
        zip:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.zip,
        country:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.country,
        international:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.international,
        email:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.email,
        phone:
          startTransferRegistration.newOwner &&
          startTransferRegistration.newOwner.phone
      });
      this.setState({
        isOwner: false
      });
    }
    if (!!startTransferRegistration && startTransferRegistration.newCoOwner && startTransferRegistration.newCoOwner.id && isCoOwner) {
      form.setFieldsValue({
        id: startTransferRegistration.newCoOwner.id,
        lastName2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.lastName,
        firstName2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.firstName,
        address12:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.address1,
        address22:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.address2,
        address32:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.address3,
        city2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.city,
        state2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.state,
        zip2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.zip,
        country2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.country,
        international2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.international,
        email2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.email,
        phone2:
          startTransferRegistration.newCoOwner &&
          startTransferRegistration.newCoOwner.phone
      });
      this.setState({
        isCoOwner: false
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
        isCoOwner: false,
        OwnerId: ownerDetails && ownerDetails.lastName
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
    if (editTransferRegList && editTransferRegList.id && updated && location.state && location.state.id) {
      this.setState(
        {
          updated: false,
          isEditing: false,
          OwnerId:
            editTransferRegList.newOwner && editTransferRegList.newOwner.id,
          coOwnerId:
            editTransferRegList.newCoOwner && editTransferRegList.newCoOwner.id
        },
        () => {
          form.setFieldsValue({
            dogName:
              editTransferRegList.dogInfo.dogName &&
              editTransferRegList.dogInfo.dogName,
            sellDate: editTransferRegList.dogInfo.sellDate
              ? moment(editTransferRegList.dogInfo.sellDate)
              : '',
            gender:
              editTransferRegList.dogInfo.gender &&
              editTransferRegList.dogInfo.gender,
            microchipNumber:
              editTransferRegList.dogInfo.microchipNumber &&
              editTransferRegList.dogInfo.microchipNumber,
            microchipType:
              editTransferRegList.dogInfo.microchipType &&
              editTransferRegList.dogInfo.microchipType,
            colorId:
              editTransferRegList.dogInfo.colorId &&
              editTransferRegList.dogInfo.colorId,
            OwnerId:
              editTransferRegList.newOwner && editTransferRegList.newOwner.id,
            lastName:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.lastName,
            firstName:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.firstName,
            address1:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.address1,
            address2:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.address2,
            address3:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.address3,
            city:
              editTransferRegList.newOwner && editTransferRegList.newOwner.city,
            state:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.state,
            zip:
              editTransferRegList.newOwner && editTransferRegList.newOwner.zip,
            country:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.country,
            international:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.international.toString(),
            email:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.email,
            phone:
              editTransferRegList.newOwner &&
              editTransferRegList.newOwner.phone,
            coOwnerId:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.id,
            lastName2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.lastName,
            firstName2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.firstName,
            address12:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.address1,
            address22:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.address2,
            address32:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.address3,
            city2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.city,
            state2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.state,
            zip2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.zip,
            country2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.country,
            international2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.international.toString(),
            email2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.email,
            phone2:
              editTransferRegList.newCoOwner &&
              editTransferRegList.newCoOwner.phone,
            typeOfRequest:
              (editTransferRegList.isInternational && 'isInternational') ||
                (editTransferRegList.overnightRequested &&
                  'overnightRequested') ||
                editTransferRegList.rushRequested
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
        // "id": OwnerId,
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
        // "id": coOwnerId,
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

  onUploadDocument = (e, id, val) => {
    const { dispatch } = this.props;
    const fileset = e.target.files;
    dispatch({
      type: 'form/submitSupportinngDoc',
      payload: {
        id,
        file: fileset[0],
        documentType: val,
        registrationType: 'transfer'
      }
    });
  };

  handleDataChange = val => {
    if (val === 'owner') {
      this.setState({ isNewOwnerExist: false });
    } else if (val === 'coOwner') {
      this.setState({ isNewCoOwnerExist: false });
    }
  };

  handleSubmitDraft = e => {
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
            isSearchingAbkc: true
          });
          dispatch({
            type: 'form/getDogsByABKCNumber',
            payload: { abkcNumber: value.abkcNumber }
          });
        }
      });
    } else if (buttonType === 'later') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, `currentDogId`, currentRegId);
        if (!err) {
          dispatch({
            type: 'form/draftTransferRegistrationForm',
            payload: this.info(value, currentDogId, currentRegId),
            content: {
              currentRegId,
              redirect: true,
              callSubmit: true,
              registrationType: 'transfer'
            }
          });
        }
      });
    } else if (buttonType === 'submit') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, `currentDogId`, currentRegId);
        if (!err) {
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitTransferRegistrationForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'transfer'
              }
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'transfer'
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'transfer',
                regArray,
                draftApi: 'draftTransferRegistration'
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
    this.setState({ isOwnerSearching: true });
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
    this.setState({ isCoOwnerSearching: true });
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

  changeViewMoreStateBack = () => {
    routerRedux.replace('/registration-list');
  };

  handleCancel = () => {
    this.setState({ showSubmitModal: false });
  };

  disabledDate = current => current && current > moment().endOf('day');

  render() {
    const {
      submitting,
      colorList,
      statusStart,
      startTransferRegistration,
      searchOwnerName,
      location,
      pedigreePaymentQuote
    } = this.props;
    const { updated, showStartSubmission, ownerName, isEditing, showSubmitModal } = this.state;

    let ownerOptions = [];
    if (searchOwnerName === ownerName && this.props.ownerName.length) {
      ownerOptions = this.props.ownerName.slice(0, 5).map(d => {
        return <Option key={d.id}>{d.fullName}</Option>;
      });
    } else {
      ownerOptions = [];
    }

    let coOwnerOptions = [];
    if (this.props.ownerName.length) {
      coOwnerOptions = this.props.ownerName.slice(0, 5).map(d => {
        return <Option key={d.id}>{d.fullName}</Option>;
      });
    } else {
      coOwnerOptions = [];
    }

    let regId = '';
    if (statusStart && startTransferRegistration) {
      regId = startTransferRegistration.id;
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
      { type: 'billOfSaleFront', required: true },
      { type: 'billOfSaleBack', required: true }
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
          title={<FormattedMessage id="form.transfer.header.text" />}
        >
          <Card bordered={false}>
            <Anchor affix={false}>
              <Link href="/registration-list" title="Back" />
            </Anchor>
            <Spin spinning={isEditing}>
              <Form style={{ marginTop: 8 }}>
                {!statusStart && showStartSubmission && updated && (
                  <div>
                    <Row gutter={16}>
                      <h2>
                        <FormattedMessage id="form.dogInfo.text" />
                      </h2>
                      <Divider />
                    </Row>
                    <Spin spinning={this.state.isSearchingAbkc}>
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
                    </Spin>
                    <Row gutter={16}>
                      {!statusStart && showStartSubmission && updated && (
                        <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
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
                )}
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
                              rules: [
                                {
                                  required: true,
                                  message: formatMessage({
                                    id: 'validation.sellDate.required'
                                  })
                                }
                              ]
                            })(
                              <DatePicker
                                style={{ width: '100%' }}
                                placeholder={formatMessage({
                                  id: 'form.sellDate.placeholder'
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
                            label={<FormattedMessage id="form.color.label" />}
                          >
                            {getFieldDecorator('colorId', {
                              // rules: [
                              //   {
                              //     // required: true,
                              //     // message: formatMessage({ id: 'validation.color.required' }),
                              //   },
                              // ],
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
                              // rules: [
                              // ],
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
                              // rules: [
                              // ],
                            })(
                              <Input
                                placeholder={formatMessage({
                                  id: 'form.microchipType.placeholder'
                                })}
                              />
                            )}
                          </FormItem>
                        </Col>
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
                                onSearch={val => this.setState({ ownerName: val })}
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
                        <Col lg={12} md={12} sm={24} />
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
                              // rules: [
                              // ],
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
                                onBlur={() => this.setState({ coOwnerName: this.state.coOwnerName })}
                                onSearch={val =>
                                  this.setState({ coOwnerName: val })
                                }
                                onChange={this.handleCoOwnerChange}
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
                              // rules: [
                              // ],
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
                              // rules: [
                              // ],
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
                              // rules: [
                              // ],
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
                              // rules: [
                              // ],
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
                              // rules: [],
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
                              // rules: [],
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
                              // rules: [],
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
                              // rules: [],
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
                              // rules: [],
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
                              // rules: [],
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
                              // rules: [],
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
                            {getFieldDecorator('typeOfRequest', {
                              // rules: [
                              //   {
                              //     // required: true,
                              //     message: formatMessage({ id: 'validation.dropdown.required' }),
                              //   },
                              // ],
                            })(
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
                      <Divider />
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
                        style={{ marginLeft: 8 }}
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

export default TransferRegistrationForm;