import React, { PureComponent } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import Highlighter from 'react-highlight-words';
import { saveAs } from 'file-saver';
import locale from 'antd/lib/date-picker/locale/en_US';
import { formatMessage, FormattedMessage } from 'umi/locale';
import {
  Form,
  Tabs,
  LocaleProvider,
  Button,
  Icon,
  Table,
  Divider,
  Input,
  Modal,
  Select,
  Row,
  Col,
} from 'antd';
import { Link } from 'react-router-dom';
import enGB from 'antd/lib/locale-provider/en_GB';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import ProgressDialog from '@/components/Dialogs/ProgressDialog';
import SubmitRegistrationDialog from '@/components/Dialogs/SubmitRegistrationDialog/SubmitRegistrationDialog';

import { generatePedigreePDF } from '@/services/pedigreeApi';
import { generateLitterReport } from '@/services/litterApi';
import { generatePuppyCertificatePDF } from '@/services/puppyApi';
import { generateJuniorHandlerCertificate } from '@/services/juniorHandlersApi';
import styles from './style.less';

moment.locale('en');
const { TabPane } = Tabs;
const { Option } = Select;
const { confirm } = Modal;


@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  colorList: form.colors,
  registration: form.registration,
  submitting: loading.effects['form/submitRegularForm'],
}))
@Form.create()
class RegistrationList extends PureComponent {
  state = {
    visible: false,
    modalHeader: '',
    regId: '',
    commentValue: '',
    type: '',
    showError: false,
    updated: true,
    show: false,
    generatingDocument: false,
    showGeneratingDocDialog: false,
    showSubmitDialog: false,
    visibleDelete: false,
    record: ''
  };

  componentDidMount() {
    this.setState({ call: !this.state.call });
  }

  componentDidUpdate(prevProps, prevState) {
    const { updated } = this.state;
    const { dispatch, list } = this.props;
    const role = localStorage.getItem('user-role');
    if (updated && list.currentUsers && list.currentUsers.id) {
      // eslint-disable-next-line react/no-did-update-set-state
      this.setState({
        updated: false,
      });
      if (role === 'ABKCOffice' || role === 'Administrator' || role === 'Administrators') {
        // alert()
        dispatch({
          type: 'list/fetchData',
        });
        dispatch({
          type: 'list/filterData',
          payload: 'Pending',
        });
      } else if (role === 'Representatives' || role === 'Owners') {
        dispatch({
          type: 'list/filterRegById',
          payload: list.currentUsers.id,
        });
        // dispatch({
        //   type: 'list/filterPendingById',
        //   payload: list.currentUsers.id,
        // });
      }
    }
  }

  getColumnSearchProps = dataIndex => ({
    filterDropdown: ({ setSelectedKeys, selectedKeys, confirm, clearFilters }) => (
      <div style={{ padding: 8 }}>
        <Input
          ref={node => {
            this.searchInput = node;
          }}
          placeholder={`Search ${dataIndex}`}
          value={selectedKeys[0]}
          onChange={e => setSelectedKeys(e.target.value ? [e.target.value] : [])}
          onPressEnter={() => this.handleSearch(selectedKeys, confirm)}
          style={{ width: 188, marginBottom: 8, display: 'block' }}
        />
        <Button
          type="primary"
          onClick={() => this.handleSearch(selectedKeys, confirm)}
          icon="search"
          size="small"
          style={{ width: 90, marginRight: 8 }}
        >
          Search
        </Button>
        <Button onClick={() => this.handleReset(clearFilters)} size="small" style={{ width: 90 }}>
          Reset
        </Button>
      </div>
    ),
    filterIcon: filtered => (
      <Icon type="search" style={{ color: filtered ? '#1890ff' : undefined }} />
    ),
    onFilter: (value, record) =>
      record[dataIndex]
        .toString()
        .toLowerCase()
        .includes(value.toLowerCase()),
    onFilterDropdownVisibleChange: visible => {
      if (visible) {
        setTimeout(() => this.searchInput.select());
      }
    },
    render: text => (
      <Highlighter
        highlightStyle={{ backgroundColor: '#ffc069', padding: 0 }}
        searchWords={[this.state.searchText]}
        autoEscape
        textToHighlight={text ? text.toString() : text}
      />
    ),
  });

  handleSearch = (selectedKeys, confirm) => {
    confirm();
    this.setState({ searchText: selectedKeys[0] });
  };

  handleReset = clearFilters => {
    clearFilters();
    this.setState({ searchText: '' });
  };

