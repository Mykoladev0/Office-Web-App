import React, { Component } from 'react';
import { CustomInput, Button } from '../../../Vendor/mr-pro/components';
import Search from '@material-ui/icons/Search';
import ViewList from '@material-ui/icons/ViewList';
import Tooltip from '@material-ui/core/Tooltip';

export interface SearchInputProps {
  placeHolder: string;
  minTextLength?: number;
  handleSearchFn: (searchText: string, jumpToSearch: boolean) => void;
  showJumpSearchButton?: boolean;
}

export interface SearchInputState {
  searchText: string;
}

export default class SearchInput extends React.Component<SearchInputProps, SearchInputState> {
  public static defaultProps: Partial<SearchInputProps> = {
    minTextLength: 4,
    showJumpSearchButton: false,
  };
  public state: SearchInputState = {
    searchText: '',
  };

  public constructor(props: SearchInputProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { minTextLength, handleSearchFn, placeHolder, showJumpSearchButton } = this.props;
    const { searchText } = this.state;
    return (
      <div className={'searchWrapper'}>
        <CustomInput
          formControlProps={{
            // className: (classes.margin + ' ' + classes.search,
            className: 'margin search',
          }}
          inputProps={{
            placeholder: placeHolder,
            inputProps: {
              'aria-label': 'Search',
              onKeyPress: e => {
                if (e.key === 'Enter' && searchText.length >= minTextLength) {
                  handleSearchFn(searchText, false);
                }
              },
            },
            onChange: event => {
              this.setState({ searchText: event.target.value });
            },
            type: 'text', //sets the input field type (html5 input types!)
            value: searchText,
          }}
          onKeyPress={e => {
            if (e.key === 'Enter' && searchText.length >= minTextLength) {
              handleSearchFn(searchText, false);
            }
          }}
        />
        <Tooltip title="Show Results in card">
          <Button
            color="white"
            size="sm"
            disabled={searchText.length < minTextLength}
            aria-label="search"
            justIcon
            round
            onClick={() => handleSearchFn(searchText, false)}
          >
            <Search />
          </Button>
        </Tooltip>
        {showJumpSearchButton && (
          <Tooltip title="Show Results on Separate Page">
            <Button
              size="sm"
              color="rose"
              disabled={searchText.length < minTextLength}
              aria-label="search"
              justIcon
              round
              onClick={() => handleSearchFn(searchText, true)}
            >
              <ViewList />
            </Button>
          </Tooltip>
        )}
      </div>
    );
  }
}
