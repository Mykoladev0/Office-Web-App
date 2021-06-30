import React, { Component } from 'react';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';
import qs from 'query-string';

import { OwnerModel } from '../../Models';
import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';
import { Button } from '../../../../Vendor/mr-pro/components';
import Modal from '@material-ui/core/Modal';
import { OwnersTable } from './OwnersTable';
import IndividualOwner from './IndividualOwner';
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import dogsSearchStyle from '../Dogs/DogsSearchStyle';

import { withGetScreen } from '../../../Utilities/GetScreenSize';
import { BeatSpinner } from '../../../Shared/ReactComponents/Spinner';

export interface OwnerSearchProps extends WithStyles<typeof dogsSearchStyle> {
  isMobile?: () => boolean;
  isTablet?: () => boolean;
}
type Props = RouteComponentProps<OwnerSearchProps>;

export interface OwnerSearchState {
  isLoading: boolean;
  toShow: OwnerModel;
  searchText: string;
}

class DogsSearch extends Component<Props, OwnerSearchState> {
  public state: OwnerSearchState = {
    isLoading: false,
    toShow: null,
    searchText: '',
  };

  public constructor(props: OwnerSearchProps) {
    super(props);
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
    const { isLoading, toShow, searchText } = this.state;
    return (
      <div>
        <GridContainer>
          {isLoading ? (
            <BeatSpinner />
          ) : (
            <GridItem xs={12}>
              <OwnersTable
                searchText={searchText}
                showPopupFn={this.handlePopupRequest}
                deleteOwnerFn={owner => {}}
                isMobile={this.props.isMobile}
                isTablet={this.props.isTablet}
              />
            </GridItem>
          )}
          <GridItem xs={12}>
            <GridContainer alignItems={'flex-end'} justify={'flex-end'}>
              <Link to={`/owners/new`}>
                <Button color="primary" className={classes.marginRight}>
                  Add New Owner
                </Button>
              </Link>
            </GridContainer>
          </GridItem>
        </GridContainer>
        {toShow && (
          <Modal
            aria-labelledby={`${toShow.firstName} ${toShow.lastName}`}
            style={{ alignItems: 'center', justifyContent: 'center' }}
            open={true}
            onClose={() => this.setState({ toShow: null })}
          >
            <div style={{ maxWidth: '85%' }}>
              <IndividualOwner
                Owner={toShow}
                IsEditable={false}
                cardTitle={`${toShow.firstName} ${toShow.lastName} Details`}
              />
            </div>
          </Modal>
        )}
      </div>
    );
  }

  private handlePopupRequest(owner: OwnerModel) {
    this.setState({ toShow: owner });
  }
}
export default withRouter(withGetScreen(withStyles(dogsSearchStyle)(DogsSearch)));
