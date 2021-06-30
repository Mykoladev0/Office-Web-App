import React, {Component} from 'react';
import Autosuggest from 'react-autosuggest';
import match from 'autosuggest-highlight/match';
import parse from 'autosuggest-highlight/parse';
import TextField from '@material-ui/core/TextField';
import Paper from '@material-ui/core/Paper';
import MenuItem from '@material-ui/core/MenuItem';
import withStyles, {WithStyles} from '@material-ui/core/styles/withStyles';
import {createStyles} from '@material-ui/core/styles';
import IsolatedScroll from 'react-isolated-scroll';

import {BreedModel} from '../../App/Models';

const autoCompleteStyles = (theme) =>  createStyles({
    container: {
      flexGrow: 1,
      position: 'relative',
      // height: 150,
    },
    suggestionsContainerOpen: {
      position: 'absolute',
      zIndex: 1,
      marginTop: 1,//theme.spacing.unit,
      left: 0,
      right: 0,
    },
    suggestion: {
      display: 'block',
    },
    suggestionsList: {
      margin: 0,
      padding: 0,
      listStyleType: 'none',
    },
    listStyle:{ maxHeight: 150, overflow: 'auto' }
  });

export interface BreedAutoCompleteProps extends WithStyles<typeof autoCompleteStyles> {
    value?: BreedModel;
    breeds: BreedModel[];
    readonly?: boolean;
    handleChangeFn?: (selected: BreedModel) => void;
}

export interface BreedAutoCompleteState {
    value: string;
    suggestions: BreedModel[];
    selectedValue: BreedModel;
}

export class BreedAutoCompleteComp extends React.Component<BreedAutoCompleteProps, BreedAutoCompleteState> {
    public static defaultProps: Partial<BreedAutoCompleteProps> = {
        value: null,
        // tslint:disable-next-line:no-empty
        handleChangeFn: (selected: BreedModel) => {}
    };
    public static getDerivedStateFromProps(nextProps, prevState) {
        const {value, breeds} = nextProps;
        const prevValue = prevState.value;
        const prevSuggestions = prevState.suggestions;
        return {
            value: prevValue === null || prevValue === ''?value?value.breed:'':prevValue,
            selectedValue: value,
            suggestions: prevSuggestions.length === 0?breeds:prevSuggestions
        };
      }

    public state: BreedAutoCompleteState = {
        value: "",
        suggestions: [],
        selectedValue: null
    };

  public constructor(props: BreedAutoCompleteProps) {
    super(props);
    this.handleChange = this.handleChange.bind(this);
    this.handleSuggestionsFetchRequested = this.handleSuggestionsFetchRequested.bind(this);
    this.handleSuggestionsClearRequested = this.handleSuggestionsClearRequested.bind(this);
    this.onSuggestionSelected = this.onSuggestionSelected.bind(this);
    this.handleOnBlur = this.handleOnBlur.bind(this);
  }

  public render(): JSX.Element {
    const classes = this.props.classes as any;
    const {readonly} = this.props;
    const {suggestions, value} = this.state;
    const inputProps = {
        classes,
        placeholder: 'Start typing to find a breed...',
        value: value,
        onChange: this.handleChange,
        onBlur: this.handleOnBlur,//use this for autosuggest with custom inputs from user
        readOnly: readonly
    };
    return (
        <Autosuggest
            theme={{
                container: classes.container,
                suggestionsContainerOpen: classes.suggestionsContainerOpen,
                suggestionsList: classes.suggestionsList,
                suggestion: classes.suggestion,
            }}
            id={"breedSelector"}
            // alwaysRenderSuggestions={true}
            renderInputComponent={renderInput}
            suggestions={suggestions}
            onSuggestionsFetchRequested={this.handleSuggestionsFetchRequested}
            onSuggestionsClearRequested={this.handleSuggestionsClearRequested}
            renderSuggestionsContainer={renderSuggestionsContainer}
            onSuggestionSelected={this.onSuggestionSelected}
            getSuggestionValue={getSuggestionValue}
            renderSuggestion={renderSuggestion}
            inputProps={inputProps}
        />
    );
  }
  private handleSuggestionsFetchRequested({value}) {
    this.setState({
        suggestions: this.getSuggestions(value),
      });
  }

  private handleSuggestionsClearRequested() {
    this.setState({
      suggestions: [],
    });
  }

  private handleChange(event, { newValue }) {
    const {handleChangeFn} = this.props;
    // handleChangeFn(newValue);
    this.setState({
        value: newValue,
    });
  }
  private onSuggestionSelected(event, { suggestion, suggestionValue}) {
      this.setState({
        selectedValue: suggestion
      });
      const {handleChangeFn} = this.props;
      handleChangeFn(suggestion);
  }
  private handleOnBlur(event) {
      //check to see if it is valid selection, otherwise clear out
      const {breeds} = this.props;
      const {value} = this.state;
      const foundIndex = breeds.findIndex(b=>b.breed === value);
      if(foundIndex === -1) {
          this.setState({
              value:''
          });
      }
  }

  private getSuggestions(value) {
    const inputValue = value.trim().toLowerCase();
    const inputLength = inputValue.length;
    let count = 0;
    const {breeds} = this.props;
    const rtnVal = inputLength === 0
      ? []//want all the breeds available?
      : breeds.filter(suggestion => {
          const keep =
            // count < 5 && suggestion.breed.toLowerCase().slice(0, inputLength) === inputValue;
            count < 5 && suggestion.breed.toLowerCase().includes(inputValue);
          if (keep) {
            count += 1;
          }
          return keep;
        });
    return rtnVal;
  }
}
const renderInput = (inputProps) => {
    const { classes, ref, ...other } = inputProps;
    return (
      <TextField
        fullWidth
        InputProps={{
          inputRef: ref,
          classes: {
            input: classes.input,
          },
          ...other,
        }}
      />
    );
  };

const renderSuggestion = (suggestion: BreedModel, { query, isHighlighted }) => {
    const matches = match(suggestion.breed, query);
    const parts = parse(suggestion.breed, matches);
    return (
      <MenuItem selected={isHighlighted} component="div">
        <div>
          {parts.map((part, index) => {
            return part.highlight ? (
              <span key={String(index)} style={{ fontWeight: 300 }}>
                {part.text}
              </span>
            ) : (
              <strong key={String(index)} style={{ fontWeight: 500 }}>
                {part.text}
              </strong>
            );
          })}
        </div>
      </MenuItem>
    );
  };

const renderSuggestionsContainer = (options) => {
    const { containerProps, children } = options;
    const { ref, ...restContainerProps } = containerProps;
    // const callRef = isolatedScroll => {
    //   if (isolatedScroll !== null) {
    //     ref(isolatedScroll.component);
    //   }
    // };
    return (
        <Paper
            style={{maxHeight: 200, overflow: 'auto'}}
            {...containerProps} square>
            {children}
        </Paper>
    //     <IsolatedScroll ref={callRef} {...restContainerProps}>

    //   </IsolatedScroll>
    );
  };

const getSuggestionValue = (suggestion: BreedModel) => {
    return suggestion.breed;
};

const BreedAutoComplete = withStyles(autoCompleteStyles)(BreedAutoCompleteComp);
export {
    BreedAutoComplete
};
