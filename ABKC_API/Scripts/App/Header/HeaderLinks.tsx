import React from 'react';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import { Manager, Reference, Popper } from 'react-popper';

// @material-ui/core components
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import MenuItem from '@material-ui/core/MenuItem';
import MenuList from '@material-ui/core/MenuList';
import ClickAwayListener from '@material-ui/core/ClickAwayListener';
import Paper from '@material-ui/core/Paper';
import Grow from '@material-ui/core/Grow';
import Hidden from '@material-ui/core/Hidden';

// @material-ui/icons
import Person from '@material-ui/icons/Person';
import Notifications from '@material-ui/icons/Notifications';
import Dashboard from '@material-ui/icons/Dashboard';
import Search from '@material-ui/icons/Search';
import { CustomInput, Button } from '../../../Vendor/mr-pro/components';

// core components

//import headerLinksStyle from "../../assets/jss/material-dashboard-react/components/headerLinksStyle.jsx";
import headerLinksStyle from '../../../Vendor/mr-pro/assets/jss/material-dashboard-react/components/headerLinksStyle.jsx';
export interface HeaderLinksProps extends WithStyles<typeof headerLinksStyle> {}
interface HeaderLinksState {
  open: boolean;
}

export default withStyles(headerLinksStyle)(
  class HeaderLinks extends React.Component<HeaderLinksProps, HeaderLinksState> {
    public state: HeaderLinksState = {
      open: false,
    };
    public render() {
      const { classes } = this.props;
      const { open } = this.state;
      const searchButton = classes.top + ' ' + classes.searchButton;
      const dropdownItem = classes.dropdownItem;
      // const wrapper = classNames({
      //   [classes.wrapperRTL]: rtlActive,
      // });
      const managerClasses = classNames({
        [classes.managerClasses]: true,
      });
      return (
        //   <div className={wrapper}>
        <div>
          <CustomInput
            formControlProps={{
              className: classes.top + ' ' + classes.search,
            }}
            inputProps={{
              placeholder: 'Search',
              inputProps: {
                'aria-label': 'Search',
                // tslint:disable-next-line:object-literal-key-quotes
                className: classes.searchInput,
              },
            }}
          />
          <Button color="white" aria-label="edit" justIcon round className={searchButton}>
            <Search className={classes.headerLinksSvg + ' ' + classes.searchIcon} />
          </Button>
          <Button
            color="transparent"
            simple
            aria-label="Dashboard"
            justIcon
            className={classes.buttonLink}
            muiClasses={{
              label: '',
            }}
          >
            <Dashboard className={classes.headerLinksSvg + ' ' + classes.links} />
            <Hidden mdUp>
              <span className={classes.linkText}>{'Dashboard'}</span>
            </Hidden>
          </Button>
          <div className={managerClasses}>
            <Manager>
              {/* className={managerClasses} */}
              <Reference>
                {({ ref }) => (
                  <Button
                    color="transparent"
                    ref={ref}
                    justIcon
                    aria-label="Notifications"
                    aria-owns={open ? 'menu-list' : null}
                    aria-haspopup="true"
                    onClick={() => this.setState({ open: !this.state.open })}
                    className={classes.buttonLink}
                    muiClasses={{
                      label: '',
                    }}
                  >
                    <Notifications className={classes.headerLinksSvg + ' ' + classes.links} />
                    <span className={classes.notifications}>5</span>
                    <Hidden mdUp>
                      <span
                        onClick={() => this.setState({ open: !this.state.open })}
                        className={classes.linkText}
                      >
                        {'Notification'}
                      </span>
                    </Hidden>
                  </Button>
                )}
              </Reference>
              {/* className={classNames({ [classes.popperClose]: !open }) + ' ' + classes.pooperResponsive} */}
              <Popper placement="bottom-start" eventsEnabled={open}>
                {({ ref, style, placement, arrowProps }) => <div />}
              </Popper>
            </Manager>
          </div>
          <Button
            color="transparent"
            aria-label="Person"
            justIcon
            className={classes.buttonLink}
            muiClasses={{
              label: '',
            }}
          >
            <Person className={classes.headerLinksSvg + ' ' + classes.links} />
            <Hidden mdUp>
              <span className={classes.linkText}>{'Profile'}</span>
            </Hidden>
          </Button>
        </div>
      );
    }
  }
);

// HeaderLinks.propTypes = {
//   classes: PropTypes.object.isRequired,
//   rtlActive: PropTypes.bool,
// };
