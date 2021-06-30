import React, { Component } from 'react';
import ReactTable from 'react-table';
import { Link } from 'react-router-dom';
// import 'react-table/react-table.css';//probably replace with MR-PRO
import capitalize from 'lodash/fp/capitalize';

import { Spinner } from '../../../Shared/ReactComponents/Spinner';
import { withGetScreen } from '../../../Utilities/GetScreenSize';

import { DogModel } from '../../Models';
import { getDogsRecordCount, getDogsTableData } from '../../Api/dogService';
import { Button } from '../../../../Vendor/mr-pro/components';
import tableStyle from '../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/components/tableStyle.jsx';
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
// import { createStyles } from '@material-ui/core/styles';

import Edit from '@material-ui/icons/Edit';
import PageView from '@material-ui/icons/PageView';
import Delete from '@material-ui/icons/Delete';
import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';
import Modal from '@material-ui/core/Modal';
import { IndividualDog } from './IndividualDog/IndividualDog';

export interface DogsTableCompProps extends WithStyles<typeof tableStyle> {
  isMobile?: () => boolean;
  isTablet?: () => boolean;
  searchText?: string;
  showPopupFn: (dog: DogModel) => void;
  deleteDogFn?: (dog: DogModel) => void;
}

export interface DogsTableCompState {
  shouldDisplayTable: boolean;
  isLoading: boolean;
  dogList: DogModel[];
  pages: number; //calculate on component mount
  defaultPageSize: number;
  showDogPopup: boolean;
  dogToShow: DogModel;
}

export class DogsTableComp extends React.Component<DogsTableCompProps, DogsTableCompState> {
  public static defaultProps: Partial<DogsTableCompProps> = {
    isMobile: () => false,
    isTablet: () => false,
    searchText: '',
    deleteDogFn: (dog: DogModel) => {},
  };

  public state: DogsTableCompState = {
    shouldDisplayTable: true,
    isLoading: true,
    dogList: [],
    pages: -1, //unknown
    defaultPageSize: 10,
    showDogPopup: false,
    dogToShow: null,
  };

  private tableColumns: any[] = [
    {
      Header: 'Dog Name',
      accessor: 'dogName',
    },
    {
      Header: 'Gender',
      id: 'gender',
      accessor: d => d.gender,
      // visible: !this.props.isMobile(),
      Cell: props => <span>{getGenderCell(props)}</span>, // Custom cell components!
    },
    {
      Header: 'Bully ID',
      accessor: 'bullyId',
      show: !this.props.isMobile(),
    },
    {
      Header: 'ABKC #',
      id: 'abkcNo',
      accessor: d => d.abkcNo,
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
            <Link to={`/dogs/${row.original.id}/edit`}>
              <Button justIcon round simple color="warning" className="edit">
                <Edit />
              </Button>
            </Link>{' '}
            <Button
              justIcon
              round
              simple
              onClick={() => this.props.deleteDogFn(row.original)}
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

  public constructor(props: DogsTableCompProps) {
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
    const { shouldDisplayTable, isLoading, defaultPageSize, pages, dogList } = this.state;
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
          data={dogList} // should default to []
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
      // Request the data however you want.
      let filtered = state.filtered;
      if (searchText && searchText.length > 0) {
        filtered = [
          {
            // the current filters model
            id: 'dogName',
            value: searchText,
          },
        ];
      }
      getDogsTableData(state.page + 1, state.pageSize, state.sorted, filtered).then(
        ({ data, count }) => {
          // Now just get the rows of data to your React Table (and update anything else like total pages or loading)
          const pageCount: number = Math.ceil(count / state.pageSize);
          this.setState({
            dogList: data,
            pages: pageCount,
            isLoading: false,
          });
        }
      );
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

const getGenderCell = props => {
  const val: string = props.value
    ? (props.value as string).includes('?')
      ? 'Unknown'
      : props.value
    : '';
  return capitalize(val);
};
const DogsTable = withStyles(tableStyle)(DogsTableComp);

export { DogsTable };

const modalStyle = {
  position: 'fixed' as 'fixed',
  zIndex: 1040,
  top: 75,
  left: 75,
};

const backdropStyle = {
  ...modalStyle,
  zIndex: 'auto',
  backgroundColor: '#000',
  opacity: 0.5,
};
