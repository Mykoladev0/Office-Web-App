import React, { Fragment } from 'react';
import { Layout, Icon } from 'antd';
import GlobalFooter from '@/components/GlobalFooter';
import { FormattedMessage } from 'umi/locale';

const { Footer } = Layout;
const FooterView = () => (
  <Footer style={{ padding: 0 }}>
    <GlobalFooter
      // links={[
      //   {
      //     key: 'Pro 首页',
      //     title: 'Pro 首页',
      //     href: 'https://pro.ant.design',
      //     blankTarget: true,
      //   },
      //   {
      //     key: 'github',
      //     title: <Icon type="github" />,
      //     href: 'https://github.com/ant-design/ant-design-pro',
      //     blankTarget: true,
      //   },
      //   {
      //     key: 'Ant Design',
      //     title: 'Ant Design',
      //     href: 'https://ant.design',
      //     blankTarget: true,
      //   },
      // ]}
      copyright={
        <Fragment>
          <FormattedMessage id="layout.basic.footer" /> <Icon type="copyright" /> 2019 ABKC Online
        </Fragment>
      }
    />
  </Footer>
);
export default FooterView;
