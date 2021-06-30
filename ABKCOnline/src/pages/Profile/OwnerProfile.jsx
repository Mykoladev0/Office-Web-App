import React, { Component } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import { Table, Divider, Icon, Input, Button, LocaleProvider, Modal } from 'antd';
import enGB from 'antd/lib/locale-provider/en_GB';
import Highlighter from 'react-highlight-words';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import { formatMessage, FormattedMessage } from 'umi/locale';
import styles from './OwnerProfile.less';

@connect(({ list, loading }) => ({
  list,
  loading: loading.models.list,
}))
class OwnerProfile extends Component {
  state = {
    searchText: '',
    expandedRowKeys: [],
    visibleRefund: false,
    regType: '',
    commentValue: '',
    regId: ''
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/fetchOwnerData',
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
        type: 'list/fetchOwnerRegData',
        payload: { val: record.id, pending: false },
      });
    }
  };

  handlePasswordReset = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/resetPasswordRequest',
      payload: id,
    });
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

  handleSuspendAccount = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/suspendAccountOwner',
      payload: id,
    });
  }

  handleUnSuspendAccount = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/unSuspendAccountOwner',
      payload: id,
    });
  }

  handleChange = e => {
    const { name, value } = e.target;
    this.setState({
      [name]: value,
    });
  };

  handleCancel = () => {
    this.setState({
      visibleRefund: false,
      regId: '',
      regType: ''
    });
  };

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
    const ownerList = list.owners;
    const expandRegDetails = list.ownerListData;
    const { expandedRowKeys, visibleRefund, commentValue } = this.state;

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
          dataIndex: 'createdAt',
          key: 'createdAt',
          sorter: (a, b) => a.dateSubmitted.localeCompare(b.dateSubmitted),
          sortDirections: ['ascend', 'descend'],
        },
        {
          title: <FormattedMessage id="component.table.header.action" />,
          key: 'operation',
          render: (text, record) => (
            <div className="owner-action-btn">
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
      return (
        <Table
          columns={columns}
          dataSource={data}
          pagination={false}
          className={styles.ownerTable}
        />
      );
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
        title: formatMessage({ id: 'component.table.header.lastName' }),
        dataIndex: 'lastname',
        key: 'lastname',
        sorter: (a, b) => a.lastname.localeCompare(b.lastname),
        sortDirections: ['ascend', 'descend'],
      },
      {
        title: formatMessage({ id: 'component.table.header.email' }),
        dataIndex: 'email',
        key: 'email',
        sorter: (a, b) => a.email.localeCompare(b.email),
        sortDirections: ['ascend', 'descend'],
        ...this.getColumnSearchProps('email'),
      },
      {
        title: formatMessage({ id: 'component.table.header.action' }),
        key: 'operation',
        render: (text, record) => (
          <div className="owner-action-btn">
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
          </div>
        ),
      },
    ];

    const data = [];
    ownerList.map(val =>
      data.push({
        key: val.id,
        id: val.id,
        firstname: val.profile ? val.profile.firstName : val.loginName,
        lastname: val.profile && val.profile.lastName,
        email: val.profile && val.profile.email,
        isSuspended: val.isSuspended
      })
    );
    const role = localStorage.getItem('user-role');
    return role === 'ABKCOffice' || role === 'Administrators' ? (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.owners.list" />}
        >
          <Table
            columns={columns}
            expandedRowRender={expandedRowRender}
            dataSource={data}
            onExpand={this.onTableRowExpand}
            scroll={{ x: 'fit-content' }}
            expandedRowKeys={expandedRowKeys}
            className={styles.ownerTable}
          />
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

export default OwnerProfile;
