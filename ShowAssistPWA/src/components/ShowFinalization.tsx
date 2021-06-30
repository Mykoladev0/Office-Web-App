import React, { Component } from 'react';
import Card from 'reactstrap/lib/Card';
import CardHeader from 'reactstrap/lib/CardHeader';
import CardBody from 'reactstrap/lib/CardBody';
import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Button from 'reactstrap/lib/Button';
import ShowEventsTable from './ShowEventsTable';
export interface ShowFinalizationProps {
  currentShow: any;
}

export interface ShowFinalizationState {
  currentShow: any;
}

export default class ShowFinalization extends Component<
  ShowFinalizationProps,
  ShowFinalizationState
> {
  public static getDerivedStateFromProps(nextProps, prevState) {
    // do things with nextProps.someProp and prevState.cachedSomeProp
    return {
      currentShow: nextProps.location.state.currentShow
        ? nextProps.location.state.currentShow
        : nextProps.currentShow,
    };
  }
  public state: ShowFinalizationState = {
    currentShow: null,
  };

  public constructor(props: ShowFinalizationProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { currentShow } = this.state;
    if (!currentShow) {
      return <h2>No Show Set!</h2>;
    }
    return (
      <Card>
        <CardHeader>
          <h3>{`Finalize ${currentShow.showName}`}</h3>
        </CardHeader>
        <CardBody>
          <Row>
            <Col>
              <p>Here a user can review show event and results prior to finalizing</p>
              <p>Quick Edits of existing results are also possible</p>
            </Col>
          </Row>
          <Row>
            <Col>
              <ShowEventsTable show={currentShow} isLoading={false} />
            </Col>
          </Row>
          <Row className="align-items-end">
            <Col>
              <Button color="danger">Submit for Approval</Button>
            </Col>
          </Row>
        </CardBody>
      </Card>
    );
  }
}
