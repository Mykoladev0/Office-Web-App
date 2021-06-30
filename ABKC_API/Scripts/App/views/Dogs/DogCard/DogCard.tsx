import React, { Component } from 'react';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';

import ViewList from '@material-ui/icons/ViewList';

import Pets from '@material-ui/icons/Pets';

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

import { getAllDogsCount, searchDogsWithSimpleReturn, getDogById } from '../../../Api/dogService';
import { BasicDogLookupInfo, DogModel } from '../../../Models/Dog';
import Modal from '@material-ui/core/Modal';
import { IndividualDog } from '../IndividualDog/IndividualDog';

export interface DogCardProps extends WithStyles<typeof CardStyle> {}
export type Props = RouteComponentProps<DogCardProps>;
export interface DogCardState {
  dogCount: number;
  isLoading: boolean;
  searchResults: BasicDogLookupInfo[];
  searchActive: boolean;
  showPopup: boolean;
  dogToShow: DogModel;
}

class DogCardComp extends React.Component<Props, DogCardState> {
  public state: DogCardState = {
    dogCount: 0,
    isLoading: true,
    searchResults: null,
    searchActive: false,
    showPopup: false,
    dogToShow: null,
  };

  public constructor(props: DogCardProps) {
    super(props);
    this.handleSearchClick = this.handleSearchClick.bind(this);
    this.handlePopupViewClick = this.handlePopupViewClick.bind(this);
  }
  public componentDidMount() {
    getAllDogsCount().then(dogCount => this.setState({ dogCount, isLoading: false }));
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const { dogCount, isLoading, searchResults, searchActive, showPopup, dogToShow } = this.state;
    return (
      <div>
        <Card>
          <CardHeader color="warning" stats icon>
            <CardIcon color="warning">
              <Pets />
            </CardIcon>
            <p className={classes.cardCategory}>Registered Dogs</p>
            <h3 className={classes.cardTitle}>{isLoading ? '' : dogCount.toLocaleString()}</h3>
          </CardHeader>
          <CardBody>
            {isLoading ? (
              <BeatSpinner />
            ) : (
              <div>
                <SearchInput
                  handleSearchFn={this.handleSearchClick}
                  placeHolder="Quick Dog Lookup"
                  showJumpSearchButton={true}
                />
                {searchActive ? (
                  <BeatSpinner />
                ) : (
                  searchResults && (
                    <SmallResultsList
                      data={searchResults}
                      handleEditFn={item => {
                        this.props.history.push(`/dogs/${item.id}/edit`);
                      }}
                      handlePopupFn={this.handlePopupViewClick}
                      displayField={'dogName'}
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
              <Link to="/Dogs">View All Dogs</Link>
            </div>
          </CardFooter>
        </Card>
        {showPopup &&
          dogToShow && (
            <Modal
              aria-labelledby="dogTitle"
              className={classes.modalStyle}
              //   style={{ alignItems: 'center', justifyContent: 'center' }}
              open={showPopup}
              onClose={() => this.setState({ showPopup: false, dogToShow: null })}
            >
              <div style={{ maxWidth: '75%' }}>
                <IndividualDog
                  Dog={dogToShow}
                  IsEditable={false}
                  cardTitle={`${dogToShow.dogName} Details`}
                />
              </div>
            </Modal>
          )}
      </div>
    );
  }

  private handlePopupViewClick(item: any) {
    //get dog
    getDogById(item.id).then(dog => {
      this.setState({ dogToShow: dog, showPopup: true });
    });
    //set state to showPopup and dogToShow
  }

  private handleSearchClick(searchText: string, jumpToSearch: boolean) {
    //what do we want here?  inline results or jump to search page?
    if (!jumpToSearch) {
      //get results
      this.setState({ searchActive: true }, () => {
        searchDogsWithSimpleReturn(searchText).then(result => {
          this.setState({ searchResults: result.data, searchActive: false });
        });
      });
    } else {
      this.props.history.push(`/dogs/?searchText=${searchText}`);
    }
  }
}

export default withRouter(withStyles(CardStyle)(DogCardComp));
