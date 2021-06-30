import React, { Component } from 'react';

import { withRouter, RouteComponentProps } from 'react-router-dom';
import cx from 'classnames';

import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Hidden from '@material-ui/core/Hidden';
import { Button } from '../../../Vendor/mr-pro/components';
import ViewList from '@material-ui/icons/ViewList';
import MoreVert from '@material-ui/icons/MoreVert';
import Menu from '@material-ui/core/Menu';
import HeaderLinks from './HeaderLinks';

import headerStyle from '../../../Vendor/mr-pro/assets/jss/material-dashboard-react/components/headerStyle.jsx';
export interface HeaderProps extends WithStyles<typeof headerStyle> {
  routes: any[];
  sideBarMinimizeFn: () => void;
  handleDrawerToggle: () => void;
  miniActive: boolean;
  pinToTop?: boolean;
}
type Props = HeaderProps & RouteComponentProps<any>;
export interface HeaderState {}

export default withRouter(
  withStyles(headerStyle)(
    class Header extends React.Component<Props, HeaderState> {
      public static defaultProps: Partial<HeaderProps> = {
        pinToTop: false,
      };
      public state: HeaderState = {};

      public constructor(props: HeaderProps) {
        super(props);
        this.makeBrand = this.makeBrand.bind(this);
      }

      public render(): JSX.Element {
        const {
          classes,
          sideBarMinimizeFn,
          handleDrawerToggle,
          miniActive,
          color,
          pinToTop,
        } = this.props;
        const appBarClasses = cx({
          [' ' + classes[color]]: color,
        });
        const sidebarMinimize = classes.sidebarMinimize;
        return (
          <AppBar
            className={classes.appBar + appBarClasses}
            position={pinToTop ? 'sticky' : 'fixed'}
          >
            <Toolbar className={classes.container}>
              <Hidden smDown>
                <div className={sidebarMinimize}>
                  {miniActive ? (
                    <Button justIcon round color="white" onClick={sideBarMinimizeFn}>
                      <ViewList className={classes.sidebarMiniIcon} />
                    </Button>
                  ) : (
                    <Button justIcon round color="white" onClick={sideBarMinimizeFn}>
                      <MoreVert className={classes.sidebarMiniIcon} />
                    </Button>
                  )}
                </div>
              </Hidden>
              <div className={classes.flex}>
                {/* Here we create navbar brand, based on route name */}
                <Button href="#" className={classes.title} color="transparent">
                  {this.makeBrand()}
                </Button>
              </div>
              <Hidden smDown implementation="css">
                <HeaderLinks {...classes} />
              </Hidden>
              <Hidden mdUp>
                <Button
                  className={classes.appResponsive}
                  color="transparent"
                  justIcon
                  aria-label="open drawer"
                  onClick={handleDrawerToggle}
                >
                  <Menu open={false} />
                </Button>
              </Hidden>
            </Toolbar>
          </AppBar>
        );
      }
      private makeBrand() {
        let name: string = '';
        const { routes, location } = this.props;
        routes.map((prop, key) => {
          if (prop.collapse) {
            prop.views.map((prop2, key) => {
              if (prop2.path === location.pathname) {
                name = prop.name;
              }
              return null;
            });
          }
          if (prop.path === location.pathname) {
            name = prop.name;
          }
          return null;
        });
        return name;
      }
    }
  )
);
