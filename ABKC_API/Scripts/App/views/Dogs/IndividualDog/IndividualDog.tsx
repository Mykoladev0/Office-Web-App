/// <reference path="../../../Models/Dog.d.ts" />

import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import moment from 'moment';

import Pets from '@material-ui/icons/Pets';
import Save from '@material-ui/icons/Save';
import ExitToApp from '@material-ui/icons/ExitToApp';

import { WithStyles, withStyles } from '@material-ui/core/styles';
// tslint:disable:no-implicit-dependencies
import isEqual from 'lodash/fp/isEqual';
import cloneDeep from 'lodash/fp/cloneDeep';

import DateTime from 'react-datetime/DateTime';
import Switch from '@material-ui/core/Switch';

import { GridContainer, GridItem } from '../../../../../Vendor/mr-pro/components/Grid';
import {
  Card,
  CardHeader,
  CardText,
  CardIcon,
  CardBody,
} from '../../../../../Vendor/mr-pro/components/Card';
import { CustomInput, Button } from '../../../../../Vendor/mr-pro/components';

import { GenderDropDown } from '../../../../Shared/ReactComponents/GenderDropdown';
import { ABKCInput } from '../../../../Shared/ReactComponents/ABKCInput';
import { BreedAutoComplete } from '../../../../Shared/ReactComponents/BreedAutoComplete';
import { DogColorInput } from '../../../../Shared/ReactComponents/DogColorInput';
import { DogLookup } from '../../../../Shared/ReactComponents/DogLookup';
import { OwnerLookup } from '../../../../Shared/ReactComponents/OwnerLookup';

import { DogModel } from '../../../Models/Dog';

import { saveDog, getDistinctColors } from '../../../Api/dogService';

// style for this view
import validationFormsStyle from '../../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/views/validationFormsStyle.jsx';

import { BreedModel } from '../../../Models';
import { getBreeds } from '../../../Api/breedService';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import InputLabel from '@material-ui/core/InputLabel';
import FormControl from '@material-ui/core/FormControl';
import FormHelperText from '@material-ui/core/FormHelperText';

///component which shows a dog's information

export interface IndividualDogProps extends WithStyles<typeof validationFormsStyle> {
  Dog?: DogModel;
  IsEditable: boolean;
  cardTitle: string;
}

export interface IndividualDogState {
  ActiveDog: PartialDog;
  breeds: BreedModel[];
  colors: string[];
  isLoaded: boolean;
}

type Props = Partial<any> & IndividualDogProps;
type PartialDog = Partial<DogModel>;

class IndividualDogComponent extends Component<Props, IndividualDogState> {
  public static getDerivedStateFromProps(nextProps: Props, prevState: IndividualDogState) {
    const { ActiveDog } = prevState;
    const { Dog } = nextProps;
    if (!ActiveDog && Dog && !isEqual(ActiveDog, Dog)) {
      return {
        ActiveDog: cloneDeep(Dog),
      };
    }
    if (ActiveDog === null) {
      return {
        id: 0, //seed dog?
      };
    }
    return {
      ActiveDog: ActiveDog,
    };
  }

  public state: IndividualDogState = {
    ActiveDog: null,
    breeds: [],
    colors: [],
    isLoaded: false,
    // canSave: false
  };

  public constructor(props) {
    super(props);
    this.fieldChange = this.fieldChange.bind(this);
    this.fieldChangeFromEvent = this.fieldChangeFromEvent.bind(this);
    this.handleGenderChange = this.handleGenderChange.bind(this);
    this.handleDateChanged = this.handleDateChanged.bind(this);
    this.saveDogHandler = this.saveDogHandler.bind(this);
    this.handleBreedChanged = this.handleBreedChanged.bind(this);
    this.goBack = this.goBack.bind(this);
  }

