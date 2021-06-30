import React, { Component } from 'react';

import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import ReactTable from 'react-table';
import 'react-table/react-table.css';

export interface ShowEventsTableProps {
  show: any;
  isLoading: boolean;
}

// export interface ShowEventsState {
//   // isLoading: boolean;
// }

export default class ShowEventsTable extends Component<ShowEventsTableProps, any> {
  // public state: ShowEventsState = {};
  public constructor(props: ShowEventsTableProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { show, isLoading } = this.props;

    if (isLoading) {
      return <h3>Loading Show</h3>;
    }
    if (!show) {
      return <h3>Show could not be loaded</h3>;
    }
    const columns = this.buildTableColumns();
    const resultColumns = this.buildResultColumns();
    return (
      <div>
        <Row>
          <Col>
            <ReactTable
              data={show.events}
              columns={columns}
              defaultPageSize={10}
              className="-striped -highlight"
              defaultSorted={[
                {
                  id: 'style',
                  desc: false,
                },
              ]}
              SubComponent={row => {
                const { original } = row;
                return (
                  <div style={{ padding: '10px' }}>
                    <p>Winning Participants:</p>
                    <ReactTable
                      data={original.results}
                      columns={resultColumns}
                      defaultPageSize={3}
                      showPagination={false}
                    />
                  </div>
                );
              }}
            />
          </Col>
        </Row>
      </div>
    );
  }

  private GetWinner(event) {
    //find winner in event and return abkc
    if (!event.results || event.results === null) {
      return null;
    }
    if (event.results.length === 1) {
      return event.results[0];
    }
    const winner = event.results.reduce(
      (prev, current) => (prev.points > current.points ? prev : current)
    );
    return winner;
  }

  private buildTableColumns(): any[] {
    return [
      {
        Header: 'Winning ABKC #',
        id: 'abkc',
        accessor: d => {
          const winner = this.GetWinner(d);
          return winner ? (winner.winning_ABKC ? winner.winning_ABKC : '') : '';
        },
      },
      {
        Header: 'Class',
        accessor: 'class',
      },
      {
        Header: 'Breed',
        accessor: 'breed',
      },
      {
        Header: 'Style',
        accessor: 'style',
      },
      {
        Header: 'Gender',
        accessor: 'gender',
      },
    ];
  }
  private buildResultColumns(): any[] {
    return [
      {
        Header: 'Armband #',
        accessor: 'armbandNumber',
      },
      {
        Header: 'Place',
        accessor: d => {
          return 'CALCULATE';
        },
        id: 'place',
      },
      {
        Header: "Dog's Name",
        accessor: 'dogName',
      },
      {
        Header: "Owner's Name",
        accessor: 'ownerName',
      },
      {
        Header: 'Phone #',
        accessor: 'phoneNum',
      },
      {
        Header: 'Points',
        accessor: 'points',
      },
      {
        Header: '# in Champ Class',
        accessor: d => {
          return '?';
        },
        id: 'champNum',
      },
    ];
  }
}
