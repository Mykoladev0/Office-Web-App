import React, { Component } from 'react';
import Card from 'reactstrap/lib/Card';
import CardHeader from 'reactstrap/lib/CardHeader';
import CardBody from 'reactstrap/lib/CardBody';
import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Label from 'reactstrap/lib/Label';
import Button from 'reactstrap/lib/Button';
import Input from 'reactstrap/lib/Input';

import { FaBarcode } from 'react-icons/fa';

import DogSearch from './DogSearch';
import { BasicDogLookupInfo } from '../models/dog';
import { registerDogForShow } from '../api/showService';
export interface ShowRegistrationProps {
  currentShow: any;
}

export interface ShowRegistrationState {
  dog: BasicDogLookupInfo;
  armbandNumber: number;
  currentShow: any;
  registering: boolean;
}

export default class ShowRegistration extends Component<
  ShowRegistrationProps,
  ShowRegistrationState
> {
  public static getDerivedStateFromProps(nextProps, prevState) {
    // do things with nextProps.someProp and prevState.cachedSomeProp
    return {
      currentShow: nextProps.location.state.currentShow
        ? nextProps.location.state.currentShow
        : nextProps.currentShow,
    };
  }
  public state: ShowRegistrationState = {
    dog: null,
    armbandNumber: null,
    currentShow: null,
    registering: false,
  };

  public constructor(props: ShowRegistrationProps) {
    super(props);
    this.handleDogSelection = this.handleDogSelection.bind(this);
    this.handleRegisterDog = this.handleRegisterDog.bind(this);
    this.armbandNumberChange = this.armbandNumberChange.bind(this);
  }

  public render(): JSX.Element {
    const { currentShow, dog, armbandNumber } = this.state;

    if (!currentShow) {
      return <h2>No Show Set!</h2>;
    }
    return (
      <Card>
        <CardHeader>
          <h3>{`Register Dogs for Show ${currentShow.showName}`}</h3>
        </CardHeader>
        <CardBody>
          <Row>
            <Col xs={8} sm={5}>
              <Col xs={12}>
                <Label>Search By Dog's Name, Bully ID, or ABKC #</Label>
              </Col>
              <Row>
                <Col xs={11}>
                  <DogSearch
                    handleSelectionChangeFn={this.handleDogSelection}
                    selDog={dog}
                    isReadOnly={false}
                  />
                </Col>
                <Col xs={1}>
                  <Button outline size={'sm'} color={'primary'}>
                    <FaBarcode />
                  </Button>
                </Col>
              </Row>
              {dog && (
                <div>
                  <br />
                  <Row className="">
                    <Col xs={8} sm={6} md={6}>
                      <Col xs={12}>Assigned Armband #</Col>
                      <Col xs={12}>
                        <Input
                          placeholder="Armband #"
                          name={'armbandNumber'}
                          type={'number'}
                          onChange={event => this.armbandNumberChange(event.target.value)}
                          bsSize="sm"
                          // style={{ paddingTop: '2px', paddingBottom: '2px' }}
                          className={'align-middle'}
                        />
                      </Col>
                    </Col>
                    <Col className="float-right md-auto align-self-center">
                      <Button
                        className="align-self-end"
                        onClick={this.handleRegisterDog}
                        color={'success'}
                        size="lg"
                        disabled={armbandNumber === null || armbandNumber === 0}
                      >
                        Register Dog
                      </Button>
                    </Col>
                  </Row>
                </div>
              )}
            </Col>
            <Col xs={12} sm={7}>
              {generateDetailsView(dog)}
            </Col>
          </Row>
        </CardBody>
      </Card>
    );
  }
  private handleDogSelection(dog: BasicDogLookupInfo) {
    this.setState({ dog });
  }
  private armbandNumberChange(armbandNumber: number) {
    this.setState({ armbandNumber });
  }

  private handleRegisterDog() {
    const { currentShow, dog, armbandNumber } = this.state;
    this.setState({ registering: true }, () => {
      registerDogForShow(currentShow.showId, dog, armbandNumber).then(r =>
        //probably need to notify the user of success?
        this.setState({ registering: false, dog: null, armbandNumber: null })
      );
    });
  }
}

const generateDetailsView = dog => {
  if (!dog) {
    return null;
  }
  return (
    <Col xs={12}>
      <h4>Selected Dog Details</h4>
    </Col>
  );
};
