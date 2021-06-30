import React, { Component } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import { Table, Divider, Icon, Input, Button, LocaleProvider } from 'antd';
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
class PendingUsers extends Component {
  state = {
    searchText: '',
    expandedRowKeys: [],
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/fetchPendingUsers',
    });
  }

  handleActivateUser = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/activatePendingUser',
      payload: id,
    });
  };

  handleDenyUserRequest = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/denyPendingUserRequest',
      payload: id,
    });
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

  render() {
    const { list } = this.props;
    const pendingUsers = list.pendingUsersList;
    const { expandedRowKeys } = this.state;

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
        sorter: (a, b) => a.email.toString().localeCompare(b.email),
        sortDirections: ['ascend', 'descend'],
      },
      {
        title: formatMessage({ id: 'component.table.header.role' }),
        dataIndex: 'role',
        key: 'role',
        sorter: (a, b) => a.role.toString().localeCompare(b.role),
        sortDirections: ['ascend', 'descend'],
      },
      {
        title: formatMessage({ id: 'component.table.header.action' }),
        key: 'operation',
        render: (text, record) => (
          <div className="rep-action-btn">
            <Button type="primary" onClick={e => this.handleActivateUser(record.id)}>
              <Icon type="check-circle" />
              <FormattedMessage id="component.table.activate" />
            </Button>{' '}
            <Divider type="vertical" />
            <Button type="primary" onClick={e => this.handleDenyUserRequest(record.id)}>
              <Icon type="user-delete" />
              <FormattedMessage id="component.table.denyRequest" />
            </Button>{' '}
          </div>
        ),
      },
    ];

    const data = [];

    pendingUsers &&
      pendingUsers.map(val =>
        data.push({
          key: val.id,
          id: val.id,
          firstname: val.profile && val.profile.firstName,
          lastname: val.profile && val.profile.lastName,
          email: val.profile && val.profile.email,
          role: val.roles && val.roles[0].name,
        })
      );

    const role = localStorage.getItem('user-role');
    return role === 'ABKCOffice' || role === 'Administrators' ? (
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.pending.users.list" />}
        >
          <Table
            columns={columns}
            dataSource={data}
            className={styles.representativeTable}
            scroll={{ x: 'fit-content' }}
            expandedRowKeys={expandedRowKeys}
          />
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

export default PendingUsers;
