import createStyles from '@material-ui/core/styles/createStyles';
import buttonStyle from '../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/components/buttonStyle';
const individualStyle = {
  marginRight: createStyles({ marginStyle: { marginRight: '3px' } }).marginStyle,
  //   tooltip: createStyles(tooltip),
  cardTitle: createStyles({
    cardTitleStyle: {
      marginTop: '0',
      marginBottom: '3px',
      color: '#3C4858',
      fontSize: '18px',
    },
  }).cardTitleStyle,
  center: createStyles({
    centerStyle: {
      textAlign: 'center',
    },
  }).centerStyle,
  right: createStyles({
    rightStyle: {
      textAlign: 'right',
    },
  }).rightStyle,
  left: createStyles({
    leftStyle: {
      textAlign: 'left',
    },
  }).leftStyle,
  //   ...buttonStyle,
};
export default createStyles(individualStyle);
