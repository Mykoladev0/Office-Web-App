import React, { memo } from 'react';
import { Row, Col, Icon, Tooltip } from 'antd';
import { FormattedMessage } from 'umi/locale';
import styles from './Analysis.less';
import { ChartCard, MiniArea, MiniBar, MiniProgress, Field } from '@/components/Charts';
import Trend from '@/components/Trend';
import numeral from 'numeral';
import Yuan from '@/utils/Yuan';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';

const topColResponsiveProps = {
  xs: 24,
  sm: 12,
  md: 12,
  lg: 12,
  xl: 6,
  style: { marginBottom: 24 },
};

const IntroduceRow = memo(({ loading, visitData }) => (
  <PageHeaderWrapper
    className={styles.formHeader}
    title={<FormattedMessage id="menu.dashboard" />}
  >
    <Row gutter={24}>
      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          title={<FormattedMessage id="app.analysis.total-sales" defaultMessage="Total Sales" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          loading={loading}
          total={() => <Yuan>126560</Yuan>}
          footer={
            <Field
            // label={<FormattedMessage id="app.analysis.day-sales" defaultMessage="Daily Sales" />}
            // value={`￥${numeral(12423).format('0,0')}`}
            />
          }
          contentHeight={46}
        >
          {/* <Trend flag="up" style={{ marginRight: 16 }}>
          <FormattedMessage id="app.analysis.week" defaultMessage="Weekly Changes" />
          <span className={styles.trendText}>12%</span>
        </Trend>
        <Trend flag="down">
          <FormattedMessage id="app.analysis.day" defaultMessage="Daily Changes" />
          <span className={styles.trendText}>11%</span>
        </Trend> */}
        </ChartCard>
      </Col>

      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          loading={loading}
          title={<FormattedMessage id="app.analysis.visits" defaultMessage="Visits" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          total={numeral(8846).format('0,0')}
          footer={
            <Field
            // label={<FormattedMessage id="app.analysis.day-visits" defaultMessage="Daily Visits" />}
            // value={numeral(1234).format('0,0')}
            />
          }
          contentHeight={46}
        >
          {/* <MiniArea color="#975FE4" data={visitData} /> */}
        </ChartCard>
      </Col>
      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          loading={loading}
          title={<FormattedMessage id="app.analysis.payments" defaultMessage="Payments" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          total={numeral(6560).format('0,0')}
          footer={
            <Field
            // label={
            //   <FormattedMessage
            //     id="app.analysis.conversion-rate"
            //     defaultMessage="Conversion Rate"
            //   />
            // }
            // value="60%"
            />
          }
          contentHeight={46}
        >
          {/* <MiniBar data={visitData} /> */}
        </ChartCard>
      </Col>
      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          loading={loading}
          title={<FormattedMessage id="app.analysis.operational-effect" defaultMessage="Payments" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          total={numeral(6560).format('0,0')}
          footer={
            <Field
            // label={
            //   <FormattedMessage
            //     id="app.analysis.conversion-rate"
            //     defaultMessage="Conversion Rate"
            //   />
            // }
            // value="60%"
            />
          }
          contentHeight={46}
        >
          {/* <MiniBar data={visitData} /> */}
        </ChartCard>
      </Col>
      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          loading={loading}
          title={<FormattedMessage id="app.analysis.sales-trend" defaultMessage="Payments" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          total={numeral(6560).format('0,0')}
          footer={
            <Field
            // label={
            //   <FormattedMessage
            //     id="app.analysis.conversion-rate"
            //     defaultMessage="Conversion Rate"
            //   />
            // }
            // value="60%"
            />
          }
          contentHeight={46}
        >
          {/* <MiniBar data={visitData} /> */}
        </ChartCard>
      </Col>
      <Col {...topColResponsiveProps}>
        <ChartCard
          bordered={false}
          loading={loading}
          title={<FormattedMessage id="app.analysis.conversion-rate" defaultMessage="Payments" />}
          action={
            <Tooltip
              title={<FormattedMessage id="app.analysis.introduce" defaultMessage="Introduce" />}
            >
              <Icon type="info-circle-o" />
            </Tooltip>
          }
          total={numeral(6560).format('0,0')}
          footer={
            <Field
            // label={
            //   <FormattedMessage
            //     id="app.analysis.conversion-rate"
            //     defaultMessage="Conversion Rate"
            //   />
            // }
            // value="60%"
            />
          }
          contentHeight={46}
        >
          {/* <MiniBar data={visitData} /> */}
        </ChartCard>
      </Col>
    </Row>
  </PageHeaderWrapper>
));

export default IntroduceRow;
