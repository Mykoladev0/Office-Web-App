import React, { Component, Suspense } from 'react';
import { connect } from 'dva';
import { Row, Col, Icon, Menu, Dropdown, List, message, Card, Avatar, Spin, } from 'antd';

import GridContent from '@/components/PageHeaderWrapper/GridContent';
import { getTimeDistance } from '@/utils/utils';

import styles from './Analysis.less';
import PageLoading from '@/components/PageLoading';

const IntroduceRow = React.lazy(() => import('./IntroduceRow'));
const SalesCard = React.lazy(() => import('./SalesCard'));
const TopSearch = React.lazy(() => import('./TopSearch'));
const ProportionSales = React.lazy(() => import('./ProportionSales'));
const OfflineData = React.lazy(() => import('./OfflineData'));

@connect(({ chart, loading }) => ({
  chart,
  loading: loading.effects['chart/fetch'],
}))
class Analysis extends Component {
  constructor(props) {
    super(props);
    this.myHandleImageLoadEvent = this.myHandleImageLoadEvent.bind(this);
  }
  state = {
    salesType: 'all',
    currentTabKey: '',
    rangePickerValue: getTimeDistance('year'),
    conentWrapperMinHeight: '',
  };

  componentDidMount() {
    const { dispatch } = this.props;
    this.reqRef = requestAnimationFrame(() => {
      dispatch({
        type: 'chart/fetch',
      });
    });
    this.myHandleImageLoadEvent()
  }

  componentWillUnmount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'chart/clear',
    });
    cancelAnimationFrame(this.reqRef);
    clearTimeout(this.timeoutId);
  }

  myHandleImageLoadEvent(e) {
    var rowHeight = document.getElementById('content-row').clientHeight;
    this.setState({ conentWrapperMinHeight: rowHeight })
  }

  handleChangeSalesType = e => {
    this.setState({
      salesType: e.target.value,
    });
  };

  render() {
    const { rangePickerValue, salesType, currentTabKey } = this.state;
    const { chart, loading } = this.props;
    const {
      visitData,
      visitData2,
      salesData,
      searchData,
      offlineData,
      offlineChartData,
      salesTypeData,
      salesTypeDataOnline,
      salesTypeDataOffline,
    } = chart;
    let salesPieData;
    if (salesType === 'all') {
      salesPieData = salesTypeData;
    } else {
      salesPieData = salesType === 'online' ? salesTypeDataOnline : salesTypeDataOffline;
    }
    const menu = (
      <Menu>
        <Menu.Item>操作一</Menu.Item>
        <Menu.Item>操作二</Menu.Item>
      </Menu>
    );

    const dropdownGroup = (
      <span className={styles.iconGroup}>
        <Dropdown overlay={menu} placement="bottomRight">
          <Icon type="ellipsis" />
        </Dropdown>
      </span>
    );

    const activeKey = currentTabKey || (offlineData[0] && offlineData[0].name);
    const data = [
      {
        id: 1,
        title: 'Registration 1',
      },
      {
        id: 2,
        title: 'Registration 2',
      },
      {
        id: 3,
        title: 'Registration 3',
      },
      {
        id: 4,
        title: 'Registration 4',
      },
      {
        id: 5,
        title: 'Registration 5',
      },
    ];
    return (
      <GridContent>
        <Suspense fallback={<PageLoading />}>
          <IntroduceRow loading={loading} visitData={visitData} />
        </Suspense>
        <div className={styles.twoColLayout}>
          <Row gutter={24} id="content-row" onLoad={(event) => this.myHandleImageLoadEvent(event)}>
            <Col xl={16} lg={16} md={16} sm={16} xs={24}>
              <Suspense fallback={null}>
                <ProportionSales
                  dropdownGroup={dropdownGroup}
                  salesType={salesType}
                  loading={loading}
                  salesPieData={salesPieData}
                  handleChangeSalesType={this.handleChangeSalesType}
                />
              </Suspense>
            </Col>
            <Col xl={8} lg={8} md={8} sm={8} xs={24} style={{ minHeight: this.state.conentWrapperMinHeight }}>
              <Suspense fallback={null}>
                <Card title="Recent Registrations" style={{ marginTop: 24, minHeight: this.state.conentWrapperMinHeight }}>
                  <List
                    itemLayout="horizontal"
                    dataSource={data}
                    renderItem={item => (
                      <List.Item>
                        <List.Item.Meta
                          title={<a href="https://ant.design">{item.id}. {item.title}</a>}
                        />
                      </List.Item>
                    )}
                  />
                </Card>
              </Suspense>
            </Col>
          </Row>
        </div>
      </GridContent>
    );
  }
}

export default Analysis;
