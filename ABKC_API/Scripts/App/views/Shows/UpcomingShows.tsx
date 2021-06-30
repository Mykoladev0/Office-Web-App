import React, { Component } from 'react';
import moment from 'moment';
import { Card, CardHeader, CardBody } from '../../../../Vendor/mr-pro/components/Card';
import { Table } from '../../../../Vendor/mr-pro/components';
import { getUpcomingShows } from '../../Api/showService';
import { ShowLookupInfo } from '../../Models/Shows';
import { BeatSpinner } from '../../../Shared/ReactComponents/Spinner';

export interface UpcomingShowsProps {}

export interface UpcomingShowsState {
  upcomingShows: ShowLookupInfo[];
}

export default class UpcomingShows extends Component<any, UpcomingShowsState> {
  public state: UpcomingShowsState = {
    upcomingShows: [],
  };

  public constructor(props: UpcomingShowsProps) {
    super(props);
  }
  public componentDidMount() {
    //get upcoming shows
    getUpcomingShows(4).then(data => this.setState({ upcomingShows: data.data }));
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const { upcomingShows } = this.state;
    let tableData = [];
    if (upcomingShows) {
      tableData = upcomingShows.map(d => {
        return [d.showName, moment(d.showDate).format('MM/DD/YYYY'), d.numberBreeds];
      });
    }
    return (
      <Card>
        <CardHeader color="warning">
          <h4 className={classes.cardTitleWhite}>Upcoming Shows</h4>
          <p className={classes.cardCategoryWhite}>Shows scheduled soon</p>
        </CardHeader>
        {!upcomingShows ? (
          <BeatSpinner />
        ) : (
          tableData && (
            <CardBody>
              {/* Need to rework table to support more complex data */}
              <Table
                tableHeaderColor="warning"
                tableHead={['Name', 'Date', '# Breeds']}
                tableData={tableData}
                // tableData={[
                //   ['Dakota Rice', '$36,738', 'Niger'],
                //   ['2', 'Minerva Hooper', '$23,789', 'Curaçao'],
                //   ['3', 'Sage Rodriguez', '$56,142', 'Netherlands'],
                //   ['4', 'Philip Chaney', '$38,735', 'Korea, South'],
                // ]}
              />
            </CardBody>
          )
        )}
      </Card>
    );
  }
}
