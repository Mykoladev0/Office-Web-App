import React, { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import Button from 'reactstrap/lib/Button';
import Table from 'reactstrap/lib/Table';
import ButtonGroup from 'reactstrap/lib/ButtonGroup';
import { getUpcomingShows } from '../api/showService';
import moment from 'moment';
import sortBy from 'lodash/sortBy';
import { IconContext } from 'react-icons';
import { MdPageview } from 'react-icons/md';
import { MdSlideshow } from 'react-icons/md';

interface ShowsState {
  shows: any[];
}
interface ShowsProps {
  handleSetCurrentShowFn: (show: any) => void;
}
export default withRouter(
  class Shows extends Component<ShowsProps & RouteComponentProps<any>, ShowsState> {
    public state: ShowsState = {
      shows: null,
    };
    public displayName = Shows.name;

    public constructor(props) {
      super(props);
      this.handleClick = this.handleClick.bind(this);
    }
    public componentDidMount() {
      //get upcoming shows
      getUpcomingShows().then(data => this.setState({ shows: data.data }));
    }

    public render() {
      const { shows } = this.state;
      return (
        <div>
          <h1>Upcoming and Recent Shows</h1>
          {shows ? this.renderShows(shows) : <h3>No shows available</h3>}
        </div>
      );
    }
    private handleClick(showId: number) {
      this.props.history.push('/ShowEvents', {
        showId: showId,
      });
    }
    private renderShows(shows) {
      const sortedShows = sortBy(shows, s => moment(s.showDate).format('YYYYMMDD'));
      const { handleSetCurrentShowFn } = this.props;
      return (
        <Table hover>
          <thead>
            <tr>
              <th>Show</th>
              <th>Address</th>
              <th>Date</th>
            </tr>
          </thead>
          <tbody>
            {sortedShows.map(s => {
              return (
                <tr key={s.showId}>
                  {/* <th scope="row">{s.showId}</th> */}
                  <th scope="row">{s.showName}</th>
                  <td>{s.address}</td>
                  <td>{moment(s.showDate).format('LL')}</td>
                  <td>
                    <ButtonGroup>
                      <Button
                        color="secondary"
                        outline
                        size="sm"
                        onClick={evt => this.handleClick(s.showId)}
                      >
                        <IconContext.Provider
                          value={{ color: 'black', size: '3em', className: 'global-class-name' }}
                        >
                          <div>
                            <MdPageview />
                          </div>
                        </IconContext.Provider>
                      </Button>
                      <Button
                        color="primary"
                        outline
                        size="sm"
                        onClick={evt => handleSetCurrentShowFn(s)}
                      >
                        <IconContext.Provider
                          value={{ color: 'black', size: '3em', className: 'global-class-name' }}
                        >
                          <div>
                            <MdSlideshow />
                          </div>
                        </IconContext.Provider>
                      </Button>
                    </ButtonGroup>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </Table>
      );
    }
  }
);
