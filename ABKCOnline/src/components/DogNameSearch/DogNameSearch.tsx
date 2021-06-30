import { searchDogNameCall } from '@/services/searchApi';
import { AutoComplete, Button, Icon, Input, Option } from 'antd';
import React, { Component } from 'react';

const Option = AutoComplete.Option;

export interface IDogNameSearchProps {
  handleSelectionFn: (dog) => void;
  genderFilter: string;
  prompt: string;
}

export interface IDogNameSearchState {
  matchingDogs: any[];
  searching: boolean;
  searchTxt: string;
}

export default class DogNameSearch extends Component<IDogNameSearchProps, IDogNameSearchState> {
  public state: IDogNameSearchState = { matchingDogs: [], searching: false, searchTxt: '' };

  public constructor(props: IDogNameSearchProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { handleSelectionFn, prompt } = this.props;
    const { matchingDogs, searching, searchTxt } = this.state;
    return (
      // style={{ width: 300 }}
      <div className="global-search-wrapper">
        <AutoComplete
          className="global-search"
          size="large"
          style={{ width: '100%' }}
          dataSource={matchingDogs.map(this.renderOption)}
          onSelect={val => {
            // console.log(val);
            const dog = matchingDogs.find(d => d.originalTableId === parseInt(val));
            handleSelectionFn(dog);
          }}
          onSearch={() => {}}
          onChange={val => {
            console.log(val);
            this.setState({ searchTxt: val });
          }}
          placeholder={prompt}
          optionLabelProp="text"
        >
          <Input
            suffix={
              <Button
                className="search-btn"
                disabled={searching || searchTxt.length < 3}
                type="primary"
                onClick={() => this.handleSearch(searchTxt)}
              >
                {!searching ? <Icon type="search" /> : <Icon type="loading" />}
              </Button>
            }
          />
        </AutoComplete>
      </div>
    );
  }

  private handleSearch = async value => {
    if (value === null || value.length === 0) {
      this.setState({ matchingDogs: [], searching: false });
      return;
    }
    const { genderFilter } = this.props;
    this.setState({ searching: true });
    let matching = await searchDogNameCall(value);
    if (genderFilter && genderFilter !== '') {
      matching = matching.filter(d => d.gender === genderFilter);
    }
    this.setState({
      matchingDogs: value ? matching : [],
      searching: false,
    });
  };

  private renderOption = item => {
    return (
      <Option key={item.originalTableId} text={item.dogName}>
        <div className="global-search-item">
          <span className="global-search-item-desc">
            {item.dogName} {item.abkcNumber}
          </span>
        </div>
      </Option>
    );
  };
}
