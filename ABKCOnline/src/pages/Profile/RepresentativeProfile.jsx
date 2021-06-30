import React, { Component } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import { Table, Divider, Icon, Input, Button, LocaleProvider, Modal, Row, Col } from 'antd';
import enGB from 'antd/lib/locale-provider/en_GB';
import Highlighter from 'react-highlight-words';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import { formatMessage, FormattedMessage } from 'umi/locale';
import styles from './RepresentativeProfile.less';

moment.locale('en');
@connect(({ list, loading }) => ({
  list,
  loading: loading.models.list,
}))
class RepresentativeProfile extends Component {
  state = {
    searchText: '',
    expandedRowKeys: [],
    visible: false,
    pedigreeRegistrationFee: '',
    litterRegistrationFee: '',
    puppyRegistrationFee: '',
    bullyIdRequestFee: '',
    jrHandlerRegistrationFee: '',
    transferFee: '',
    regId: '',
    visibleRefund: false,
    regType: '',
    commentValue: '',
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/fetchRepData',
    });
  }

  onTableRowExpand = (expanded, record) => {
    const { dispatch } = this.props;
    const keys = [];
    if (expanded) {
      keys.push(record.id);
    }
    this.setState({ expandedRowKeys: keys });
    if (expanded) {
      dispatch({
        type: 'list/filterByRep',
        payload: { val: record.id, pending: false },
      });
    }
  };

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

  handleSubmit = () => {
    const { dispatch } = this.props;
    const {
      regId,
      pedigreeRegistrationFee,
      litterRegistrationFee,
      puppyRegistrationFee,
      bullyIdRequestFee,
      jrHandlerRegistrationFee,
      transferFee,
    } = this.state;

    const updatedFees = {
      pedigreeRegistrationFee: parseInt(pedigreeRegistrationFee),
      litterRegistrationFee: parseInt(litterRegistrationFee),
      puppyRegistrationFee: parseInt(puppyRegistrationFee),
      bullyIdRequestFee: parseInt(bullyIdRequestFee),
      jrHandlerRegistrationFee: parseInt(jrHandlerRegistrationFee),
      transferFee: parseInt(transferFee),
    };

    dispatch({
      type: 'list/updateRegistrationFee',
      payload: { id: regId, updatedFees },
    });

    this.setState({
      visible: false,
      visibleRefund: false,
      regId: '',
      regType: ''
    });
  };

  showModal = (text, record) => {
    this.setState({
      visible: true,
      regId: record.id,
      pedigreeRegistrationFee: record.pedigreeRegistrationFee,
      litterRegistrationFee: record.litterRegistrationFee,
      puppyRegistrationFee: record.puppyRegistrationFee,
      bullyIdRequestFee: record.bullyIdRequestFee,
      jrHandlerRegistrationFee: record.jrHandlerRegistrationFee,
      transferFee: record.transferFee,
    });
  };

  handleCancel = () => {
    this.setState({
      visible: false,
      visibleRefund: false,
      regId: '',
      regType: ''
    });
  };

  handleChange = e => {
    const { name, value } = e.target;
    this.setState({
      [name]: value,
    });
  };

  handlePasswordReset = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/resetPasswordRequest',
      payload: id,
    });
  }

  handleSuspendAccount = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/suspendAccountRep',
      payload: id,
    });
  }

  handleUnSuspendAccount = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/unSuspendAccountRep',
      payload: id,
    });
  }

  handleIssueRefund = () => {
    const { commentValue, regId, regType } = this.state;
    const { dispatch } = this.props;
    dispatch({
      type: 'list/issueRefund',
      payload: {
        commentValue,
        regId,
        regType
      },
    });
    this.setState({
      visible: false,
      visibleRefund: false,
      regId: '',
      regType: ''
    });
  }

  showRefundModal = (val, record) => {
    this.setState({
      visibleRefund: true,
      regId: record.id,
      regType: record.registrationType,
      commentValue: '',
    });
  };

  render() {
    const { list } = this.props;
    const repList = list.representatives;
    const expandRegDetails = list.listData;
    const {
      expandedRowKeys,
      visible,
      pedigreeRegistrationFee,
      litterRegistrationFee,
      puppyRegistrationFee,
      bullyIdRequestFee,
      jrHandlerRegistrationFee,
      transferFee,
      visibleRefund,
      commentValue
    } = this.state;

    const expandedRowRender = () => {
      const columns = [
        {
          title: <FormattedMessage id="component.table.header.id" />,
          dataIndex: 'id',
          key: 'id',
          sorter: (a, b) => a.id.toString().localeCompare(b.id),
          sortDirections: ['ascend', 'descend'],
        },
        {
          title: <FormattedMessage id="component.table.header.registrations" />,
          dataIndex: 'name',
          key: 'name',
          sorter: (a, b) => a.name.localeCompare(b.name),
          sortDirections: ['ascend', 'descend'],
        },
        {
          title: <FormattedMessage id="component.table.header.submitted.date" />,
          dataIndex: 'dateSubmitted',
          key: 'dateSubmitted',
          sorter: (a, b) => a.dateSubmitted.localeCompare(b.dateSubmitted),
          sortDirections: ['ascend', 'descend'],
        },
        {
          title: <FormattedMessage id="component.table.header.action" />,
          key: 'operation',
          render: (text, record) => (
            <div className="rep-action-btn">
              <Button type="primary" onClick={e => this.showRefundModal('Approve', record)}>
                <Icon type="dollar" />
                <FormattedMessage id="component.table.issue.refund" />
              </Button>{' '}
            </div>
          ),
        },
      ];

      const data = [];
      expandRegDetails.map((val, key) =>
        data.push({
          key,
          id: val.id,
          name: val.dogInfo.dogName,
          dateSubmitted: moment(val.dateSubmitted).format('MMMM Do YYYY'),
          registrationType: val.registrationType
        })
      );

      return <Table columns={columns} dataSource={data} pagination={false} />;
    };

    const columns = [
      {
        title: formatMessage({ id: 'component.table.header.firstName' }),
        dataIndex: 'firstname',
        key: 'firstname',
        sorter: (a, b) => a.firstname.localeCompare(b.firstname),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('firstname'),
      },
      {
        title: <FormattedMessage id="component.table.header.registrations" />,
        dataIndex: 'registrations',
        key: 'registrations',
        sorter: (a, b) => a.registrations.toString().localeCompare(b.registrations),
        sortDirections: ['ascend', 'descend'],
      },
      {
        title: <FormattedMessage id="component.table.header.pending.registrations" />,
        dataIndex: 'pending',
        key: 'pending',
        sorter: (a, b) => a.pending.toString().localeCompare(b.pending),
        sortDirections: ['ascend', 'descend'],
      },
      {
        title: <FormattedMessage id="component.table.header.action" />,
        key: 'operation',
        render: (text, record) => (
          <div className="rep-action-btn">
            {record.isSuspended === 'False' ?
              <Button type="primary" onClick={e => this.handleSuspendAccount(record.id)}>
                <Icon type="eye-invisible" />
                <FormattedMessage id="component.table.suspend" />
              </Button> :
              <Button type="primary" onClick={e => this.handleUnSuspendAccount(record.id)}>
                <Icon type="eye-invisible" />
                <FormattedMessage id="component.table.unsuspend" />
              </Button>
            }
            {' '}
            <Divider type="vertical" />
            <Button type="danger" ghost onClick={e => this.handlePasswordReset(record.id)}>
              <Icon type="exclamation-circle" />
              <FormattedMessage id="component.table.resetPassword" />
            </Button>{' '}
            <Divider type="vertical" />
            <Button type="primary" onClick={e => this.showModal(text, record)}>
              <Icon type="edit" />
              <FormattedMessage id="component.table.update.fee" />
            </Button>{' '}
          </div>
        ),
      },
    ];

    const data = [];
    // const repWithRegistration = repList.filter(rep => rep.registrationCount > 0);
    repList.map(val =>
      data.push({
        key: val.id,
        id: val.id,
        firstname: val.profile ? val.profile.firstName : val.loginName,
        registrations: val.registrationCount,
        pending: val.pendingRegistrationCount,
        jrHandlerRegistrationFee: val.jrHandlerRegistrationFee,
        bullyIdRequestFee: val.bullyIdRequestFee,
        litterRegistrationFee: val.litterRegistrationFee,
        pedigreeRegistrationFee: val.pedigreeRegistrationFee,
        transferFee: val.transferFee,
        puppyRegistrationFee: val.puppyRegistrationFee,
        representativeId: val.representativeId,
        isSuspended: val.isSuspended
      })
    );
    const role = localStorage.getItem('user-role');
    return role === 'ABKCOffice' || role === 'Administrators' ? (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.representatives.list" />}
        >
          <Table
            columns={columns}
            expandedRowRender={expandedRowRender}
            onExpand={this.onTableRowExpand}
            dataSource={data}
            className={styles.representativeTable}
            scroll={{ x: 'fit-content' }}
            expandedRowKeys={expandedRowKeys}
          />
          <Modal
            title="Update Registration Fee"
            className={styles.updateFeeModal}
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
            <Row gutter={24}>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="pedigreeRegistrationFee">
                  <FormattedMessage id="form.update.fee.pedigree" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={pedigreeRegistrationFee}
                  name="pedigreeRegistrationFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="litterRegistrationFee">
                  <FormattedMessage id="form.update.fee.litter" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={litterRegistrationFee}
                  name="litterRegistrationFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
            </Row>
            <Row gutter={24}>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="puppyRegistrationFee">
                  <FormattedMessage id="form.update.fee.puppy" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={puppyRegistrationFee}
                  name="puppyRegistrationFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="bullyIdRequestFee">
                  <FormattedMessage id="form.update.fee.bully" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={bullyIdRequestFee}
                  name="bullyIdRequestFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
            </Row>
            <Row gutter={24}>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="jrHandlerRegistrationFee">
                  <FormattedMessage id="form.update.fee.junior" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={jrHandlerRegistrationFee}
                  name="jrHandlerRegistrationFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
              <Col lg={12} md={12} sm={24}>
                <label htmlFor="transferFee">
                  <FormattedMessage id="form.update.fee.transfer" />:
                </label>
                <Input
                  placeholder={formatMessage({ id: 'form.update.fee.placeholder' })}
                  value={transferFee}
                  name="transferFee"
                  onChange={e => this.handleChange(e)}
                />
              </Col>
            </Row>
          </Modal>
          <Modal
            title="Issue Refund"
            visible={visibleRefund}
            onOk={this.handleIssueRefund}
            onCancel={this.handleCancel}
            footer={[
              <Button key="submit" type="default" onClick={this.handleCancel}>
                <FormattedMessage id="menu.btn.cancel" />
              </Button>,
              <Button key="submit" type="primary" onClick={this.handleIssueRefund}>
                <FormattedMessage id="menu.btn.save" />
              </Button>,
            ]}
          >
            <Input
              placeholder={formatMessage({ id: 'menu.modal.reason.placeholder' })}
              value={commentValue}
              name="commentValue"
              onChange={e => this.handleChange(e)}
            />
          </Modal>
        </PageHeaderWrapper>
      </LocaleProvider>
    ) : (
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.no.access" />}
        />
      );
  }
}

export default RepresentativeProfile;
