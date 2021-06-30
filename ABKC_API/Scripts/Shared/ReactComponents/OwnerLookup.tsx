import React, { Component } from 'react';

import Select from 'react-select';

import { OwnerModel, OwnerLookupInfo } from '../../App/Models/Owner';
import { searchOwnersWithSimpleReturn, getOwnerById } from '../../App/Api/ownerService';
import { Button } from '../../../Vendor/mr-pro/components';

import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import { createStyles } from '@material-ui/core/styles';
import Info from '@material-ui/icons/Info';
import IconButton from '@material-ui/core/IconButton';
import { GridContainer, GridItem } from '../../../Vendor/mr-pro/components/Grid';
import Popover from '@material-ui/core/Popover';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';

const ownerLookupStyle = theme =>
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

export interface OwnerLookupProps extends WithStyles<typeof ownerLookupStyle> {
  handleSelectFn: (selected: OwnerLookupInfo) => void;
  showInfoButton?: boolean;
  showSelectButton?: boolean;
  selectButtonLabel?: string;
  selectedOwnerId: number;
  isReadOnly: boolean;
}

export interface OwnerLookupState {
  matchesLoading: boolean;
  matches: OwnerLookupInfo[];
  selOwner: OwnerModel;
  selOwnerId: number;
  loading: boolean;
  infoOpen: boolean;
  lookupRef: any;
  manuallyChanged: boolean;
}

export default class OwnerLookupComp extends React.Component<OwnerLookupProps, OwnerLookupState> {
  public static defaultProps: Partial<OwnerLookupProps> = {
    showInfoButton: true,
    selectButtonLabel: 'Choose',
    showSelectButton: true,
  };
  public static getDerivedStateFromProps(nextProps: OwnerLookupProps, prevState: OwnerLookupState) {
    const { selOwnerId, manuallyChanged } = prevState;
    const { selectedOwnerId } = nextProps;

    if (!manuallyChanged) {
      if (selectedOwnerId && selOwnerId !== selectedOwnerId) {
        //replace the value and retrieve it from the server
        return {
          selOwnerId: selectedOwnerId,
          selOwner: null,
        };
      }
    }
    return null; //tells react nothing changed
  }
  public state: OwnerLookupState = {
    matchesLoading: false,
    matches: [],
    selOwner: null,
    selOwnerId: -1,
    loading: false,
    infoOpen: false,
    lookupRef: null,
    manuallyChanged: false,
  };
  private lookupRef2: any;

  public constructor(props: OwnerLookupProps) {
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
    const { selOwner, selOwnerId, manuallyChanged } = this.state;
    if (selOwner === null && selOwnerId > -1) {
      // At this point, we're in the "commit" phase, so it's safe to load the new data.
      // if(manuallyChanged) {
      if (prevState.selOwner && prevState.selOwner.ownerId === selOwnerId) {
        this.setState({ selOwner: prevState.selOwner });
      } else {
        this.getSelectedOwner();
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
      selectedOwnerId,
      isReadOnly,
    } = this.props;
    const { matchesLoading, matches, selOwner, infoOpen } = this.state;
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
                value={selOwner}
                getOptionLabel={(option: OwnerLookupInfo) =>
                  option.fullName === '' ? 'No Name Provided' : option.fullName
                }
                getOptionValue={(option: OwnerLookupInfo) => option.ownerId}
                loadingMessage={() => 'retrieving results'}
                noOptionsMessage={obj =>
                  obj.inputValue.length > 2 ? `no owners were found matching ${obj.inputValue}` : ''
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
                disabled={!selOwner}
                onClick={evt =>
                  this.setState({ infoOpen: !infoOpen, lookupRef: evt.currentTarget })
                }
                className={classes.icons}
                aria-label="Info"
              >
                <Info color="primary" />
              </IconButton>
            </GridItem>
          )}
          {showSelectButton &&
            selOwner && (
              <GridItem>
                <Button
                  size="sm"
                  round
                  className={classes.chooseButton}
                  aria-label="Choose"
                  disabled={!selOwner || (selOwner && selOwner.ownerId === selectedOwnerId)}
                  onClick={this.submitSelection}
                >
                  {selectButtonLabel}
                </Button>
              </GridItem>
            )}
        </GridContainer>
        {selOwner && (
          <Popover
            open={infoOpen}
            anchorEl={this.lookupRef2}
            onClose={() => this.setState({ infoOpen: !infoOpen })}
            anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
          >
            <div>
              <Paper>
                <Typography variant="body1" gutterBottom>
                  Name:{selOwner.fullName}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Owner ID:{selOwner.ownerId}
                </Typography>
                <Typography gutterBottom>Email: {selOwner.email}</Typography>
              </Paper>
            </div>
          </Popover>
        )}
      </div>
    );
  }

  private getSelectedOwner() {
    const { selOwnerId, matches, loading } = this.state;
    if (!loading) {
      this.setState({ loading: true }, () => {
        getOwnerById(selOwnerId).then(owner => {
          //if owner is not in matches, put it there
          if (owner) {
            // const index = matches.indexOf(m => m.ownerId === owner.ownerId);
            const index: number = matches.findIndex(m => m.ownerId === owner.ownerId);
            if (index === -1) {
              // const tmpOwner: OwnerLookupInfo = {
              //   firstName: owner.firstName,
              //   fullName: owner.fullName,
              //   lastName: owner.lastName,
              //   id: owner.ownerId,
              //   ownerId: owner.ownerId,
              // };
              matches.splice(0, 0, owner);
            }
          }
          this.setState({ selOwner: owner, matches, loading: false });
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
        searchOwnersWithSimpleReturn(inputValue).then(matches => {
          this.setState({
            matches: matches.data,
            matchesLoading: false,
          });
        });
      });
    }
  }

  private submitSelection() {
    const { handleSelectFn } = this.props;
    const { selOwner } = this.state;
    handleSelectFn(selOwner);
  }

  private handleSelectionChange(selection: OwnerLookupInfo) {
    if (!selection) {
      this.setState({ matches: [], selOwnerId: -1, selOwner: null, manuallyChanged: true });
    } else {
      const { selOwner, selOwnerId } = this.state;
      if (selection.ownerId !== selOwnerId) {
        this.setState({
          matches: [],
          selOwnerId: selection.ownerId,
          selOwner: null,
          manuallyChanged: true,
        });
      }
    }
  }
}

const OwnerLookup = withStyles(ownerLookupStyle)(OwnerLookupComp);
export { OwnerLookup };
