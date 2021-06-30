import React, { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import isEqual from 'lodash/fp/isEqual';
import cloneDeep from 'lodash/fp/cloneDeep';
import ReactPhoneInput from 'react-phone-input-2';
import Address from '@react-ag-components/address';
import SweetAlert from 'react-bootstrap-sweetalert';

import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import { WithStyles, withStyles } from '@material-ui/core/styles';

import IndividualStyle from './IndividualStyle';
import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';
import {
  Card,
  CardHeader,
  CardText,
  CardIcon,
  CardBody,
  CardFooter,
} from '../../../../Vendor/mr-pro/components/Card';

import PeopleOutline from '@material-ui/icons/PeopleOutline';

import { OwnerModel } from '../../Models';
import { CustomInput, Button } from '../../../../Vendor/mr-pro/components';
import FormControl from '../../../../node_modules/@material-ui/core/FormControl';
import FormHelperText from '../../../../node_modules/@material-ui/core/FormHelperText';
import Save from '../../../../node_modules/@material-ui/icons/Save';
import ExitToApp from '../../../../node_modules/@material-ui/icons/ExitToApp';
import { saveOwner } from '../../Api/ownerService';

export interface IndividualOwnerProps extends WithStyles<typeof IndividualStyle> {
  Owner: OwnerModel;
  IsEditable: boolean;
  cardTitle: string;
}

export interface IndividualOwnerState {
  ownerDirty: OwnerModel;
  dialogMsg: JSX.Element;
}
type Props = IndividualOwnerProps & RouteComponentProps;
export default withRouter(
  withStyles(IndividualStyle)(
    class IndividualOwner extends Component<Props, IndividualOwnerState> {
      public static getDerivedStateFromProps(
        nextProps: IndividualOwnerProps,
        prevState: IndividualOwnerState
      ) {
        const { ownerDirty } = prevState;
        const { Owner } = nextProps;
        if (!ownerDirty && Owner && !isEqual(ownerDirty, Owner)) {
          return {
            ownerDirty: cloneDeep(Owner),
          };
        }
        if (ownerDirty === null) {
          return {
            id: 0, //seed owner?
          };
        }
        return {
          ownerDirty,
        };
      }
      public state: IndividualOwnerState = {
        ownerDirty: null,
        dialogMsg: null,
      };

      public constructor(props: IndividualOwnerProps) {
        super(props);
        this.fieldChangeFromEvent = this.fieldChangeFromEvent.bind(this);
        this.fieldChange = this.fieldChange.bind(this);
        this.addressChange = this.addressChange.bind(this);
        this.goBack = this.goBack.bind(this);
        this.saveDirty = this.saveDirty.bind(this);
        this.handleConfirmationClose = this.handleConfirmationClose.bind(this);
      }

      public render(): JSX.Element {
        const { IsEditable } = this.props;
        const { ownerDirty, dialogMsg } = this.state;
        const curAddress = {
          addressLine1: ownerDirty.address1,
          addressLine2: ownerDirty.address2,
          addressLine3: ownerDirty.address3,
          suburb: ownerDirty.city,
          state: ownerDirty.state,
          postcode: ownerDirty.zip,
          country: ownerDirty.country && ownerDirty.country !== '' ? ownerDirty.country : 'US',
        };
        const canSave: boolean = IsEditable && !isEqual(ownerDirty, this.props.Owner);
        return (
          <Card>
            <CardHeader color="primary" icon>
              <CardIcon color="primary">
                <PeopleOutline />
              </CardIcon>
            </CardHeader>
            <CardBody>
              {dialogMsg}
              <form>
                <GridContainer>
                  <GridItem xs={4} md={4}>
                    <CustomInput
                      // success={dogNameSuccess} //should check for existence!
                      // error={!dogNameSuccess}
                      labelText={'First Name *'}
                      id="firstName"
                      formControlProps={{
                        fullWidth: true,
                        disabled: !IsEditable,
                      }}
                      inputProps={{
                        onChange: event => this.fieldChangeFromEvent(event, 'firstName'),
                        type: 'text', //sets the input field type (html5 input types!)
                        value: ownerDirty.firstName,
                        readOnly: !IsEditable,
                      }}
                    />
                  </GridItem>
                  <GridItem xs={4} md={2}>
                    <CustomInput
                      // success={dogNameSuccess} //should check for existence!
                      // error={!dogNameSuccess}
                      labelText={'Middle Name'}
                      id="middleInitial"
                      formControlProps={{
                        fullWidth: true,
                        disabled: !IsEditable,
                      }}
                      inputProps={{
                        onChange: event => this.fieldChangeFromEvent(event, 'middleInitial'),
                        type: 'text', //sets the input field type (html5 input types!)
                        value: ownerDirty.middleInitial ? ownerDirty.middleInitial : '',
                        readOnly: !IsEditable,
                      }}
                    />
                  </GridItem>
                  <GridItem xs={4} md={4}>
                    <CustomInput
                      // success={dogNameSuccess} //should check for existence!
                      // error={!dogNameSuccess}
                      labelText={'Last Name *'}
                      id="lastName"
                      formControlProps={{
                        fullWidth: true,
                        disabled: !IsEditable,
                      }}
                      inputProps={{
                        onChange: event => this.fieldChangeFromEvent(event, 'lastName'),
                        type: 'text', //sets the input field type (html5 input types!)
                        value: ownerDirty.lastName,
                        readOnly: !IsEditable,
                      }}
                    />
                  </GridItem>
                  <GridItem xs={6} sm={4} md={4}>
                    <FormControl fullWidth={true}>
                      <FormHelperText>Telephone *</FormHelperText>
                      <ReactPhoneInput
                        defaultCountry={'us'}
                        required={true}
                        disabled={!IsEditable}
                        inputClass={'CustomInput'}
                        placeholder={'Enter home telephone'}
                        inputStyle={{
                          width: '100%',
                          border: 'none',
                          borderBottomStyle: 'solid',
                          borderBottomWidth: '1px',
                          borderRadius: '0px',
                          lineHeight: '1.1875em',
                          fontSize: '1rem',
                        }}
                        buttonStyle={{
                          backgroundColor: 'white',
                          border: 'none',
                          borderBottomStyle: 'solid',
                          borderBottomWidth: '1px',
                          borderRadius: '0px',
                        }}
                        value={ownerDirty.phone ? ownerDirty.phone : ''}
                        onChange={val => this.fieldChange(val, 'phone')}
                      />
                    </FormControl>
                  </GridItem>
                  <GridItem xs={6} sm={4} md={4}>
                    <FormControl fullWidth={true}>
                      <FormHelperText>Cell-Phone</FormHelperText>
                      <ReactPhoneInput
                        defaultCountry={'us'}
                        required={true}
                        placeholder={'Cell Phone'}
                        disabled={!IsEditable}
                        inputClass={'CustomInput'}
                        inputStyle={{
                          width: '100%',
                          border: 'none',
                          borderBottomStyle: 'solid',
                          borderBottomWidth: '1px',
                          borderRadius: '0px',
                          lineHeight: '1.1875em',
                          fontSize: '1rem',
                        }}
                        buttonStyle={{
                          backgroundColor: 'white',
                          border: 'none',
                          borderBottomStyle: 'solid',
                          borderBottomWidth: '1px',
                          borderRadius: '0px',
                        }}
                        value={ownerDirty.cell ? ownerDirty.cell : ''}
                        onChange={val => this.fieldChange(val, 'cell')}
                      />
                    </FormControl>
                  </GridItem>
                  <GridItem xs={6} sm={4} md={4}>
                    <CustomInput
                      labelText={'Email *'}
                      id="email"
                      formControlProps={{
                        fullWidth: true,
                        disabled: !IsEditable,
                      }}
                      inputProps={{
                        onChange: event => this.fieldChangeFromEvent(event, 'email'),
                        type: 'email', //sets the input field type (html5 input types!)
                        value: ownerDirty.email ? ownerDirty.email : '',
                        readOnly: !IsEditable,
                      }}
                    />
                  </GridItem>
                  <GridItem xs={12} sm={10} md={8}>
                    <MuiThemeProvider>
                      <Address
                        countryUrl={'/wwwroot/static/'}
                        countryType={'countries.json'}
                        value={curAddress}
                        label={'Home Address'}
                        required={true}
                        onChange={address => this.addressChange}
                      />
                    </MuiThemeProvider>
                  </GridItem>
                </GridContainer>
              </form>
            </CardBody>
            <CardFooter>
              {IsEditable && (
                <GridContainer alignItems={'flex-end'} justify={'flex-end'}>
                  <Button
                    color="primary"
                    // className={classes.marginRight}
                    onClick={() => this.goBack()}
                  >
                    {/* className={classes.icons} */}
                    <ExitToApp /> Cancel
                  </Button>
                  <Button
                    color="success"
                    disabled={!canSave}
                    type={'submit'}
                    // className={classes.marginRight}
                    onClick={this.saveDirty}
                  >
                    {/* className={classes.icons} */}
                    <Save /> Save Changes
                  </Button>
                </GridContainer>
              )}
            </CardFooter>
          </Card>
        );
      }
      private saveDirty(e: any): void {
        e.preventDefault();
        const { ownerDirty } = this.state;
        ownerDirty.modifiedBy = 'CURRENT USER HERE';
        const title: string = `Owner Save Successful`;
        saveOwner(ownerDirty as OwnerModel)
          .then(data => {
            //alert the user the save was successful!
            this.setState({
              dialogMsg: (
                <SweetAlert
                  success
                  style={{ display: 'block', marginTop: '-100px' }}
                  title={title}
                  onConfirm={() => this.handleConfirmationClose()}
                  onCancel={() => this.handleConfirmationClose()}
                  confirmBtnCssClass={this.props.classes.button + ' ' + this.props.classes.success}
                >
                  The save was successful!
                </SweetAlert>
              ),
            });
          })
          .catch(r => {
            //alert user there was problem with save
            let errorStr: string = 'Error Saving!';
            if (Array.isArray(r.data)) {
              errorStr = r.data.join('\n');
            } else {
              errorStr = 'Could not save because of the following problems: \r\n';
              // tslint:disable-next-line:forin
              for (const msg in r.data) {
                errorStr = errorStr + r.data[msg] + ' \r\n';
              }
            }
            this.setState({
              dialogMsg: (
                <SweetAlert
                  warning
                  style={{ display: 'block', marginTop: '-100px' }}
                  title={'Error Saving Owner!'}
                  onConfirm={() => this.setState({ dialogMsg: null })}
                  onCancel={() => this.setState({ dialogMsg: null })}
                  confirmBtnCssClass={this.props.classes.button + ' ' + this.props.classes.success}
                  confirmBtnText="Okay"
                >
                  {errorStr}
                </SweetAlert>
              ),
            });
          });
      }
      private handleConfirmationClose() {
        this.setState(
          {
            dialogMsg: null,
          },
          () => this.goBack()
        );
      }
      private goBack() {
        const { history } = this.props;
        history.goBack();
      }

      private addressChange(address: any): void {
        const { ownerDirty } = this.state;
        ownerDirty.address1 = address.addressLine1;
        ownerDirty.address2 = address.addressLine2;
        ownerDirty.address3 = address.addressLine3;
        ownerDirty.country = address.country;
        ownerDirty.state = address.state;
        ownerDirty.city = address.suburb;
        this.setState({ ownerDirty });
      }
      private fieldChangeFromEvent(event: any, field: string): void {
        this.fieldChange(event.target.value, field);
      }
      private fieldChange(value: any, field: string): void {
        const { ownerDirty } = this.state;
        ownerDirty[field] = value;
        this.setState({ ownerDirty });
      }
    }
  )
);
