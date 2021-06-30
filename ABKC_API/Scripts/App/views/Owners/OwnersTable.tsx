import React, { Component } from 'react';
import ReactTable from 'react-table';
import { Link } from 'react-router-dom';
// import 'react-table/react-table.css';//probably replace with MR-PRO
import capitalize from 'lodash/fp/capitalize';

import { Spinner } from '../../../Shared/ReactComponents/Spinner';

import { OwnerModel } from '../../Models';
import { Button } from '../../../../Vendor/mr-pro/components';
import tableStyle from '../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/components/tableStyle.jsx';
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
// import { createStyles } from '@material-ui/core/styles';

import Edit from '@material-ui/icons/Edit';
import PageView from '@material-ui/icons/PageView';
import Delete from '@material-ui/icons/Delete';
import { getOwnersTableData } from '../../Api/ownerService';

export interface OwnersTableCompProps extends WithStyles<typeof tableStyle> {
  isMobile?: () => boolean;
  isTablet?: () => boolean;
  searchText?: string;
  showPopupFn: (owner: OwnerModel) => void;
  deleteOwnerFn?: (owner: OwnerModel) => void;
}

export interface OwnersTableCompState {
  shouldDisplayTable: boolean;
  isLoading: boolean;
  ownerList: OwnerModel[];
  pages: number; //calculate on component mount
  defaultPageSize: number;
}

export class OwnerTableComp extends React.Component<OwnersTableCompProps, OwnersTableCompState> {
  public static defaultProps: Partial<OwnersTableCompProps> = {
    isMobile: () => false,
    isTablet: () => false,
    searchText: '',
    deleteOwnerFn: (owner: OwnerModel) => {},
  };

  public state: OwnersTableCompState = {
    shouldDisplayTable: true,
    isLoading: true,
    ownerList: [],
    pages: -1, //unknown
    defaultPageSize: 10,
  };

  private tableColumns: any[] = [
    {
      Header: 'First Name',
      accessor: 'firstName',
    },
    {
      Header: 'Last Name',
      accessor: 'lastName',
    },
    {
      Header: 'Email Address',
      id: 'email',
      accessor: d => d.email,
      show: !this.props.isMobile(),
    },
    {
      Header: '',
      sortable: false,
      filterable: false,
      width: 140,
      Cell: row => {
        return (
          <div className="actions-right">
            <Button
              justIcon
              round
              simple
              onClick={() => this.props.showPopupFn(row.original)}
              color="info"
              className="like"
            >
              <PageView />
            </Button>{' '}
            <Link to={`/owners/${row.original.ownerId}/edit`}>
              <Button justIcon round simple color="warning" className="edit">
                <Edit />
              </Button>
            </Link>{' '}
            <Button
              justIcon
              round
              simple
              onClick={() => this.props.deleteOwnerFn(row.original)}
              color="danger"
              className="remove"
            >
              <Delete />
            </Button>
          </div>
        );
      },
    },
  ];

  public constructor(props: OwnersTableCompProps) {
    super(props);
    this.onPageSizeChange = this.onPageSizeChange.bind(this);
    this.fetchTableData = this.fetchTableData.bind(this);
  }
  public componentDidMount() {
    //load the # of possible records with no filter
    //load first page of date
    const { defaultPageSize } = this.state;
  }

  public render(): JSX.Element {
    const { classes, searchText } = this.props;
    const { shouldDisplayTable, isLoading, defaultPageSize, pages, ownerList } = this.state;
    const canFilter: boolean = searchText === '';
    const h =
      window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;

    if (!shouldDisplayTable) {
      return (
        <div className="text-center">
          <Spinner />
        </div>
      );
    }

    return (
      <div>
        <ReactTable
          data={ownerList} // should default to []
          pages={pages} // should default to -1 (which means we don't know how many pages we have)
          loading={isLoading}
          className="-striped -highlight"
          columns={this.tableColumns}
          defaultPageSize={defaultPageSize}
          sortable={true}
          filterable={canFilter}
          manual // informs React Table that you'll be handling sorting and pagination server-side
          onPageSizeChange={this.onPageSizeChange}
          onFetchData={this.fetchTableData}
          style={{
            height: `${h - 195}px`, // This will force the table body to overflow and scroll, since there is not enough room
          }}
        />
      </div>
    );
  }

  private fetchTableData(state, instance) {
    // Whenever the table model changes, or the user sorts or changes pages, this method gets called and passed the current table model.
    // You can set the `loading` prop of the table to true to use the built-in one or show you're own loading bar if you want.
    this.setState({ isLoading: true }, () => {
      const { searchText } = this.props;
      let isExternalFilter: boolean = false;
      // Request the data however you want.
      let filtered = state.filtered;
      if (searchText && searchText.length > 0) {
        filtered = [
          {
            id: 'lastName',
            value: searchText,
          },
        ];
        isExternalFilter = true;
      }
      getOwnersTableData(
        state.page + 1,
        state.pageSize,
        state.sorted,
        filtered,
        isExternalFilter
      ).then(({ data, count }) => {
        // Now just get the rows of data to your React Table (and update anything else like total pages or loading)
        const pageCount: number = Math.ceil(count / state.pageSize);
        this.setState({
          ownerList: data,
          pages: pageCount,
          isLoading: false,
        });
      });
    });
  }

  private onPageSizeChange(pageSize: number) {
    const { defaultPageSize } = this.state;
    this.setState({ defaultPageSize: pageSize }, () => {
      const container = document.querySelector('#mainPanel');
      container.scrollTop = 0;
    });
  }
}

const OwnersTable = withStyles(tableStyle)(OwnerTableComp);

export { OwnersTable };
