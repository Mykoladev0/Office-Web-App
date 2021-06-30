import React from 'react';
import moment from 'moment';

import Card from 'reactstrap/lib/Card';
import CardBody from 'reactstrap/lib/CardBody';
import CardTitle from 'reactstrap/lib/CardTitle';
import CardSubtitle from 'reactstrap/lib/CardSubtitle';
import CardText from 'reactstrap/lib/CardText';
import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Button from 'reactstrap/lib/Button';

interface ShowDetailsProps {
  handleAddEventFn?: () => void;
  show: any;
}

const ShowDetails = (props: ShowDetailsProps) => {
  const { show, handleAddEventFn } = props;
  return (
    <Card>
      <CardBody>
        <Row>
          <Col sm={{ size: 10 }}>
            <CardTitle>{show.showName}</CardTitle>
          </Col>
          {handleAddEventFn && (
            <Col className={'float-sm-right'}>
              <Button color={'success'} onClick={handleAddEventFn}>
                Add Event
              </Button>
            </Col>
          )}
        </Row>
        <Row>
          <CardSubtitle>{`Show Date: ${moment(show.showDate).format('LL')}`}</CardSubtitle>
        </Row>
        <Row>
          <Col>
            <CardText>{`Show Location: ${
              show.address && show.address !== '' ? show.address : 'None Entered'
            }`}</CardText>
          </Col>
          <Col>
            <CardText>{`ABKC Representative: ${
              show.abkcrep && show.abkcrep !== '' ? show.abkcrep : 'None Entered'
            }`}</CardText>
          </Col>
          <Col>
            <CardText>
              Judges:{' '}
              {show.showJudges && show.showJudges.length > 0
                ? show.showJudges.map(j => `${j.firstName} ${j.lastName}`).join()
                : 'None Entered'}
            </CardText>
          </Col>
        </Row>
      </CardBody>
    </Card>
  );
};

export default ShowDetails;