  public componentDidMount() {
    //fetch breeds
    const { IsEditable } = this.props;
    if (IsEditable) {
      let breedsLoaded: boolean = false;
      let colorsLoaded: boolean = false;
      getBreeds().then(breeds => {
        breedsLoaded = true;
        this.setState({
          breeds,
          isLoaded: colorsLoaded,
        });
      });
      getDistinctColors().then(colors => {
        colorsLoaded = true;
        this.setState({
          colors,
          isLoaded: breedsLoaded,
        });
      });
    } else {
      this.setState({ isLoaded: true });
    }
  }

  public render(): JSX.Element {
    const { classes, cardTitle, IsEditable, Dog } = this.props;
    const { ActiveDog, breeds, colors } = this.state;
    const canSave = this.dogIsValid(ActiveDog) && !isEqual(ActiveDog, Dog);
    let dogNameSuccess: boolean = false;
    // let dogNameError: boolean = false;
    if (ActiveDog) {
      dogNameSuccess = ActiveDog.dogName.length > 3 && IsEditable;
      //dogNameError = ActiveDog.dogName.length > 0 && IsEditable;
    }
    // const tmp = moment(ActiveDog.birthdate, "YYYY-MM-DD");
    return (
      <GridContainer>
        <Card>
          <CardHeader color="primary" icon>
            <CardIcon color="primary">
              <Pets />
            </CardIcon>
            <h4 className={classes.cardIconTitle} id="dogTitle">
              {cardTitle}
            </h4>
          </CardHeader>
          <CardBody>
            <form>
              <GridContainer>
                <GridItem xs={12} sm={12} md={8}>
                  <GridContainer>
                    <GridItem xs={12} sm={6} md={6}>
                      <CustomInput
                        success={dogNameSuccess} //should check for existence!
                        // error={!dogNameSuccess}
                        labelText={'Dog Name *'}
                        id="dogName"
                        formControlProps={{
                          fullWidth: true,
                          disabled: !IsEditable,
                        }}
                        inputProps={{
                          onChange: event => this.fieldChangeFromEvent(event, 'dogName'),
                          type: 'text', //sets the input field type (html5 input types!)
                          value: ActiveDog.dogName,
                          readOnly: !IsEditable,
                        }}
                      />
                    </GridItem>
                    <GridItem xs={12} sm={6} md={6}>
                      <GenderDropDown
                        readonly={!IsEditable}
                        required={true}
                        value={ActiveDog.gender}
                        handleSelectionFn={this.handleGenderChange}
                      />
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      <FormControl fullWidth={true}>
                        <FormHelperText>Birth Date</FormHelperText>
                        <DateTime
                          className="rdtPickerOpenUpwards"
                          dateFormat="YYYY-MM-DD"
                          timeFormat={false}
                          closeOnSelect={true}
                          inputProps={{
                            placeholder: 'Date of Birth *',
                            id: 'birthDate',
                            readOnly: !IsEditable,
                            disabled: !IsEditable,
                          }}
                          value={moment(ActiveDog.birthdate)}
                          onChange={(date: moment.Moment) => {
                            this.handleDateChanged('birthdate', date);
                          }}
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      <FormControl fullWidth={true}>
                        <FormHelperText>Date of Registration</FormHelperText>
                        <DateTime
                          className="rdtPickerOpenUpwards"
                          dateFormat="YYYY-MM-DD"
                          timeFormat={false}
                          closeOnSelect={true}
                          inputProps={{
                            placeholder: 'Date of Registration *',
                            id: 'dateRegistered',
                            readOnly: !IsEditable,
                            disabled: !IsEditable,
                          }}
                          value={moment(ActiveDog.dateRegistered)}
                          onChange={(date: moment.Moment) => {
                            this.handleDateChanged('dateRegistered', date);
                          }}
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      <FormHelperText>Breed</FormHelperText>
                      <FormControl fullWidth={true} required={true} disabled={!IsEditable}>
                        <BreedAutoComplete
                          breeds={breeds}
                          readonly={!IsEditable}
                          value={{ id: 0, breed: ActiveDog.breed }} //temporary, need to build a breedmodel!
                          handleChangeFn={this.handleBreedChanged}
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      <FormControl fullWidth={true} required={true} disabled={!IsEditable}>
                        <FormHelperText>Color</FormHelperText>
                        <DogColorInput
                          colors={colors}
                          value={ActiveDog.color}
                          readonly={!IsEditable}
                          handleChangeFn={color => this.fieldChange(color, 'color')}
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      {/* Sire lookup */}
                      <FormControl fullWidth={true} disabled={!IsEditable}>
                        <FormHelperText>Sire</FormHelperText>
                        <DogLookup
                          showInfoButton={true}
                          showSelectButton={IsEditable}
                          isReadOnly={!IsEditable}
                          selectButtonLabel={'Select'}
                          selectedDogId={ActiveDog.sireNo ? ActiveDog.sireNo : -1}
                          handleSelectFn={sel =>
                            sel
                              ? this.fieldChange(sel.id, 'sireNo')
                              : this.fieldChange(-1, 'sireNo')
                          }
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      {/* Dam lookup */}
                      <FormControl fullWidth={true} disabled={!IsEditable}>
                        <FormHelperText>Dam</FormHelperText>
                        <DogLookup
                          showInfoButton={true}
                          showSelectButton={IsEditable}
                          isReadOnly={!IsEditable}
                          selectButtonLabel={'Select'}
                          selectedDogId={ActiveDog.damNo ? ActiveDog.damNo : -1}
                          handleSelectFn={sel =>
                            sel ? this.fieldChange(sel.id, 'damNo') : this.fieldChange(-1, 'damNo')
                          }
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      {/* Owner lookup */}
                      <FormControl fullWidth={true} disabled={!IsEditable}>
                        <FormHelperText>Owner</FormHelperText>
                        <OwnerLookup
                          showInfoButton={true}
                          showSelectButton={IsEditable}
                          isReadOnly={!IsEditable}
                          selectButtonLabel={'Select'}
                          selectedOwnerId={ActiveDog.ownerId ? ActiveDog.ownerId : -1}
                          handleSelectFn={sel =>
                            sel
                              ? this.fieldChange(sel.ownerId, 'ownerId')
                              : this.fieldChange(-1, 'ownerId')
                          }
                        />
                      </FormControl>
                    </GridItem>
                    <GridItem xs={12} md={6}>
                      {/* CoOwner lookup */}
                      <FormControl fullWidth={true} disabled={!IsEditable}>
                        <FormHelperText>Co-Owner</FormHelperText>
                        <OwnerLookup
                          showInfoButton={true}
                          showSelectButton={IsEditable}
                          isReadOnly={!IsEditable}
                          selectButtonLabel={'Select'}
                          selectedOwnerId={ActiveDog.coOwnerId ? ActiveDog.coOwnerId : -1}
                          handleSelectFn={sel =>
                            sel
                              ? this.fieldChange(sel.ownerId, 'coOwnerId')
                              : this.fieldChange(-1, 'coOwnerId')
                          }
                        />
                      </FormControl>
                    </GridItem>
                  </GridContainer>
                </GridItem>
                {/* <GridItem xs={6} sm={4} md={3} lg={2}>
                            </GridItem> */}
                <GridItem xs={12} sm={4}>
                  <ABKCInput
                    value={ActiveDog.abkcNo}
                    readonly={!IsEditable}
                    changeFn={val => this.fieldChange(val, 'abkcNo')}
                  />
                  <CustomInput
                    labelText={'Litter #'}
                    id="litterNo"
                    formControlProps={{
                      fullWidth: true,
                      disabled: !IsEditable,
                    }}
                    inputProps={{
                      onChange: event => this.fieldChangeFromEvent(event, 'litterNo'),
                      type: 'number', //sets the input field type (html5 input types!)
                      value: ActiveDog.litterNo,
                      readOnly: !IsEditable,
                    }}
                  />
                  <CustomInput
                    labelText={'Chip #'}
                    id="chipNo"
                    formControlProps={{
                      fullWidth: true,
                      disabled: !IsEditable,
                    }}
                    inputProps={{
                      onChange: event => this.fieldChangeFromEvent(event, 'chipNo'),
                      type: 'text', //sets the input field type (html5 input types!)
                      value: ActiveDog.chipNo,
                      readOnly: !IsEditable,
                    }}
                  />
                  <div className={classes.block}>
                    <FormControlLabel
                      disabled={!IsEditable}
                      control={
                        <Switch
                          checked={ActiveDog.saveBully}
                          onChange={() => this.fieldChange(!ActiveDog.saveBully, 'saveBully')}
                          value="saveBully"
                          disabled={!IsEditable}
                          classes={{
                            switchBase: classes.switchBase,
                            checked: classes.switchChecked,
                            icon: classes.switchIcon,
                            iconChecked: classes.switchIconChecked,
                            bar: classes.switchBar,
                          }}
                        />
                      }
                      classes={{
                        label: classes.label,
                      }}
                      label="Save-A-Bully"
                    />
                  </div>
                </GridItem>
                {IsEditable && (
                  <GridItem xs={12} sm={12} md={12}>
                    <GridContainer alignItems={'flex-end'} justify={'flex-end'}>
                      <Button
                        color="primary"
                        className={classes.marginRight}
                        onClick={() => this.goBack(this.props)}
                      >
                        <ExitToApp className={classes.icons} /> Cancel
                      </Button>
                      <Button
                        color="success"
                        disabled={!canSave}
                        type={'submit'}
                        className={classes.marginRight}
                        onClick={this.saveDogHandler}
                      >
                        <Save className={classes.icons} /> Save Changes
                      </Button>
                    </GridContainer>
                  </GridItem>
                )}
              </GridContainer>
            </form>
          </CardBody>
        </Card>
      </GridContainer>
    );
  }

