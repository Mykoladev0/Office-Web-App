import React, { Component } from 'react';
import Select from 'react-select';
// import debounce from 'lodash/debounce';

import { ShowParticipantInfo } from '../models/show';
// import { searchDogsWithSimpleReturn } from '../api/dogService';
import { searchParticipants } from '../api/showService';

export interface ParticipantSearchProps {
  participant: ShowParticipantInfo;
  showId: number;
  isReadOnly: boolean;
  handleSelectionChangeFn: (selParticipant: ShowParticipantInfo) => void;
}

export interface ParticipantSearchState {
  matchesLoading: boolean;
  matches: ShowParticipantInfo[];
}

export default class DogSearch extends Component<ParticipantSearchProps, ParticipantSearchState> {
  public state: ParticipantSearchState = {
    matchesLoading: false,
    matches: [],
  };

  public constructor(props: ParticipantSearchProps) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  public render(): JSX.Element {
    const { participant, isReadOnly, handleSelectionChangeFn } = this.props;
    const { matchesLoading, matches } = this.state;
    return (
      <Select
        isLoading={matchesLoading}
        options={matches}
        isDisabled={isReadOnly}
        value={participant}
        getOptionLabel={(option: ShowParticipantInfo) =>
          option.dog != null
            ? option.dog.dogName === ''
              ? 'No Name Provided'
              : option.dog.dogName
            : option.armbandNumber
        }
        getOptionValue={(option: ShowParticipantInfo) => option.dog.id}
        loadingMessage={() => 'retrieving results'}
        // noOptionsMessage={obj =>
        //   obj.inputValue.length > 2 ? `no dogs were found matching ${obj.inputValue}` : ''
        // }
        placeholder={'Name/Armband/BullyId/ABKC#'}
        onInputChange={this.handleInputChange}
        backspaceRemovesValue={true}
        isClearable={true}
        closeMenuOnSelect={true}
        onChange={handleSelectionChangeFn}
        menuPlacement={'auto'}
        maxMenuHeight={200}
      />
    );
  }

  private handleInputChange(inputValue: any) {
    const { showId } = this.props;
    //do nothing if length <3 except clear out matches if previously set?
    if (inputValue && inputValue.length > 0) {
      //use internal filtering after 3?
      //set state with a callback
      this.setState({ matchesLoading: true }, () => {
        searchParticipants(showId, inputValue).then(({ data }) => {
          this.setState({
            matches: data,
            matchesLoading: false,
          });
        });
      });
    } else {
      //clear out matches
      this.setState({ matchesLoading: false, matches: [] });
    }
  }
}
