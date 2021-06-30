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
  Anchor,
  Spin,
  Modal
} from 'antd';
import Redirect from 'umi/redirect';
import enGB from 'antd/lib/locale-provider/en_GB';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import styles from './style.less';

import { ChartCard } from '@/components/Charts';

const FormItem = Form.Item;
const { Option } = Select;
const { Link } = Anchor;

let coOwnerNameSearch;

@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  statusStart: form.statusStart,
  colorList: form.colors,
  registration: form.registration,
  startRegistration: form.startRegistration,
  damNameList: form.damNameList,
  sireNameList: form.sireNameList,
  searchDamName: form.searchDamName,
  ownerName: form.ownerName,
  searchOwnerName: form.searchOwnerName,
  ownerDetails: form.ownerDetails,
  spinner: form.spinner,
  editList: form.editList,
  editPedigreeInfo: form.editPedigreeInfo,
  editPedigreeStart: form.editPedigreeStart,
  pedigreePaymentQuote: form.pedigreePaymentQuote,
  submitSuccess: form.submitSuccess,
  submitting: loading.effects['form/submitRegularForm']
}))
@Form.create()
// class PedigreeRegistrationForm extends PureComponent {
class PedigreeRegistrationForm extends React.Component {
  state = {
    showSubmitionFields: false,
    buttonType: 'start',
    currentRegId: '',
    currentDogId: '',
    updated: true,
    damName: '',
    sireName: '',
    ownerName: '',
    coOwnerName: '',
    isOwner: false,
    isCoOwner: false,
    showStartSubmission: true,
    ownerNo: '',
    coOwnerNo: '',
    dogType: '',
    overnightRequested: false,
    rushRequested: false,
    isInternational: false,
    isDamIdSearching: false,
    isSireIdSearching: false,
    isOwnerSearching: false,
    isCoOwnerSearching: false,
    isRegStarting: false,
    isEditing: false,
    searchField: '',
    showSubmitModal: false,
  };

