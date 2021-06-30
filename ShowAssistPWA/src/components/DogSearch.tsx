import React, { Component } from 'react';
import Select from 'react-select';
// import debounce from 'lodash/debounce';

import { BasicDogLookupInfo } from '../models/dog';
import { searchDogsWithSimpleReturn } from '../api/dogService';

export interface DogSearchProps {
  selDog: BasicDogLookupInfo;
  isReadOnly: boolean;
  handleSelectionChangeFn: (selDog: BasicDogLookupInfo) => void;
}

export interface DogSearchState {
  matchesLoading: boolean;
  matches: BasicDogLookupInfo[];
}

export default class DogSearch extends Component<DogSearchProps, DogSearchState> {
  public state: DogSearchState = {
    matchesLoading: false,
    matches: [],
  };

  public constructor(props: DogSearchProps) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  public render(): JSX.Element {
    const { selDog, isReadOnly, handleSelectionChangeFn } = this.props;
    const { matchesLoading, matches } = this.state;
    return (
      <Select
        isLoading={matchesLoading}
        options={matches}
        isDisabled={isReadOnly}
        value={selDog}
        getOptionLabel={(option: BasicDogLookupInfo) =>
          option.dogName === '' ? 'No Name Provided' : option.dogName
        }
        getOptionValue={(option: BasicDogLookupInfo) => option.id}
        loadingMessage={() => 'retrieving results'}
        noOptionsMessage={obj =>
          obj.inputValue.length > 2 ? `no dogs were found matching ${obj.inputValue}` : ''
        }
        placeholder={'Name/BullyId/ABKC#'}
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
    //do nothing if length <3 except clear out matches if previously set?
    if (inputValue && inputValue.length === 3) {
      //use internal filtering after 3?
      //set state with a callback
      this.setState({ matchesLoading: true }, () => {
        searchDogsWithSimpleReturn(inputValue).then(({ data }) => {
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