  private goBack(props) {
    this.props.history.goBack();
  }

  private handleDateChanged(field: string, date: any) {
    const { ActiveDog } = this.state;
    let changes: boolean = false;
    if (moment.isMoment(date)) {
      ActiveDog[field] = date.format(); //2015-01-08T00:00:00
      changes = true;
    } else if (date === '') {
      changes = true;
      ActiveDog[field] = '';
    }
    if (changes) {
      this.setState({ ActiveDog });
    }
  }

  private fieldChangeFromEvent(event: any, field: string): void {
    this.fieldChange(event.target.value, field);
  }
  private fieldChange(value: any, field: string): void {
    const { ActiveDog } = this.state;
    ActiveDog[field] = value;
    this.setState({ ActiveDog });
  }
  private handleGenderChange(newGender: any): void {
    const { ActiveDog } = this.state;
    ActiveDog.gender = newGender;
    this.setState({ ActiveDog });
  }
  private dogIsValid(dog: Partial<DogModel>): boolean {
    if (!dog) {
      return false;
    }
    if (!dog.dogName || dog.dogName.length < 3) {
      return false;
    }
    if (!dog.abkcNo || dog.abkcNo.length !== 7) {
      return false;
    }
    const birthDateValid = moment(dog.birthdate).isValid();
    if (!dog.birthdate && (dog.birthdate !== '' || !birthDateValid)) {
      return false;
    }
    return true;
  }

  private saveDogHandler(e: any): void {
    e.preventDefault();
    const { ActiveDog } = this.state;
    ActiveDog.modifiedBy = 'CURRENT USER HERE';
    saveDog(ActiveDog as DogModel).then(data => {
      //redirect to dashboard or where we came from?
      // this.setState(() => ({
      //     ActiveDog: data
      //   }));
      //alert the user the save was successful!
      this.goBack(this.props);
      // const {history} = this.props;
      // this.props.history.push(history[history.length - 1]);
    });
  }
  private handleBreedChanged(breed: BreedModel) {
    const { ActiveDog } = this.state;
    ActiveDog.breed = breed.breed; //will replace with complex object at some point
    this.setState({ ActiveDog });
  }
}

const IndividualDog = withRouter(withStyles(validationFormsStyle)(IndividualDogComponent));
export { IndividualDog };
