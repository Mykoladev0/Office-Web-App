import {
  defaultFont,
  primaryColor,
  dangerColor,
  tooltip,
} from '../../../Vendor/mr-pro/assets/jss/material-dashboard-pro-react';
import createStyles from '@material-ui/core/styles/createStyles';

const tableCellStyle = {
  // ...defaultFont,
  fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  // fontWeight: 300,
  padding: '0',
  verticalAlign: 'middle',
  border: 'none',
  // lineHeight: '1.42857143',
  // fontSize: '14px',
  fontSize: '12px',
  height: 'auto !important',
};

const resultsTableStyle = {
  container: createStyles({
    containerStyle: {
      height: '100px',
      ['overflow-x']: 'hidden',
      ['overflow-y']: 'auto',
      position: 'relative',
    },
  }).containerStyle,
  table: {
    marginBottom: '0',
  },
  tableRow: createStyles({
    tableRowStyle: {
      position: 'relative',
      borderBottom: '1px solid #dddddd',
      height: '30px',
    },
  }).tableRowStyle,
  tableActions: {
    border: 'none',
    padding: '0px 8px !important', //12px
    verticalAlign: 'middle',
  },
  tableCell: tableCellStyle,
  tableActionButton: {
    // marginBottom: '-2px',
    // width: '27px',
    // height: '14px',
  },
  tableActionButtonIcon: {
    width: '27px',
    height: '18px',
  },
  edit: {
    backgroundColor: 'transparent',
    color: primaryColor,
    boxShadow: 'none',
  },
  close: {
    backgroundColor: 'transparent',
    color: dangerColor,
    boxShadow: 'none',
  },
  //   tooltip: createStyles(tooltip),
};
export default createStyles(resultsTableStyle);
