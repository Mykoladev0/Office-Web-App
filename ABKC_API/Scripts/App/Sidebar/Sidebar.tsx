import React, { Component } from 'react';
import { RouteComponentProps, NavLink, withRouter } from 'react-router-dom';

import { WithStyles, withStyles } from '@material-ui/core/styles';
import cx from 'classnames';

import { withAuth } from '@okta/okta-react';

// @material-ui/core components
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Hidden from '@material-ui/core/Hidden';
import Collapse from '@material-ui/core/Collapse';

import sidebarStyle from './sidebarStyle';
import { HeaderLinks } from '../../../Vendor/mr-pro/components/Header';
import { BaseProps } from '../Models';
import UserSideBarComponent from './UserSidebarComponent';
import SidebarWrapper from './SidebarWrapper';
import NavigationLinksComp from './NavigationLinksComp';
import { Button } from '../../../Vendor/mr-pro/components';

export enum AvailColorsEnum {
  white = 'white',
  red = 'red',
  orange = 'orange',
  green = 'green',
  blue = 'blue',
  purple = 'purple',
  rose = 'rose',
}
export enum BGColorEnum {
  white = 'white',
  black = 'black',
  blue = 'blue',
}

export interface SidebarProps extends WithStyles<typeof sidebarStyle>, BaseProps {
  logo: string;
  logoText: string;
  image: string;
  routes: any[];
  color: AvailColorsEnum;
  bgColor: BGColorEnum;
  miniActive: boolean;
  sideBarMinimizeFn: () => void;
}

export interface SidebarState {
  openAvatar: boolean;
  openDogs: boolean;
  openOwners: boolean;
  miniActive: boolean;
}

type Props = SidebarProps & RouteComponentProps<any>;

export default class SidebarComp extends React.Component<Props, SidebarState> {
  public state: SidebarState = {
    openAvatar: false,
    openDogs: false,
    openOwners: false,
    miniActive: true,
  };

  public constructor(props: SidebarProps) {
    super(props);
    // this.activeRoute = this.activeRoute.bind(this);
  }

  public render(): JSX.Element {
    const {
      classes,
      color,
      logo,
      image,
      logoText,
      routes,
      bgColor,
      miniActive,
      sideBarMinimizeFn,
    } = this.props;
    const itemText =
      classes.itemText +
      ' ' +
      cx({
        [classes.itemTextMini]: this.props.miniActive, // && this.state.miniActive,
      });
    const collapseItemText =
      classes.collapseItemText +
      ' ' +
      cx({
        [classes.collapseItemTextMini]: miniActive, // && this.state.miniActive,
      });
    const userWrapperClass =
      classes.user + ' ' + cx({ [classes.whiteAfter]: bgColor === BGColorEnum.white });
    const { caret, collapseItemMini, photo, logoMini } = classes;
    const logoNormal =
      classes.logoNormal +
      ' ' +
      cx({
        [classes.logoNormalSidebarMini]: this.props.miniActive, // && this.state.miniActive,
      });
    const logoClasses =
      classes.logo +
      ' ' +
      cx({
        [classes.whiteAfter]: bgColor === 'white',
      });
    const brand = (
      <div className={logoClasses}>
        <div className={logoMini}>
          <Button color="transparent" justIcon round onClick={() => sideBarMinimizeFn()}>
            <img src={logo} alt="logo" className={classes.img} />
          </Button>
        </div>
        {<div className={logoNormal}>{logoText}</div>}
      </div>
    );
    const drawerPaper =
      classes.drawerPaper +
      ' ' +
      cx({
        [classes.drawerPaperMini]: this.props.miniActive, // && this.state.miniActive,
      });
    const sidebarWrapper =
      classes.sidebarWrapper +
      ' ' +
      cx({
        [classes.drawerPaperMini]: this.props.miniActive, // && this.state.miniActive,
        [classes.sidebarWrapperWithPerfectScrollbar]: navigator.platform.indexOf('Win') > -1,
      });
    const userComp = (
      <UserSideBarComponent
        bgWhite={bgColor === BGColorEnum.white}
        // miniActive={miniActive && this.state.miniActive}
        miniActive={miniActive}
        classes={classes}
      />
    );
    const navLinksComp = (
      <NavigationLinksComp
        // miniActive={miniActive && this.state.miniActive}
        miniActive={miniActive}
        curPath={this.props.location.pathname}
        color={color}
        routes={routes}
        classes={classes}
      />
    );
    return (
      <div ref="mainPanel">
        <Hidden mdUp>
          <Drawer
            variant="temporary"
            anchor={'right'}
            open={this.props.open}
            classes={{
              paper: drawerPaper + ' ' + classes[bgColor + 'Background'],
            }}
            onClose={this.props.handleDrawerToggle}
            ModalProps={{
              keepMounted: true, // Better open performance on mobile.
            }}
          >
            {brand}
            <SidebarWrapper
              className={sidebarWrapper}
              userComp={userComp}
              linksComp={navLinksComp}
              headerLinksComp={HeaderLinks}
            />
            {/* sidebarwrapper goes here, line 436
                holds links, user, and headerlinks
            */}
            {image !== undefined ? (
              <div
                className={classes.background}
                style={{ backgroundImage: 'url(' + image + ')' }}
              />
            ) : null}
          </Drawer>
        </Hidden>
        <Hidden smDown>
          <Drawer
            onMouseOver={() => this.setState({ miniActive: false })}
            onMouseOut={() => this.setState({ miniActive: true })}
            anchor={'left'}
            variant="permanent"
            open
            classes={{
              paper: drawerPaper + ' ' + classes[bgColor + 'Background'],
            }}
          >
            {brand}
            <SidebarWrapper
              className={sidebarWrapper}
              userComp={userComp}
              linksComp={navLinksComp}
              headerLinksComp={HeaderLinks}
            />
            {image !== undefined ? (
              <div
                className={classes.background}
                style={{ backgroundImage: 'url(' + image + ')' }}
              />
            ) : null}
          </Drawer>
        </Hidden>
      </div>
    );
  }

  //   // verifies if routeName is the one active (in browser input)
  //   private activeRoute(routeName): boolean {
  //     const { location } = this.props;
  //     return location.pathname.indexOf(routeName) > -1;
  //   }
}

const SideBar = withRouter(withAuth(withStyles(sidebarStyle)(SidebarComp)));

export { SideBar };
