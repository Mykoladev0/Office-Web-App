import React, { Component } from 'react';
import { Col, Container, Row } from 'reactstrap';
// import { Button } from 'antd';
// import 'antd/dist/antd.css';
import NavMenu from './NavMenu/NavMenu';

interface LayoutProps {
  currentShow: any;
}

export class Layout extends Component<LayoutProps, any> {
  public displayName = Layout.name;
  public render() {
    const { currentShow } = this.props;
    return (
      <Container fluid={true} id="wrapper">
        <Row>
          {/* col-xs-12 col-sm-4 col-lg-3 col-xl-2 */}
          <Col sm={4} xs={12} lg={3} xl={2}>
            <NavMenu currentShow={currentShow} />
          </Col>
          {/* col-xs-12 col-sm-8 col-lg-9 col-xl-10 pt-3 pl-4 ml-auto */}
          <Col sm={8} xs={12} lg={9} xl={10} className="pt-3 pl-4 ml-auto">
            {this.props.children}
          </Col>
        </Row>
      </Container>
    );
  }
}