  componentDidMount() {
    const { dispatch, location } = this.props;
    const { isEditing } = this.state;
    dispatch({
      type: 'form/getBreeds'
    });
    dispatch({
      type: 'form/getColors'
    });
    if (location.state && location.state.id) {
      this.setState({ isEditing: true });
      dispatch({
        // type: 'list/getRegInfo',
        type: 'form/getRegInfo',
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
    const { currentUsers } = nextProps.list;
    const { editList } = nextProps;
    const { ownerDetails, location } = nextProps;
    const userRole = currentUsers && currentUsers.abkcRolesUserBelongsTo[1];
    if (nextProps.submitSuccess && (userRole === "Owners" || userRole === "Representatives")) {
      this.setState({ showSubmitModal: true });
    }
    if (!nextProps.spinner) {
      this.setState({
        isDamIdSearching: false,
        isSireIdSearching: false,
        isOwnerSearching: false,
        isCoOwnerSearching: false,
        isRegStarting: false
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
    if (editList && editList.id && updated && location.state && location.state.id && location.state.id === editList.id) {
      this.setState({ updated: false, isEditing: false },
        () => {
          // const list = nextProps.list.editList
          form.setFieldsValue({
            dogName: editList.dogInfo.dogName,
            dateOfBirth: moment(editList.dogInfo.dateOfBirth),
            gender: editList.dogInfo.gender,
            microchipNumber:
              editList.dogInfo.microchipNumber &&
              editList.dogInfo.microchipNumber,
            breedId: editList.dogInfo.breedId,
            colorId: editList.dogInfo.colorId,
            sireId: 0,
            damId: 0,
            lastName: editList.owner && editList.owner.lastName,
            firstName: editList.owner && editList.owner.firstName,
            address1: editList.owner && editList.owner.address1,
            address2: editList.owner && editList.owner.address2,
            address3: editList.owner && editList.owner.address3,
            city: editList.owner && editList.owner.city,
            state: editList.owner && editList.owner.state,
            zip: editList.owner && editList.owner.zip,
            country: editList.owner && editList.owner.country,
            international:
              editList.owner && editList.owner.international.toString(),
            email: editList.owner && editList.owner.email,
            phone: editList.owner && editList.owner.phone,
            lastName2: editList.coOwner && editList.coOwner.lastName,
            firstName2: editList.coOwner && editList.coOwner.firstName,
            address12: editList.coOwner && editList.coOwner.address1,
            address22: editList.coOwner && editList.coOwner.address2,
            address32: editList.coOwner && editList.coOwner.address3,
            city2: editList.coOwner && editList.coOwner.city,
            state2: editList.coOwner && editList.coOwner.state,
            zip2: editList.coOwner && editList.coOwner.zip,
            country2: editList.coOwner && editList.coOwner.country,
            international2:
              editList.coOwner && editList.coOwner.international.toString(),
            email2: editList.coOwner && editList.coOwner.email,
            phone2: editList.coOwner && editList.coOwner.phone,
            typeOfRequest:
              (editList.isInternational && 'isInternational') ||
                (editList.overnightRequested && 'overnightRequested') ||
                editList.rushRequested
                ? 'rushRequested'
                : ''
          });
        }
      );
    }
  }

  handleSubmit = e => {
    const { dispatch, form } = this.props;
    e.preventDefault();
    form.validateFieldsAndScroll((err, values) => {
      if (!err) {
        dispatch({
          type: 'form/submitRegularForm',
          payload: values
        });
      }
    });
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

  getButtonId = (e, id, regId, dogId) => {
    this.setState(
      {
        buttonType: id,
        currentRegId: regId,
        currentDogId: dogId
      },
      () => {
        this.handleSubmitDraft();
      }
    );
  };

  info = (value, currentDogId, currentRegId) => {
    const {
      ownerNo,
      coOwnerNo,
      overnightRequested,
      rushRequested,
      isInternational
    } = this.state;
    if (currentDogId && currentRegId && ownerNo && coOwnerNo) {
      return {
        id: currentRegId,
        dogInfo: {
          id: currentDogId,
          dogName: value.dogName,
          dateOfBirth: moment(value.dateOfBirth).format(),
          gender: value.gender,
          microchipNumber: value.microchipNumber && value.microchipNumber,
          breedId: value.breedId,
          colorId: value.colorId,
          sireId: 0,
          damId: 0
        },
        owner: {
          id: ownerNo,
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
          id: coOwnerNo,
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
        },
        // "submittedBy": {
        //   "id": 0,
        //   "oktaId": "string",
        //   "loginName": "string",
        //   "roles": [
        //     {
        //       "name": "string",
        //       "roleTypeId": value.submittedBy
        //     }
        //   ]
        // },
        dateSubmitted: moment().format(),
        isInternational: isInternational,
        overnightRequested: overnightRequested,
        rushRequested: rushRequested
      };
    }
    if (currentDogId && currentRegId && ownerNo) {
      return {
        id: currentRegId,
        dogInfo: {
          id: currentDogId,
          dogName: value.dogName,
          dateOfBirth: moment(value.dateOfBirth).format(),
          gender: value.gender,
          microchipNumber: value.microchipNumber && value.microchipNumber,
          breedId: value.breedId,
          colorId: value.colorId,
          sireId: 0,
          damId: 0
        },
        owner: {
          id: ownerNo,
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
        // "coOwner": {
        //   "lastName": value.lastName2,
        //   "firstName": value.firstName2,
        //   "address1": value.address12,
        //   "address2": value.address22,
        //   "address3": value.address32,
        //   "city": value.city2,
        //   "state": value.state2,
        //   "zip": value.zip2,
        //   "country": value.country2,
        //   "international": value.international2,
        //   "email": value.email2,
        //   "phone": value.phone2
        // },
        // "submittedBy": {
        //   "id": 0,
        //   "oktaId": "string",
        //   "loginName": "string",
        //   "roles": [
        //     {
        //       "name": "string",
        //       "roleTypeId": value.submittedBy
        //     }
        //   ]
        // },
        dateSubmitted: moment().format(),
        isInternational: isInternational,
        overnightRequested: overnightRequested,
        rushRequested: rushRequested
      };
    }
    if (currentDogId && currentRegId) {
      return {
        id: currentRegId,
        dogInfo: {
          id: currentDogId,
          dogName: value.dogName,
          dateOfBirth: moment(value.dateOfBirth).format(),
          gender: value.gender,
          microchipNumber: value.microchipNumber && value.microchipNumber,
          breedId: value.breedId,
          colorId: value.colorId,
          sireId: 0,
          damId: 0
        },
        // "owner": {
        //   "lastName": value.lastName,
        //   "firstName": value.firstName,
        //   "address1": value.address1,
        //   "address2": value.address2,
        //   "address3": value.address3,
        //   "city": value.city,
        //   "state": value.state,
        //   "zip": value.zip,
        //   "country": value.country,
        //   "international": value.international,
        //   "email": value.email,
        //   "phone": value.phone
        // },
        // "coOwner": {
        //   "lastName": value.lastName2,
        //   "firstName": value.firstName2,
        //   "address1": value.address12,
        //   "address2": value.address22,
        //   "address3": value.address32,
        //   "city": value.city2,
        //   "state": value.state2,
        //   "zip": value.zip2,
        //   "country": value.country2,
        //   "international": value.international2,
        //   "email": value.email2,
        //   "phone": value.phone2
        // },
        // "submittedBy": {
        //   "id": 0,
        //   "oktaId": "string",
        //   "loginName": "string",
        //   "roles": [
        //     {
        //       "name": "string",
        //       "roleTypeId": value.submittedBy
        //     }
        //   ]
        // },
        dateSubmitted: moment().format(),
        isInternational: isInternational,
        overnightRequested: overnightRequested,
        rushRequested: rushRequested
      };
    }
    return {
      dogInfo: {
        dogName: value.dogName,
        dateOfBirth: moment(value.dateOfBirth).format(),
        gender: value.gender,
        microchipNumber: value.microchipNumber && value.microchipNumber,
        breedId: value.breedId,
        colorId: value.colorId,
        sireId: 0,
        damId: 0
      },
      owner: {
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
      },
      // "submittedBy": {
      //   "id": 0,
      //   "oktaId": "string",
      //   "loginName": "string",
      //   "roles": [
      //     {
      //       "name": "string",
      //       "roleTypeId": value.submittedBy
      //     }
      //   ]
      // },
      dateSubmitted: moment().format(),
      isInternational: isInternational,
      overnightRequested: overnightRequested,
      rushRequested: rushRequested
    };
  };

  handleSubmitDraft = () => {
    const { dispatch, form } = this.props;
    const { buttonType, currentRegId, currentDogId } = this.state;
    const role = localStorage.getItem('user-role');
    if (buttonType === 'start') {
      form.validateFields(['dogName'], errors => {
        const value = form.getFieldsValue();
        this.info(value, currentDogId, currentRegId);
        if (!errors) {
          this.setState({
            showStartSubmission: false,
            isRegStarting: true
          });
          dispatch({
            type: 'form/startPedigreeRegistration',
            payload: this.info(value, currentDogId, currentRegId)
          });
        }
      });
    } else if (buttonType === 'draft') {
      form.validateFields(
        [
          'dogName',
          'gender',
          'dateOfBirth',
          'colorId',
          'breedId',
          'damId',
          'sireId',
          'overnightRequested',
          'rushRequested',
          'isInternational',
          'typeOfRequest'
        ],
        errors => {
          const value = form.getFieldsValue();
          this.info(value, currentDogId, currentRegId);
          if (!errors) {
            this.setState({
              showSubmitionFields: true
            });
            dispatch({
              type: 'form/draftRegistrationForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                redirect: false,
                callSubmit: false
              }
            });
          }
        }
      );
    } else if (buttonType === 'submit') {
      form.validateFieldsAndScroll(err => {
        const value = form.getFieldsValue();
        this.info(value, currentDogId, currentRegId);
        if (!err) {
          this.setState({
            showSubmitionFields: true
          });
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitRegistrationForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'pedigree'
              }
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'pedigree'
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'pedigree',
                regArray,
                draftApi: 'draftRegistration'
              }
            });
          }
        }
      });
    } else if (buttonType === 'later') {
      form.validateFields(
        [
          'dogName',
          'gender',
          'dateOfBirth',
          'colorId',
          'breedId',
          'damId',
          'sireId',
          'overnightRequested',
          'rushRequested',
          'isInternational'
        ],
        errors => {
          const value = form.getFieldsValue();
          this.info(value, currentDogId, currentRegId);
          if (!errors) {
            this.setState({
              showSubmitionFields: true
            });
            dispatch({
              type: 'form/draftRegistrationForm',
              payload: this.info(value, currentDogId, currentRegId),
              content: {
                redirect: true,
                callSubmit: false
              }
            });
          }
        }
      );
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
        registrationType: 'pedigree'
      }
    });
  };

