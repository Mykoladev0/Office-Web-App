import {
  successColor,
  tooltip,
  cardTitle,
} from '../../../Vendor/mr-pro/assets/jss/material-dashboard-pro-react';
import { createStyles } from '@material-ui/core/styles';

const modalStyle = {
  position: 'fixed' as 'fixed',
  zIndex: 1040,
  top: 75,
  left: 75,
  alignItems: 'center',
  justifyContent: 'center',
};

const CardStyle = {
  cardTitle: {
    ...cardTitle,
    marginTop: '0px',
    marginBottom: '3px',
  },
  cardIconTitle: {
    ...cardTitle,
    marginTop: '15px',
    marginBottom: '0px',
  },
  cardCategory: {
    color: '#999999',
    fontSize: '14px',
    paddingTop: '10px',
    marginBottom: '0',
    marginTop: '0',
    margin: '0',
  },
  stats: createStyles({
    statsStyle: {
      color: '#999999',
      fontSize: '12px',
      lineHeight: '22px',
      display: 'inline-flex',
      ['& svg']: {
        position: 'relative',
        top: '4px',
        width: '16px',
        height: '16px',
        marginRight: '3px',
      },
      ['& i']: {
        position: 'relative',
        top: '4px',
        fontSize: '16px',
        marginRight: '3px',
      },
    },
  }).statsStyle,
  modalStyle: modalStyle,
};
export default createStyles(CardStyle);
