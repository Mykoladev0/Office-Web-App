import React, { Component } from 'react';

import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Button from 'reactstrap/lib/Button';
import Input from 'reactstrap/lib/Input';
import { FaSave } from 'react-icons/fa';
// import DogSearch from './DogSearch';
import { BasicDogLookupInfo } from '../models/dog';
import ParticipantSearch from './ParticipantSearch';
import { ShowParticipantInfo } from '../models/show';

export interface ResultProps {
  result: any;
  showId: number;
  isReadonly: boolean;
  handleSaveChangesFn: (result: any) => void;
}
interface ResultState {
  // armbandNumber: number;
  participant: ShowParticipantInfo;
  points: number;
}

export default class Result extends Component<ResultProps, ResultState> {
  public state: ResultState = {
    // armbandNumber: null,
    participant: null,
    points: 0,
  };
  public constructor(props: ResultProps) {
    super(props);
    this.handleSaveClick = this.handleSaveClick.bind(this);
    this.handleDogSelection = this.handleDogSelection.bind(this);
    this.handleParticipantSelection = this.handleParticipantSelection.bind(this);
    this.armbandNumberChange = this.armbandNumberChange.bind(this);
    this.pointsChange = this.pointsChange.bind(this);
  }

  public render(): JSX.Element {
    const { participant, points } = this.state;
    const { showId } = this.props;
    const canSave: boolean = participant != null && participant.armbandNumber != null;
    const canChangeArmband: boolean = participant !== null && participant.armbandNumber === null;
    return (
      <Row noGutters={false} className={'pt-2'}>
        <Col xs={2} sm={1}>
          <Button
            onClick={this.handleSaveClick}
            color="secondary"
            disabled={!canSave}
            size={'sm'}
            className={'align-middle'}
          >
            <FaSave />
          </Button>
        </Col>
        {/* Armband #, dogname (storing id!), points (readonly?) */}
        <Col xs={4} sm={3} md={2}>
          <Input
            placeholder="Armband #"
            name={'armbandNumber'}
            type={'number'}
            value={participant != null ? participant.armbandNumber : ''}
            readOnly={!canChangeArmband}
            onChange={event => this.armbandNumberChange(event.target.value)}
            bsSize="sm"
            // style={{ paddingTop: '2px', paddingBottom: '2px' }}
            className={'align-middle'}
          />
        </Col>
        <Col xs={6} sm={4}>
          <ParticipantSearch
            handleSelectionChangeFn={this.handleParticipantSelection}
            participant={participant}
            isReadOnly={false}
            showId={showId}
          />
        </Col>
        <Col xs={3} sm={2} md={1}>
          <Input
            placeholder="Points"
            name={'points'}
            onChange={event => this.pointsChange(event.target.value)}
            value={points}
            type={'number'}
            bsSize="sm"
            // style={{ paddingTop: '2px', paddingBottom: '2px' }}
            className={'align-middle'}
          />
        </Col>
      </Row>
    );
  }

  private handleDogSelection(dog: BasicDogLookupInfo) {
    const { showId } = this.props;
    const participant: ShowParticipantInfo = {
      dog,
      showId: showId,
    };
    this.setState({ participant });
  }
  private handleParticipantSelection(participant: ShowParticipantInfo) {
    this.setState({ participant });
  }
  private armbandNumberChange(armbandNumber: number): void {
    const { participant } = this.state;
    participant.armbandNumber = armbandNumber;
    this.setState({ participant });
  }
  private pointsChange(points: number): void {
    this.setState({ points });
  }
  private handleSaveClick() {
    const { handleSaveChangesFn } = this.props;
    const { participant, points } = this.state;
    const rtnObj: any = {
      armbandNumber: participant.armbandNumber,
      dogId: participant != null && participant.dog != null ? participant.dog.id : -1,
      points,
    };
    handleSaveChangesFn(rtnObj);
  }
}