  handleSearchDam = val => {
    const { dispatch } = this.props;
    const { damName, sireName } = this.state;
    if (val === 'sire') {
      this.setState({ isSireIdSearching: true, searchField: 'sire', sireName: '' });
      dispatch({
        type: 'form/searchDamName',
        payload: sireName
      });
    } else if (val === 'dam') {
      this.setState({ isDamIdSearching: true, searchField: 'dam', damName: '' });
      dispatch({
        type: 'form/searchDamName',
        payload: damName
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

  handleSearchCoOwner = (x) => {
    console.log("handleSearchCoOwner fn", x)
    const { dispatch } = this.props;
    const { coOwnerName } = this.state;
    // if (coOwnerName) {
    if (x) {
      dispatch({
        type: 'form/searchOwnerName',
        // payload: coOwnerName
        payload: x
      });
    }
    coOwnerNameSearch = ''
    this.setState({ isCoOwnerSearching: true, coOwnerName: '' });
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
        ownerNo: value
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
        coOwnerNo: value
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
      colorList,
      status,
      statusStart,
      registration,
      startRegistration,
      searchDamName,
      searchOwnerName,
      location,
      damNameList,
      sireNameList,
      pedigreePaymentQuote
    } = this.props;
    const {
      showSubmitionFields,
      updated,
      showStartSubmission,
      damName,
      sireName,
      ownerName,
      dogType,
      isRegStarting,
      searchField,
      isEditing,
      showSubmitModal
    } = this.state;
    let optionsSire = [];
    let optionsDam = [];

    if (searchField === 'dam' && damNameList.length && dogType === 'dam') {
      optionsDam = damNameList
        .slice(0, 5)
        .map(d => <Option key={d.originalTableId}>{d.dogName}</Option>);
    } else if (searchField === 'sire' && sireNameList.length && dogType === 'sire') {
      optionsSire = sireNameList
        .slice(0, 5)
        .map(d => <Option key={d.originalTableId}>{d.dogName}</Option>);
    } else {
      optionsSire = [];
      optionsDam = [];
    }

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
    let dogId = '';

    if (statusStart && startRegistration) {
      regId = startRegistration.id;
      dogId = startRegistration.dogInfo.id;
    }

    // if (this.props.list.editPedigreeStart && this.props.list.editPedigreeInfo) {
    //   regId = this.props.list.editPedigreeInfo.id;
    //   dogId = this.props.list.editPedigreeInfo.dogInfo.id;
    // }
    if (this.props.editPedigreeStart && this.props.editPedigreeInfo) {
      regId = this.props.editPedigreeInfo.id;
      dogId = this.props.editPedigreeInfo.dogInfo.id;
    }
    if (status && registration) {
      regId = registration.id;
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

    const { buttonType } = this.state;

    const documents = [
      { type: 'frontPedigree', required: true },
      { type: 'backPedigree', required: false },
      { type: 'frontPhoto', required: true },
      { type: 'sidePhoto', required: true },
      { type: 'ownerSignature', required: true },
      { type: 'coOwnerSignature', required: false }
    ];

    return (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.pedigree.header.text" />}
        >
          <Card bordered={false}>
            <Anchor affix={false}>
              <Link href="/registration-list" title="Back" />
            </Anchor>
            <Spin spinning={isEditing}>
              <Form style={{ marginTop: 8 }}>
                <div>
                  <Row gutter={16}>
                    <h2>
                      <FormattedMessage id="form.dogInfo.text" />
                    </h2>
                    <Divider />
                  </Row>
                  <Spin spinning={this.state.isRegStarting}>
                    <Row gutter={24}>
                      <Col lg={16} md={16} sm={24}>
                        <FormItem
                          {...formLabelItemLayout}
                          label={<FormattedMessage id="form.dogName.label" />}
                        >
                          {getFieldDecorator('dogName', {
                            rules: [
                              {
                                required: true,
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
                            <FormattedMessage id="form.dateOfBirth.label" />
                          }
                        >
                          {getFieldDecorator('dateOfBirth', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.dateOfBirth.required'
                                })
                              }
                            ]
                          })(
                            <DatePicker
                              style={{ width: '100%' }}
                              placeholder={formatMessage({
                                id: 'form.dateOfBirth.placeholder'
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
                          label={<FormattedMessage id="form.gender.label" />}
                        >
                          {getFieldDecorator('gender', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.gender.required'
                                })
                              }
                            ]
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.gender.placeholder'
                              })}
                              optionFilterProp="children"
                              filterOption={(input, option) =>
                                option.props.children
                                  .toLowerCase()
                                  .indexOf(input.toLowerCase()) >= 0
                              }
                            >
                              <Option value="male">Male</Option>
                              <Option value="female">Female</Option>
                            </Select>
                          )}
                        </FormItem>
                      </Col>
                      <Col lg={8} md={8} sm={24}>
                        <FormItem
                          {...formItemLayout}
                          label={<FormattedMessage id="form.color.label" />}
                        >
                          {getFieldDecorator('colorId', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.color.required'
                                })
                              }
                            ]
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
                          label={<FormattedMessage id="form.breed.label" />}
                        >
                          {getFieldDecorator('breedId', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.breed.required'
                                })
                              }
                            ]
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.breed.placeholder'
                              })}
                              optionFilterProp="children"
                              filterOption={(input, option) =>
                                option.props.children
                                  .toLowerCase()
                                  .indexOf(input.toLowerCase()) >= 0
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
                    </Row>
                    <Row gutter={16}>
                      <Col lg={8} md={8} sm={24}>
                        <FormItem
                          {...formItemLayout}
                          label={<FormattedMessage id="form.microChip.label" />}
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
                      <Col lg={8} md={8} sm={8} className={styles.searchButton}>
                        <FormItem
                          {...formItemLayout}
                          label={<FormattedMessage id="form.sireName.label" />}
                        >
                          {getFieldDecorator('sireId', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.sireName.required'
                                })
                              }
                            ]
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({
                                id: 'form.sireName.placeholder'
                              })}
                              optionFilterProp="children"
                              filterOption={(input, option) =>
                                option.props.children
                                  .toLowerCase()
                                  .indexOf(input.toLowerCase()) >= 0
                              }
                              // onSearch={this.handleSearch}
                              onBlur={() => this.setState({ sireName })}
                              onSearch={value =>
                                this.setState({
                                  sireName: value,
                                  dogType: 'sire'
                                })
                              }
                              value={this.state.sireName}
                              defaultActiveFirstOption={false}
                              showArrow={false}
                              notFoundContent={null}
                              loading={this.state.isSireIdSearching}
                            >
                              {optionsSire}
                            </Select>
                          )}
                        </FormItem>
                        {!this.state.isSireIdSearching && (
                          <FormItem>
                            <Button
                              type="primary"
                              icon="search"
                              disabled={sireName.length < 3}
                              onClick={() => this.handleSearchDam('sire')}
                            />
                          </FormItem>
                        )}
                      </Col>
                      <Col lg={8} md={8} sm={8} className={styles.searchButton}>
                        <FormItem
                          {...formItemLayout}
                          label={<FormattedMessage id="form.damName.label" />}
                        >
                          {getFieldDecorator('damId', {
                            rules: [
                              {
                                required: true,
                                message: formatMessage({
                                  id: 'validation.damName.required'
                                })
                              }
                            ]
                          })(
                            <Select
                              showSearch
                              placeholder={formatMessage({ id: 'form.damName.placeholder' })}
                              optionFilterProp="children"
                              onBlur={() => this.setState({ damName })}
                              filterOption={(input, option) =>
                                option.props.children
                                  .toLowerCase()
                                  .indexOf(input.toLowerCase()) >= 0
                              }
                              // onSearch={this.handleSearch}
                              onSearch={value => this.setState({ damName: value, dogType: 'dam' })}
                              value={this.state.damName}
                              defaultActiveFirstOption={false}
                              showArrow={false}
                              notFoundContent={null}
                              loading={this.state.isDamIdSearching}
                            >
                              {optionsDam}
                            </Select>
                          )}
                        </FormItem>
                        {!this.state.isDamIdSearching && (
                          <FormItem>
                            <Button
                              type="primary"
                              icon="search"
                              disabled={damName.length < 3}
                              onClick={() => this.handleSearchDam('dam')}
                            />
                          </FormItem>
                        )}
                      </Col>
                      {!statusStart && showStartSubmission && updated && (
                        <FormItem
                          {...draftFormLayout}
                          style={{ marginTop: 32 }}
                        >
                          <Button
                            type="primary"
                            id="start"
                            style={{ marginLeft: 8 }}
                            htmlType="submit"
                            loading={submitting}
                            onClick={e => this.getButtonId(e, 'start')}
                          >
                            <FormattedMessage id="form.start.registration" />
                          </Button>
                        </FormItem>
                      )}
                      <Divider />
                    </Row>
                  </Spin>
                </div>
                {/* {statusStart || !updated ? ( */}
                {true ? (
                  <div>
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
                                placeholder={formatMessage({ id: 'form.ownerName.placeholder' })}
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
                            {getFieldDecorator('address3', {
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
                                // onBlur={() => { console.log('coOwnerNameSearch -->', coOwnerNameSearch); }}
                                // onBlur={() => this.setState({ coOwnerName: this.state.coOwnerName })}
                                // onBlur={() => console.log('this.state.coOwnerName -->', this.state.coOwnerName)}
                                onSearch={val => {
                                  this.setState({ coOwnerName: val })
                                  coOwnerNameSearch = val;
                                }}
                                onChange={this.handleChange}
                                // value={this.state.coOwnerName}
                                value={coOwnerNameSearch}
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
                                // disabled={this.state.coOwnerName.length < 3}
                                // disabled={coOwnerNameSearch.length < 3}
                                // onClick={() => this.handleSearchCoOwner(coOwnerNameSearch)}
                                onClick={() => { console.log('coOwnerNameSearch -->', coOwnerNameSearch) }}
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
                            {getFieldDecorator('typeOfRequest', {
                              rules: [
                                {
                                  required: true,
                                  message: formatMessage({
                                    id: 'validation.dropdown.required'
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
                    {!status && !showSubmitionFields && updated && (
                      <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
                        <Button
                          type="primary"
                          id="draft"
                          style={{ marginLeft: 8 }}
                          htmlType="button"
                          loading={submitting}
                          onClick={e =>
                            this.getButtonId(e, 'draft', regId, dogId)
                          }
                        >
                          <FormattedMessage id="form.draft.registration" />
                        </Button>
                      </FormItem>
                    )}
                  </div>
                ) : (
                    ''
                  )}

                {(status && showSubmitionFields) || !updated ? (
                  <div>
                    <div>
                      <Row gutter={16}>
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
                                        buttonType === 'submit'
                                          ? val.required
                                          : false
                                      }
                                      type="file"
                                      onChange={e =>
                                        this.onUploadDocument(
                                          e,
                                          regId,
                                          val.type
                                        )
                                      }
                                    />
                                  }
                                  contentHeight={46}
                                />
                              </Col>
                            ))}
                          </Row>
                        </div>
                        <Modal
                          title="Update Registration Fee"
                          className={styles.updateFeeModal}
                          visible={showSubmitModal}
                          onOk={this.handleOk}
                          onCancel={this.handleCancel}
                          footer={[
                            // <Button key="submit" type="primary" onClick={() => { this.setState({ showSubmitModal: false }, () => <Redirect to="/registration-list" />) }}>
                            <Button key="submit" type="primary" onClick={this.handleOk}>
                              <FormattedMessage id="menu.btn.ok" />
                            </Button>
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
                        {/* {!statusStart && showStartSubmission && updated &&
                        <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
                          <Button type="primary" id="start" style={{ marginLeft: 8 }} htmlType="submit" loading={submitting} onClick={e => this.getButtonId(e, "start")}>
                            <FormattedMessage id="form.start.registration" />
                          </Button>
                        </FormItem>
                      } */}
                        <Divider />
                      </Row>
                    </div>
                    <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
                      <Button
                        type="primary"
                        id="submit"
                        htmlType="button"
                        loading={submitting}
                        onClick={e =>
                          this.getButtonId(e, 'submit', regId, dogId)
                        }
                      >
                        <FormattedMessage id="form.submit.now" />
                      </Button>
                      <Button
                        type="primary"
                        id="later"
                        style={{ marginLeft: 8 }}
                        htmlType="button"
                        loading={submitting}
                        onClick={e =>
                          this.getButtonId(e, 'later', regId, dogId)
                        }
                      >
                        <FormattedMessage id="form.draft.registration" />
                      </Button>
                    </FormItem>
                  </div>
                ) : null}
              </Form>
            </Spin>
          </Card>
        </PageHeaderWrapper>
      </LocaleProvider >
    );
  }
}

export default PedigreeRegistrationForm;