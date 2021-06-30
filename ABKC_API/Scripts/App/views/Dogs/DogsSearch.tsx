import React, { Component, isValidElement } from 'react';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';
import qs from 'query-string';

import { getDogsTableData } from '../../Api/dogService';
import { DogModel } from '../../Models';
import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';
import { Button } from '../../../../Vendor/mr-pro/components';
import Modal from '@material-ui/core/Modal';
import { DogsTable } from './DogsTable';
import { IndividualDog } from './IndividualDog/IndividualDog';
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import dogsSearchStyle from './DogsSearchStyle';

import { withGetScreen } from '../../../Utilities/GetScreenSize';
import { BeatSpinner } from '../../../Shared/ReactComponents/Spinner';

export interface DogsSearchProps extends WithStyles<typeof dogsSearchStyle> {
  isMobile?: () => boolean;
  isTablet?: () => boolean;
}
type Props = RouteComponentProps<DogsSearchProps>;

export interface DogsSearchState {
  dogList: DogModel[];
  isLoading: boolean;
  totalCount: number;
  dogToShow: DogModel;
  searchText: string;
}

class DogsSearch extends React.Component<Props, DogsSearchState> {
  public state: DogsSearchState = {
    dogList: [],
    isLoading: false,
    totalCount: 0,
    dogToShow: null,
    searchText: '',
  };

  public constructor(props: DogsSearchProps) {
    super(props);
    // this.handleRequestData = this.handleRequestData.bind(this);
    this.handlePopupRequest = this.handlePopupRequest.bind(this);
  }

  public componentDidMount() {
    const qsParams = qs.parse(this.props.location.search);
    if (qsParams['searchText']) {
      this.setState({ searchText: qsParams['searchText'] });
    }
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const { isLoading, dogToShow, searchText } = this.state;
    return (
      <div>
        <GridContainer>
          {isLoading ? (
            <BeatSpinner />
          ) : (
            <GridItem xs={12}>
              {/* <DogsTable
                data={dogList}
                totalCount={totalCount}
                fetchDataFn={this.handleRequestData}
                showPopupFn={this.handlePopupRequest}
                {...classes}
                isMobile={this.props.isMobile}
                isTablet={this.props.isTablet}
                isLoading={isLoading}
              /> */}
              <DogsTable
                searchText={searchText}
                showPopupFn={this.handlePopupRequest}
                deleteDogFn={dog => {}}
                isMobile={this.props.isMobile}
                isTablet={this.props.isTablet}
              />
            </GridItem>
          )}
          <GridItem xs={12}>
            <GridContainer alignItems={'flex-end'} justify={'flex-end'}>
              <Link to={`/dogs/new`}>
                <Button color="primary" className={classes.marginRight}>
                  Add New Dog
                </Button>
              </Link>
            </GridContainer>
          </GridItem>
        </GridContainer>
        {dogToShow && (
          <Modal
            aria-labelledby="dogTitle"
            style={{ alignItems: 'center', justifyContent: 'center' }}
            open={true}
            onClose={() => this.setState({ dogToShow: null })}
          >
            <div style={{ maxWidth: '85%' }}>
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

  private handleRequestData(page: number, pageSize: number, sorted: any, filtered: any) {
    this.setState({ isLoading: true }, () => {
      getDogsTableData(page + 1, pageSize, sorted, filtered).then(({ data, count }) => {
        // Now just get the rows of data to your React Table (and update anything else like total pages or loading)
        this.setState({
          dogList: data,
          isLoading: false,
          totalCount: count,
        });
      });
    });
  }

  private handlePopupRequest(dog: DogModel) {
    this.setState({ dogToShow: dog });
  }
}
export default withRouter(withGetScreen(withStyles(dogsSearchStyle)(DogsSearch)));
