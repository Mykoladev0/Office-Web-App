import React, { Component } from 'react';

import Select from 'react-select';
import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Button from 'reactstrap/lib/Button';
import Label from 'reactstrap/lib/Label';
import GenderTypes, { GenderList, GenderFromString } from '../models/gender';
import ResultComponent from './ResultComponent';

export interface AddEventProps {
  eventDetails: any;
  handleDetailChanged: (eventDetails: any) => void;
  classTemplates: any[];
  styles: any[];
  breeds: any[];
  showId: number;
}

export interface AddEventState {
  a: boolean;
}

export default class AddEvent extends Component<AddEventProps, AddEventState> {
  public state: AddEventState = {
    a: false,
  };

  public constructor(props: AddEventProps) {
    super(props);
    this.handleSelectChange = this.handleSelectChange.bind(this);
    this.addResultsSection = this.addResultsSection.bind(this);
    this.addResults = this.addResults.bind(this);
    this.handleParticipantSaveChangesFn = this.handleParticipantSaveChangesFn.bind(this);
    this.addParticipantButton = this.addParticipantButton.bind(this);
  }

  public render(): JSX.Element {
    const { eventDetails, breeds, classTemplates, styles } = this.props;
    return (
      <div>
        <Row>
          <Col md={4}>
            <Select
              autoFocus={true}
              isMulti={false}
              isSearchable={true}
              placeholder={'Choose Breed'}
              name={'breed'}
              options={breeds}
              onChange={val => this.handleSelectChange('breed', val)}
              getOptionLabel={option => option.breed}
              getOptionValue={obj => {
                return obj.id;
              }}
              selectedValue={eventDetails['breed']}
            />
          </Col>
          <Col md={3}>
            <Select
              autoFocus={true}
              isMulti={false}
              isSearchable={true}
              placeholder={'Choose Class'}
              name={'classTemplate'}
              options={classTemplates}
              onChange={val => this.handleSelectChange('classTemplate', val)}
              getOptionValue={obj => {
                return obj.classId;
              }}
              getOptionLabel={option => option.name}
              labelKey={'name'}
              selectedValue={eventDetails['classTemplate']}
            />
          </Col>
          <Col md={3}>
            <Select
              autoFocus={true}
              isMulti={false}
              isSearchable={true}
              placeholder={'Choose Style'}
              name={'style'}
              options={styles}
              onChange={val => this.handleSelectChange('style', val)}
              getOptionValue={obj => {
                return obj.id;
              }}
              defaultValue={{ id: 1, styleName: 'Standard' }}
              getOptionLabel={option => option.styleName}
              selectedValue={eventDetails['style']}
            />
          </Col>
          <Col md={2}>
            <Select
              autoFocus={true}
              isMulti={false}
              isSearchable={true}
              placeholder={'Gender'}
              name={'gender'}
              options={GenderList.map(opt => ({ label: opt, value: opt }))}
              onChange={val => this.handleSelectChange('gender', val.value)}
              getOptionValue={obj => {
                return obj.value;
              }}
              selectedValue={eventDetails['gender']}
            />
          </Col>
        </Row>
        <br />
        {this.addResultsSection()}
      </div>
    );
  }
  private handleSelectChange(prop: any, val: any) {
    const { eventDetails, handleDetailChanged } = this.props;
    eventDetails[prop] = val;
    handleDetailChanged(eventDetails);
  }

  private addResultsSection() {
    const { showId } = this.props;
    // gender, breed, style,
    const { gender, breed, classTemplate, results } = this.props.eventDetails;
    if (!classTemplate) {
      return null;
    }
    if (classTemplate.name === 'Best of Show') {
      //don't need anything else
      if (results && results.length > 0) {
        //should only ever be one!
        return results.map((r, i) => (
          <ResultComponent
            result={r}
            key={i}
            showId={showId}
            isReadonly={false}
            handleSaveChangesFn={this.handleParticipantSaveChangesFn}
          />
        ));
      } else {
        //show add button
        return this.addParticipantButton();
      }
    }
    //need breed from here on out!
    if (!breed) {
      return null;
    }
    if (classTemplate.isBestOf) {
      //need gender and style for basic best and reserve best of
      if (classTemplate.name !== 'Best' || classTemplate.name !== 'Reserve Best') {
        //show results
        return this.addResults(results) || this.addParticipantButton();
      }
    }
    if (!gender || GenderFromString(gender) === GenderTypes.NA) {
      return null;
    }
    if (!breed) {
      return null;
    }

    //fall through to a happy, normal state, show results if exist AND add button
    return this.addResults(results) || this.addParticipantButton();
  }
  private addResults(results: any[]) {
    const { showId } = this.props;
    if (!results || results.length === 0) {
      return null;
    }
    const rtn = (
      <Col xs={12}>
        <Row noGutters={false}>
          <Col xs={2} sm={1}>
            {'  '}
          </Col>
          {/* Armband #, dogname (storing id!), points (readonly?) */}
          <Col xs={4} sm={3} md={2} className={'align-content-center'}>
            <Label className="text-center">Armband #</Label>
          </Col>
          <Col xs={6} sm={4} className={'align-content-center'}>
            <Label className="text-center">Dog Name</Label>
          </Col>
          <Col xs={3} sm={2} md={1} className={'align-content-center'}>
            <Label className="text-center">Points</Label>
          </Col>
        </Row>
        {results.map((r, i) => (
          <ResultComponent
            result={r}
            key={i}
            showId={showId}
            isReadonly={false}
            handleSaveChangesFn={this.handleParticipantSaveChangesFn}
          />
        ))}
      </Col>
    );
    return rtn;
  }
  private handleParticipantSaveChangesFn(result: any) {
    const { eventDetails, handleDetailChanged } = this.props;
    let { results } = eventDetails;
    if (!results) {
      results = [];
    }
    const foundIndex = results.findIndex(r => r.dogId === result.dogId);
    if (foundIndex > 0) {
      console.log('removing dog from result because it is being replaced');
      results.splice(foundIndex, 1, result);
      //warn user?
    } else {
      results.push(result);
    }
    //TODO:check for id in case of update/change
    //add it to current event details object
    handleDetailChanged(eventDetails);
  }
  private addParticipantButton() {
    const { handleDetailChanged } = this.props;
    let { eventDetails } = this.props;
    const addClick = () => {
      if (!eventDetails) {
        eventDetails = {
          results: [],
        };
      }
      let { results } = eventDetails;
      if (!results) {
        results = [];
      }
      results.push({});
      eventDetails.results = results;
      handleDetailChanged(eventDetails);
    };
    return (
      <Row>
        <Col xs={10}>
          <Button color="primary" size="sm" onClick={addClick} className={'float-right'}>
            Add Participant
          </Button>
        </Col>
      </Row>
    );
  }
}
