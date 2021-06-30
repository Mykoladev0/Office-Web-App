import React, { Component } from 'react';
import truncate from 'lodash/truncate';

import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableRow from '@material-ui/core/TableRow';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';

import PageView from '@material-ui/icons/PageView';
import Edit from '@material-ui/icons/Edit';

// creates a beautiful scrollbar
import PerfectScrollbar from 'perfect-scrollbar';
import 'perfect-scrollbar/css/perfect-scrollbar.css';

import resultsTableStyle from '../Styles/ResultsTableStyle';
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import { Button } from '../../../Vendor/mr-pro/components';

export interface SmallResultsListProps extends WithStyles<typeof resultsTableStyle> {
  data: any[];
  displayField?: string;
  handlePopupFn?: (item: any) => void;
  handleEditFn?: (item: any) => void;
}

class SmallResultsList extends React.Component<SmallResultsListProps, any> {
  public static defaultProps: Partial<SmallResultsListProps> = {
    displayField: 'title',
    handlePopupFn: null,
    handleEditFn: null,
  };

  public resultsRef: any;
  private _ps: PerfectScrollbar = null;

  public constructor(props: SmallResultsListProps) {
    super(props);
  }
  public componentDidMount() {
    if (navigator.platform.indexOf('Win') > -1) {
      if (this.resultsRef) {
        this._ps = new PerfectScrollbar(this.resultsRef, {
          suppressScrollX: true,
          suppressScrollY: false,
        });
      }
    }
    this.setState({ loaded: true });
  }
  public componentWillUnmount() {
    if (navigator.platform.indexOf('Win') > -1) {
      this._ps.destroy();
    }
  }
  public render(): JSX.Element {
    const { classes, data, displayField, handleEditFn, handlePopupFn } = this.props;
    return (
      <div
        ref={elem => {
          this.resultsRef = elem;
        }}
        className={classes.container}
      >
        {!data || data.length === 0 ? (
          <p>No Results</p>
        ) : (
          <Table className={classes.table + ' table-condensed'}>
            <TableBody>
              {data.map((value, index) => {
                const displayText: string = truncate(value[displayField], { length: 25 });
                return (
                  <TableRow key={index} className={classes.tableRow}>
                    <Tooltip title={value[displayField]} placement="bottom">
                      <TableCell className={classes.tableCell}>{displayText}</TableCell>
                    </Tooltip>
                    {(handleEditFn || handlePopupFn) && (
                      <TableCell className={classes.tableActions} numeric={true}>
                        {handlePopupFn && (
                          <Button
                            aria-label="View"
                            size="sm"
                            fullWidth={false}
                            color="transparent"
                            justIcon
                            className={classes.tableActionButton}
                            onClick={() => {
                              handlePopupFn(value);
                            }}
                          >
                            <PageView className={classes.tableActionButtonIcon} />
                          </Button>
                        )}
                        {handleEditFn && (
                          <Button
                            size="sm"
                            fullWidth={false}
                            color="transparent"
                            justIcon
                            className={classes.tableActionButton}
                            onClick={evt => {
                              handleEditFn(value);
                            }}
                            aria-label="Edit"
                          >
                            <Edit className={classes.tableActionButtonIcon + ' ' + classes.edit} />
                          </Button>

                          // <IconButton
                          //   onClick={evt => {
                          //     console.log('clikc');
                          //     handleEditFn(value);
                          //   }}
                          //   aria-label="Edit"
                          //   className={classes.tableActionButton}
                          // >
                          //   <Edit className={classes.tableActionButtonIcon + ' ' + classes.edit} />
                          // </IconButton>
                          // <Tooltip id="tooltip-top-start" title="Edit" placement="top">

                          // </Tooltip>
                        )}
                      </TableCell>
                    )}
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        )}
      </div>
    );
  }
}
export default withStyles(resultsTableStyle)(SmallResultsList);
