import React, { Component } from 'react';

import Select from 'react-select';

import { DogModel, BasicDogLookupInfo } from '../../App/Models/Dog';
import { searchDogsWithSimpleReturn, getDogById } from '../../App/Api/dogService';
import { Button } from '../../../Vendor/mr-pro/components';

import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import { createStyles } from '@material-ui/core/styles';
import Info from '@material-ui/icons/Info';
import IconButton from '@material-ui/core/IconButton';
import { GridContainer, GridItem } from '../../../Vendor/mr-pro/components/Grid';
import Popover from '@material-ui/core/Popover';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';

const dogLookupStyle = theme =>
  createStyles({
    cardTitle: {
      marginTop: '0',
      marginBottom: '3px',
      color: '#3C4858',
      fontSize: '18px',
    },
    cardHeader: {
      zIndex: 3,
    },
    cardContentLeft: {
      padding: '15px 20px 15px 0px',
      position: 'relative',
    },
    cardContentRight: {
      padding: '15px 20px 15px 0px',
      position: 'relative',
    },
    cardContentBottom: {
      padding: '15px 0px 0px 0px',
      position: 'relative',
    },
    marginRight: {
      marginRight: '5px',
    },
    icons: {
      marginLeft: '-10px',
      width: '17px',
      height: '17px',
    },
    socialButtonsIcons: {
      fontSize: '18px',
      marginTop: '-2px',
      position: 'relative',
    },
    chooseButton: {
      margin: theme.spacing.unit,
    },
  });

export interface DogLookupProps extends WithStyles<typeof dogLookupStyle> {
  handleSelectFn: (selected: BasicDogLookupInfo) => void;
  showInfoButton?: boolean;
  showSelectButton?: boolean;
  selectButtonLabel?: string;
  selectedDogId: number;
  isReadOnly: boolean;
}

export interface DogLookupState {
  matchesLoading: boolean;
  matches: any[];
  selDog: DogModel;
  selDogId: number;
  loadingDog: boolean;
  dogInfoOpen: boolean;
  lookupRef: any;
  manuallyChanged: boolean;
}

export default class DogLookupComp extends React.Component<DogLookupProps, DogLookupState> {
  public static defaultProps: Partial<DogLookupProps> = {
    showInfoButton: true,
    selectButtonLabel: 'Choose',
    showSelectButton: true,
  };
  public static getDerivedStateFromProps(nextProps: DogLookupProps, prevState: DogLookupState) {
    const { selDogId, manuallyChanged } = prevState;
    const { selectedDogId } = nextProps;
    //!(selDogId && selDogId > -1) ||
    if (!manuallyChanged) {
      if (selectedDogId && selDogId !== selectedDogId) {
        //replace the value and retrieve it from the server
        return {
          selDogId: selectedDogId,
          selDog: null,
        };
      }
    }
    return null; //tells react nothing changed
  }
  public state: DogLookupState = {
    matchesLoading: false,
    matches: [],
    selDog: null,
    selDogId: -1,
    loadingDog: false,
    dogInfoOpen: false,
    lookupRef: null,
    manuallyChanged: false,
  };
  private lookupRef2: any;

  public constructor(props: DogLookupProps) {
    super(props);
    // this.lookupRef = React.createRef();
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleSelectionChange = this.handleSelectionChange.bind(this);
    this.submitSelection = this.submitSelection.bind(this);
  }
  public componentDidMount() {
    //   const {selDog, selDogId} = this.state;
    //   let shouldFetch: boolean = false;
    //   if(selDog == null && selDogId > -1) {
    //       //fetch
    //       shouldFetch = true;
    //   } else if(selDog && selDogId !== selDog.id) {
    //       //fetch
    //       shouldFetch = true;
    //   }
    //   if(shouldFetch) {
    //       this.getSelectedDog();
    // }
  }

  public componentDidUpdate(prevProps, prevState) {
    const { selDog, selDogId, manuallyChanged } = this.state;
    if (selDog === null && selDogId > -1) {
      // At this point, we're in the "commit" phase, so it's safe to load the new data.
      // if(manuallyChanged) {
      if (prevState.selDog && prevState.selDog.id === selDogId) {
        this.setState({ selDog: prevState.selDog });
      } else {
        this.getSelectedDog();
      }
      // }
    }
  }

