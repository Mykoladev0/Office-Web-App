// ##############################
// // // App styles
// #############################

import {
  drawerWidth,
  drawerMiniWidth,
  transition,
  containerFluid,
} from '../../material-dashboard-pro-react.jsx';

const appStyle = theme => ({
  wrapper: {
    position: 'relative',
    top: '0',
    height: '100vh',
    '&:after': {
      display: 'table',
      clear: 'both',
      content: '" "',
    },
  },
  mainPanel: {
    transitionProperty: 'top, bottom, width',
    transitionDuration: '.2s, .2s, .35s',
    transitionTimingFunction: 'linear, linear, ease',
    [theme.breakpoints.up('md')]: {
      width: `calc(100% - ${drawerWidth}px)`,
    },
    overflow: 'auto',
    position: 'relative',
    float: 'right',
    ...transition,
    maxHeight: '100%',
    width: '100%',
    overflowScrolling: 'touch',
  },
  // Load app bar information from the theme
  toolbar: theme.mixins.toolbar,
  content: {
    // marginTop: '70px',
    marginTop: '30px',
    padding: '0px 15px 30px ',
    // minHeight: 'calc(100vh - 123px)',
    minHeight: 'calc(100vh - 73px)',
  },
  container: { ...containerFluid },
  map: {
    marginTop: '70px',
  },
  mainPanelSidebarMini: {
    [theme.breakpoints.up('md')]: {
      width: `calc(100% - ${drawerMiniWidth}px)`,
    },
  },
  mainPanelWithPerfectScrollbar: {
    overflow: 'hidden !important',
  },
});

export default appStyle;

// WEBPACK FOOTER //
// ./src/assets/jss/material-dashboard-pro-react/layouts/dashboardStyle.jsx