  fnDeletePedigreeRegistration = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deletePedigree',
      payload: id,
    });
  };

  fnDeleteLitterRegistration = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deleteLitter',
      payload: id,
    });
  };

  fnDeleteJuniorHandlerRegistration = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deleteJuniorHandler',
      payload: id,
    });
  };

  fnDeletePuppyRegistration = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deletePuppy',
      payload: id,
    });
  };

  fnDeleteTransferRegistration = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deleteTransferRegistration',
      payload: id,
    });
  };

  fnDeleteBullyRequest = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/deleteBullyRequest',
      payload: id,
    });
  };

  showModal = (val, record) => {
    this.setState({
      visible: true,
      modalHeader: val,
      regId: record.id,
      type: record.registrationType,
      commentValue: '',
      showError: false,
    });
  };

  handleSubmit = () => {
    const { dispatch } = this.props;
    const { commentValue, regId, type, modalHeader } = this.state;

    if (commentValue === '' && modalHeader === 'Reject') {
      this.setState({
        showError: true,
        visible: true,
      });
    } else {
      this.setState({
        visible: false,
        modalHeader: '',
        commentValue: '',
      });
    }
    if (modalHeader === 'Approve') {
      dispatch({
        type: 'list/approveRegistration',
        payload: { val: commentValue, id: regId, type },
      });
    } else if (commentValue && modalHeader === 'Reject') {
      dispatch({
        type: 'list/rejectRegistration',
        payload: { val: commentValue, id: regId, type },
      });
    }
  };

  handleCancel = () => {
    this.setState({
      visible: false,
      modalHeader: '',
      visibleDelete: false
    });
  };

  handleChange = e => {
    const val = e.target.value;
    this.setState({
      commentValue: val,
    });
  };

  canGenerateCert = (record, userRole) => {
    if (userRole === 'Administrators' || userRole === 'ABKCOffice') {
      switch (record.registrationType) {
        case 'Pedigree':
        case 'Litter':
        case 'Puppy':
        case 'JuniorHandler':
          return true;
        default:
          return false;
      }
    }
    return false;
  };

  generateFinalDocument = async record => {
    this.setState({ generatingDocument: true, showGeneratingDocDialog: true });
    let doc = null;
    switch (record.registrationType) {
      case 'Pedigree':
        doc = await generatePedigreePDF(record.id);
        break;
      case 'Litter':
        doc = await generateLitterReport(record.id);
        break;
      case 'Puppy':
        doc = await generatePuppyCertificatePDF(record.id);
        break;
      case 'Transfer':
        // doc = await generatePuppyCertificatePDF(record.id);
        break;
      case 'JuniorHandler':
        doc = await generateJuniorHandlerCertificate(record.id);
        break;
      case 'BullyId':
        // alert user that this type is not supported
        break;
      default:
        break;
    }
    if (doc) {
      // const blob = new Blob(['Hello, world!'], { type: 'text/plain;charset=utf-8' });
      saveAs(doc.fileUrl, doc.fileName);
    }
    this.setState({ generatingDocument: false });
  };

  showDeleteConfirm = (e, record) => {
    e.preventDefault();
    this.setState({
      visibleDelete: true,
      record
    })
  }

  handleDeleteOk = () => {
    const { record } = this.state;
    if (record.registrationType === 'Litter') {
      this.fnDeleteLitterRegistration(record.id);
    } else if (record.registrationType === 'Pedigree') {
      this.fnDeletePedigreeRegistration(record.id);
    } else if (record.registrationType === 'JuniorHandler') {
      this.fnDeleteJuniorHandlerRegistration(record.id);
    } else if (record.registrationType === 'Puppy') {
      this.fnDeletePuppyRegistration(record.id);
    } else if (record.registrationType === 'Transfer') {
      this.fnDeleteTransferRegistration(record.id);
    } else if (record.registrationType === 'BullyId') {
      this.fnDeleteBullyRequest(record.id);
    }
    this.setState({
      visibleDelete: false
    })
  }

  getRegistrationPath = registrationType => {
    switch (registrationType) {
      case 'Litter':
        return '/litter-registration-form';
      case 'Pedigree':
        return '/pedigree-registration-form';
      case 'JuniorHandler':
        return '/junior-handlers-registration-form';
      case 'Puppy':
        return '/puppy-registration-form';
      case 'Transfer':
        return '/transfer-registration-form';
      case 'Bully':
        return '/bully-request-form';
      default:
        return '/registration-list';
    }
  };

  getColumns = (id, role) => {
    return [
      {
        title: formatMessage({ id: 'menu.table.header.name' }),
        dataIndex: 'name',
        key: 'name',
        sorter: (a, b) => a.name.localeCompare(b.name),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('name'),
      },
      {
        title: formatMessage({ id: 'menu.table.header.dob' }),
        dataIndex: 'dob',
        key: 'dob',
        sorter: (a, b) => a.dob.localeCompare(b.dob),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('dob'),
      },
      {
        title: formatMessage({ id: 'menu.table.header.breed' }),
        dataIndex: 'breed',
        key: 'breed',
        sorter: (a, b) => a.breed.localeCompare(b.breed),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('breed'),
      },
      {
        title: formatMessage({ id: 'menu.table.header.gender' }),
        key: 'gender',
        dataIndex: 'gender',
        sorter: (a, b) => a.gender.localeCompare(b.gender),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('gender'),
      },
      {
        title: formatMessage({ id: 'menu.table.header.type' }),
        key: 'registrationType',
        dataIndex: 'registrationType',
        sorter: (a, b) => a.registrationType.localeCompare(b.registrationType),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('registrationType'),
      },
      {
        title: formatMessage({ id: 'menu.table.header.action' }),
        key: 'action',
        render: (text, record) => (
          <span>
            {id === 1 && (
              <>
                <Link
                  to={{
                    pathname: this.getRegistrationPath(record.registrationType),
                    state: { id: record.id },
                  }}
                >
                  <FormattedMessage id="menu.table.edit" />
                </Link>
                <Divider type="vertical" />
                <Link to='' onClick={e => this.showDeleteConfirm(e, record)}>
                  <FormattedMessage id="menu.table.delete" />
                </Link>
              </>
            )}
            {id === 2 &&
              (role === 'ABKCOffice' || role === 'Administrator' || role === 'Administrators') && (
                <>
                  <Link to="/registration-list" onClick={e => this.showModal('Approve', record)}>
                    <FormattedMessage id="menu.table.approve" />
                  </Link>
                  <Divider type="vertical" />
                  <Link to="/registration-list" onClick={e => this.showModal('Reject', record)}>
                    <FormattedMessage id="menu.table.reject" />
                  </Link>
                </>
              )}
            {id === 3 && this.canGenerateCert(record, role) && (
              <Button
                icon="cloud-download"
                shape="round"
                size="small"
                onClick={async () => {
                  await this.generateFinalDocument(record);
                }}
              >
                <FormattedMessage id="menu.table.certificate" />
              </Button>
            )}
          </span>
        ),
      },
    ];
  };

  operations = () => {
    return (
      <div style={{ width: '40%' }}>
        <Row style={{ display: '-webkit-inline-box' }}>
          <Col lg={12} md={12} sm={24}>
            <p>
              {' '}
              <Icon type="plus" />
              <FormattedMessage id="menu.add.registration" />
            </p>
          </Col>
          <Col lg={8} md={8} sm={24}>
            <Select
              placeholder={formatMessage({ id: 'menu.add.registration.placeholder' })}
              optionFilterProp="children"
              style={{ width: '160px', marginRight: '17px' }}
              filterOption={(input, option) =>
                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
              }
            >
              <Option value="pedigree">
                <Link to={{ pathname: '/pedigree-registration-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.pedigree" />
                </Link>
              </Option>
              <Option value="litter">
                <Link to={{ pathname: '/litter-registration-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.litter" />
                </Link>
              </Option>
              <Option value="puppy">
                <Link to={{ pathname: '/puppy-registration-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.puppy" />
                </Link>
              </Option>
              <Option value="jr-handler">
                <Link to={{ pathname: '/junior-handlers-registration-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.junior" />
                </Link>
              </Option>
              <Option value="transfer-registration">
                <Link to={{ pathname: '/transfer-registration-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.transfer" />
                </Link>
              </Option>
              <Option value="bully-request">
                <Link to={{ pathname: '/bully-request-form' }}>
                  {' '}
                  <FormattedMessage id="menu.add.registration.bully" />
                </Link>
              </Option>
            </Select>
          </Col>
        </Row>
      </div>
    );
  };

  render() {
    const { list } = this.props;
    const listData = list.filters;
    const {
      visible,
      visibleDelete,
      modalHeader,
      commentValue,
      showError,
      generatingDocument,
      showGeneratingDocDialog,
      showSubmitDialog,
    } = this.state;
    // console.log('listData', listData);
    const filterPendingRes = listData.filter(scenario =>
      scenario.registrationStatus.match('Pending')
    );

    const filterDraftRes = listData.filter(scenario => scenario.registrationStatus.match('Draft'));

    const filterApprovedRes = listData.filter(scenario =>
      scenario.registrationStatus.match('Approved')
    );

    const filterRejectedRes = listData.filter(scenario =>
      scenario.registrationStatus.match('Denied')
    );

    const role = localStorage.getItem('user-role');
    const draftRegistrations = [];
    filterDraftRes.map((val, key) =>
      draftRegistrations.push({
        key,
        id: val.id,
        name: val.dogInfo && val.dogInfo.dogName,
        dob: val.dogInfo && moment(val.dogInfo.dateOfBirth).format('MMMM Do YYYY'),
        breed: val.dogInfo && val.dogInfo.breed,
        gender: val.dogInfo && val.dogInfo.gender,
        registrationType: val.registrationType,
      })
    );

    const pendingRegistrations = [];
    filterPendingRes.map((val, key) =>
      pendingRegistrations.push({
        key,
        id: val.id,
        name: val.dogInfo && val.dogInfo.dogName,
        dob: val.dogInfo && moment(val.dogInfo.dateOfBirth).format('MMMM Do YYYY'),
        breed: val.dogInfo && val.dogInfo.breed,
        gender: val.dogInfo && val.dogInfo.gender,
        registrationType: val.registrationType,
      })
    );

    const approvedRegistrations = [];
    filterApprovedRes.map((val, key) =>
      approvedRegistrations.push({
        key,
        id: val.id,
        name: val.dogInfo && val.dogInfo.dogName,
        dob: val.dogInfo && moment(val.dogInfo.dateOfBirth).format('MMMM Do YYYY'),
        breed: val.dogInfo && val.dogInfo.breed,
        gender: val.dogInfo && val.dogInfo.gender,
        registrationType: val.registrationType,
      })
    );

    const rejectedRegistrations = [];
    filterRejectedRes.map((val, key) =>
      rejectedRegistrations.push({
        key,
        id: val.id,
        name: val.dogInfo && val.dogInfo.dogName,
        dob: val.dogInfo && moment(val.dogInfo.dateOfBirth).format('MMMM Do YYYY'),
        breed: val.dogInfo && val.dogInfo.breed,
        gender: val.dogInfo && val.dogInfo.gender,
        registrationType: val.registrationType,
      })
    );

    return (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.dog.registration.list" />}
        >
          <Tabs tabBarExtraContent={this.operations()}>
            <TabPane tab={formatMessage({ id: 'menu.header.draft' })} key="1">
              <Table
                columns={this.getColumns(1, role)}
                dataSource={draftRegistrations}
                footer={() => (
                  <Button
                    disabled={draftRegistrations.length === 0}
                    onClick={() => this.setState({ showSubmitDialog: true })}
                  >
                    Submit Drafts
                  </Button>
                )}
              />
            </TabPane>
            <TabPane tab={formatMessage({ id: 'menu.header.pending' })} key="2">
              <Table columns={this.getColumns(2, role)} dataSource={pendingRegistrations} />
            </TabPane>
            <TabPane tab={formatMessage({ id: 'menu.header.approved' })} key="3">
              <Table columns={this.getColumns(3, role)} dataSource={approvedRegistrations} />
            </TabPane>
            <TabPane tab={formatMessage({ id: 'menu.header.rejected' })} key="4">
              <Table columns={this.getColumns(4, role)} dataSource={rejectedRegistrations} />
            </TabPane>
          </Tabs>
          <Modal
            title={`${modalHeader} Registration`}
            visible={visible}
            onOk={this.handleSubmit}
            onCancel={this.handleCancel}
            footer={[
              <Button key="submit" type="default" onClick={this.handleCancel}>
                <FormattedMessage id="menu.btn.cancel" />
              </Button>,
              <Button key="submit" type="primary" onClick={this.handleSubmit}>
                <FormattedMessage id="menu.btn.save" />
              </Button>,
            ]}
          >
            <Input
              placeholder={formatMessage({ id: 'menu.modal.reason.placeholder' })}
              value={commentValue}
              onChange={e => this.handleChange(e)}
            />
            {showError && (
              <p className="show-error" style={{ color: '#f5222d' }}>
                <FormattedMessage id="menu.reason.for.rejection" />
              </p>
            )}
          </Modal>
          <Modal
            visible={visibleDelete}
            onOk={this.handleDeleteOk}
            onCancel={this.handleCancel}
            okText='Yes'
            okType='danger'
            cancelText='No'
          >
            <h2 className={styles.confirmDelete}><Icon type="question-circle" /> Are you sure delete this registration?</h2>
            <p>This action is permanent.Cannot be undone.</p>
          </Modal>
          <ProgressDialog
            title="Generating Certificate"
            caption="Please wait while your document is generated"
            isProcessing={generatingDocument}
            showDialog={showGeneratingDocDialog}
            handleCloseFn={() => this.setState({ showGeneratingDocDialog: false })}
          />
          {showSubmitDialog && (
            <SubmitRegistrationDialog
              draftRegistrations={draftRegistrations}
              handleCloseFn={isComplete => {
                // console.log(isComplete);
                if (isComplete) {
                  // TODO:update draft list
                }
                this.setState({ showSubmitDialog: false });
              }}
            />
          )}
        </PageHeaderWrapper>
      </LocaleProvider >
    );
  }
}

export default RegistrationList;
