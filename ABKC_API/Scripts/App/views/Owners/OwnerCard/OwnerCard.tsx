import React, { Component } from 'react';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';

import ViewList from '@material-ui/icons/ViewList';

import PeopleOutline from '@material-ui/icons/PeopleOutline';

import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import { Primary } from '../../../../../Vendor/mr-pro/components/Typography';
import {
  Card,
  CardHeader,
  CardIcon,
  CardBody,
  CardFooter,
} from '../../../../../Vendor/mr-pro/components/Card';

import { BeatSpinner } from '../../../../Shared/ReactComponents/Spinner';
import SearchInput from '../../../../Shared/ReactComponents/SearchInput';
import SmallResultsList from '../../../../Shared/ReactComponents/SmallResultsList';

import CardStyle from '../../../../Shared/Styles/CardStyle';

import Modal from '@material-ui/core/Modal';
import {
  getOwnersCount,
  getOwnerById,
  searchOwnersWithSimpleReturn,
} from '../../../Api/ownerService';
import { OwnerModel } from '../../../Models';
import { OwnerLookupInfo } from '../../../Models/Owner';
import IndividualOwner from '../IndividualOwner';

export interface CardProps extends WithStyles<typeof CardStyle> {}
export type Props = RouteComponentProps<CardProps>;
export interface CardState {
  ownerCount: number;
  isLoading: boolean;
  searchResults: OwnerLookupInfo[];
  searchActive: boolean;
  showPopup: boolean;
  ownerToShow: OwnerModel;
}

class OwnerCardComp extends React.Component<Props, CardState> {
  public state: CardState = {
    ownerCount: 0,
    isLoading: true,
    searchResults: null,
    searchActive: false,
    showPopup: false,
    ownerToShow: null,
  };

  public constructor(props: CardProps) {
    super(props);
    this.handleSearchClick = this.handleSearchClick.bind(this);
    this.handlePopupViewClick = this.handlePopupViewClick.bind(this);
  }
  public componentDidMount() {
    getOwnersCount().then(ownerCount => this.setState({ ownerCount, isLoading: false }));
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const {
      ownerCount,
      isLoading,
      searchResults,
      searchActive,
      showPopup,
      ownerToShow,
    } = this.state;
    return (
      <div>
        <Card>
          <CardHeader color="success" stats icon>
            <CardIcon color="success">
              <PeopleOutline />
            </CardIcon>
            <p className={classes.cardCategory}>Registered Owners</p>
            <h3 className={classes.cardTitle}>{isLoading ? '' : ownerCount.toLocaleString()}</h3>
          </CardHeader>
          <CardBody>
            {isLoading ? (
              <BeatSpinner />
            ) : (
              <div>
                <SearchInput
                  handleSearchFn={this.handleSearchClick}
                  placeHolder="Quick Owner Lookup"
                  showJumpSearchButton={true}
                />
                {searchActive ? (
                  <BeatSpinner />
                ) : (
                  searchResults && (
                    <SmallResultsList
                      data={searchResults}
                      handleEditFn={(item: OwnerLookupInfo) => {
                        this.props.history.push(`/owners/${item.ownerId}/edit`);
                      }}
                      handlePopupFn={this.handlePopupViewClick}
                      displayField={'fullName'}
                    />
                  )
                )}
              </div>
            )}
          </CardBody>
          <CardFooter>
            <div className={classes.stats}>
              <Primary>
                <ViewList />
              </Primary>
              <Link to="/Owners">View All Owners</Link>
            </div>
          </CardFooter>
        </Card>
        {showPopup &&
          ownerToShow && (
            <Modal
              className={classes.modalStyle}
              //   style={{ alignItems: 'center', justifyContent: 'center' }}
              open={showPopup}
              onClose={() => this.setState({ showPopup: false, ownerToShow: null })}
            >
              <div style={{ maxWidth: '75%' }}>
                <IndividualOwner
                  Owner={ownerToShow}
                  IsEditable={false}
                  cardTitle={`${ownerToShow.fullName} Details`}
                />
              </div>
            </Modal>
          )}
      </div>
    );
  }

  private handlePopupViewClick(item: OwnerLookupInfo) {
    getOwnerById(item.ownerId).then(owner => {
      this.setState({ ownerToShow: owner, showPopup: true });
    });
    //set state to showPopup and dogToShow
  }

  private handleSearchClick(searchText: string, jumpToSearch: boolean) {
    //what do we want here?  inline results or jump to search page?
    if (!jumpToSearch) {
      //get results
      this.setState({ searchActive: true }, () => {
        searchOwnersWithSimpleReturn(searchText).then(result => {
          this.setState({ searchResults: result.data, searchActive: false });
        });
      });
    } else {
      this.props.history.push(`/owners/?searchText=${searchText}`);
    }
  }
}

export default withRouter(withStyles(CardStyle)(OwnerCardComp));