  public render(): JSX.Element {
    const {
      showInfoButton,
      showSelectButton,
      classes,
      selectButtonLabel,
      selectedDogId,
      isReadOnly,
    } = this.props;
    const { matchesLoading, matches, selDog, dogInfoOpen } = this.state;
    return (
      <div>
        <GridContainer alignItems={'stretch'} justify={'flex-start'}>
          <GridItem xs={8}>
            <div ref={c => (this.lookupRef2 = c)}>
              <Select
                isLoading={matchesLoading}
                options={matches}
                isDisabled={isReadOnly}
                // isOptionSelected = {(option, options) => {
                //     return option.id === this.props.selectedDogId;
                // }}
                value={selDog}
                getOptionLabel={(option: BasicDogLookupInfo) =>
                  option.dogName === '' ? 'No Name Provided' : option.dogName
                }
                getOptionValue={(option: BasicDogLookupInfo) => option.id}
                loadingMessage={() => 'retrieving results'}
                noOptionsMessage={obj =>
                  obj.inputValue.length > 2 ? `no dogs were found matching ${obj.inputValue}` : ''
                }
                placeholder={'type name or id to begin'}
                onInputChange={this.handleInputChange}
                backspaceRemovesValue={true}
                isClearable={true}
                closeMenuOnSelect={true}
                onChange={this.handleSelectionChange}
                menuPlacement={'auto'}
                maxMenuHeight={200}
                // ref={(node) => {this.lookupRef = node;}}
              />
            </div>
          </GridItem>
          {showInfoButton && (
            <GridItem xs={1} style={{ marginLeft: '-10px', marginRight: '-10px' }}>
              <IconButton
                disabled={!selDog}
                onClick={evt =>
                  this.setState({ dogInfoOpen: !dogInfoOpen, lookupRef: evt.currentTarget })
                }
                className={classes.icons}
                aria-label="Info"
              >
                <Info color="primary" />
              </IconButton>
            </GridItem>
          )}
          {showSelectButton &&
            selDog && (
              <GridItem>
                <Button
                  size="sm"
                  round
                  className={classes.chooseButton}
                  aria-label="Choose"
                  disabled={!selDog || (selDog && selDog.id === selectedDogId)}
                  onClick={this.submitSelection}
                >
                  {selectButtonLabel}
                </Button>
              </GridItem>
            )}
        </GridContainer>
        {selDog && (
          <Popover
            open={dogInfoOpen}
            anchorEl={this.lookupRef2}
            onClose={() => this.setState({ dogInfoOpen: !dogInfoOpen })}
            anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
          >
            <div>
              <Paper>
                <Typography variant="body1" gutterBottom>
                  Name:{selDog.dogName}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Bully ID:{selDog.bullyId}
                </Typography>
                <Typography gutterBottom>ABKC #: {selDog.abkcNo}</Typography>
              </Paper>
            </div>
          </Popover>
        )}
      </div>
    );
  }

  private getSelectedDog() {
    const { selDogId, matches, loadingDog } = this.state;
    if (!loadingDog) {
      this.setState({ loadingDog: true }, () => {
        getDogById(selDogId).then(dog => {
          //if dog is not in matches, put it there
          if (dog) {
            const index = matches.indexOf(m => m.id === dog.id);
            if (index === -1) {
              matches.splice(0, 0, dog);
            }
          }
          this.setState({ selDog: dog, matches, loadingDog: false });
          // this.state.selDog = dog;
        });
      });
    }
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
    }
  }

  private submitSelection() {
    const { handleSelectFn } = this.props;
    const { selDog } = this.state;
    handleSelectFn(selDog);
  }

  private handleSelectionChange(selection: BasicDogLookupInfo) {
    if (!selection) {
      this.setState({ matches: [], selDogId: -1, selDog: null, manuallyChanged: true });
    } else {
      const { selDog, selDogId } = this.state;
      if (selection.id !== selDogId) {
        this.setState({ matches: [], selDogId: selection.id, selDog: null, manuallyChanged: true });
      }
      //     this.setState({matches:[], selDogId:-1, selDog:null}, () => {
      //         // this.props.handleSelectFn(null);
      //         return {
      //             matches:[],
      //             selDogId: -1,
      //             selDog: null,
      //             manuallyChanged: true
      //         };
      //     });
      //   } else {
      //       const {selDog, selDogId} = this.state;
      //       if(selection.id !== selDogId) {
      //         this.setState(() => {
      //             // this.props.handleSelectFn(selection);
      //             return {
      //                 selDogId: selection.id,
      //                 selDog: null, //let props update it?
      //                 manuallyChanged: true
      //             };
      //         });
      //     }
    }
  }
}

const DogLookup = withStyles(dogLookupStyle)(DogLookupComp);
export { DogLookup };
