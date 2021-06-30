import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Switch, Redirect, Route } from 'react-router-dom';
import cx from 'classnames';

// creates a beautiful scrollbar
import PerfectScrollbar from 'perfect-scrollbar';
import 'perfect-scrollbar/css/perfect-scrollbar.css';

import '../../Vendor/mr-pro/assets/css/material-dashboard-pro-react.css';

import { SideBar, AvailColorsEnum, BGColorEnum } from '../App/Sidebar/Sidebar';

// import { Header } from '../../Vendor/mr-pro/components/Header';
import Header from './Header/Header';
import routes from './Dashboard/routes';
import Domain from '@material-ui/icons/Domain';

import withStyles from '@material-ui/core/styles/withStyles';
import appStyle from '../../Vendor/mr-pro/assets/jss/material-dashboard-react/layouts/dashboardStyle.jsx';

// import image from "../../Vendor/material-react/assets/img/sidebar-3.jpg";
import logo from '../../Content/images/abkc_logo.png';
import sideBarImage from '../../Content/images/abkcBannerImage.jpg';

class App extends Component<any, any> {
  public state = {
    mobileOpen: false,
    miniActive: false,
    loaded: false,
  };
  public refs: {
    mainPanel: HTMLDivElement;
  };
  private _ps: PerfectScrollbar = null;
  // private _scrollBarRef: any = null;
  private switchRoutes = (
    <Switch>
      {routes.map((prop, key) => {
        if (prop.redirect) {
          return <Redirect from={prop.path} to={prop.to} key={key} />;
        }
        // return <Route path={prop.path} component={prop.component} key={key} />;
        return (
          <Route
            path={prop.path}
            render={props => <prop.component scroll={this._ps} {...props} />}
            key={key}
          />
        );
      })}
    </Switch>
  );

  public constructor(props) {
    super(props);
    this.handleDrawerToggle = this.handleDrawerToggle.bind(this);
    this.handleSidebarMinimize = this.handleSidebarMinimize.bind(this);
  }
  public componentDidMount() {
    if (navigator.platform.indexOf('Win') > -1) {
      this._ps = new PerfectScrollbar(this.refs.mainPanel, {
        // this._ps = new PerfectScrollbar('#mainPanel', {
        suppressScrollX: true,
        suppressScrollY: false,
      });
      document.body.style.overflow = 'hidden';
    }
    this.setState({ loaded: true });
  }
  public componentWillUnmount() {
    if (navigator.platform.indexOf('Win') > -1) {
      this._ps.destroy();
    }
  }
  public render(): JSX.Element {
    const { classes, ...rest } = this.props;
    const { miniActive, loaded } = this.state;
    const mainPanel =
      classes.mainPanel +
      ' ' +
      cx({
        [classes.mainPanelSidebarMini]: miniActive,
        [classes.mainPanelWithPerfectScrollbar]: navigator.platform.indexOf('Win') > -1,
      });
    const { mobileOpen } = this.state;
    return (
      <div className={classes.wrapper}>
        <SideBar
          routes={routes}
          logoText={'ABKC Online'}
          logo={logo}
          image={sideBarImage}
          handleDrawerToggle={this.handleDrawerToggle}
          open={mobileOpen}
          color={AvailColorsEnum.blue}
          bgColor={BGColorEnum.black}
          miniActive={miniActive}
          sideBarMinimizeFn={this.handleSidebarMinimize}
          {...rest}
        />
        {
          <div className={mainPanel} ref="mainPanel" id="mainPanel">
            {/* <Header
              routes={routes}
              handleDrawerToggle={this.handleDrawerToggle}
              miniActive={miniActive}
              sideBarMinimizeFn={this.handleSidebarMinimize}
              pinToTop={true}
              {...rest}
            /> */}
            {/* On the /maps route we want the map to be on full screen - this is not possible if the content and container classes are present because they have some paddings which would make the map smaller */}
            <div className={classes.content}>
              {loaded && <div className={classes.container}>{this.switchRoutes}</div>}
            </div>

            {/* {this.getRoute() ? <Footer /> : null} */}
          </div>
        }
      </div>
    );
  }

  private handleDrawerToggle(): void {
    this.setState({ mobileOpen: !this.state.mobileOpen });
  }
  private handleSidebarMinimize() {
    this.setState({ miniActive: !this.state.miniActive });
  }
}

export default withStyles(appStyle)(App);
