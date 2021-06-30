import React, { Component } from 'react';
import truncate from 'truncate';
import NavItem from 'reactstrap/lib/NavItem';
// import { Collapse, Nav, Navbar , NavItem, NavbarBrand, NavbarToggler } from 'reactstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { MdMenu, MdHome, MdEventAvailable, MdSettingsPower } from 'react-icons/md';
import { FaUserPlus, FaFlagCheckered } from 'react-icons/fa';

export interface INavMenuProps extends RouteComponentProps<any, any> {
  currentShow: any;
}
class NavMenu extends Component<INavMenuProps, any> {
  public displayName: string = NavMenu.name;

  public render() {
    const { currentShow } = this.props;
    const showName: string = truncate(currentShow != null ? currentShow.showName : 'None', 12);

    return (
      <nav className="sidebar">
        <h3 className="site-title">
          <a href="index.html">Show Assist</a>
        </h3>
        <h5>{'Current Show'}</h5>
        <h5>{`${showName}`}</h5>
        {/* <LinkContainer to={'#menu-toggle'} id="menu-toggle" className="btn btn-default">
          <MdMenu />
        </LinkContainer> */}
        <a href="#menu-toggle" id="menu-toggle" className="btn btn-default">
          <MdMenu />
        </a>
        <ul className="nav nav-pills flex-column sidebar-nav">
          <LinkContainer to={'/'} exact={true} className="nav-link">
            <NavItem>
              <MdHome /> Shows
            </NavItem>
          </LinkContainer>
          {currentShow && this.getCurEventsNavLink()}
          {currentShow && this.getShowRegistrationNavLink()}
          {currentShow && this.getShowFinalizationNavLink()}
        </ul>
        <LinkContainer to={'/login'} className="logout-button">
          <div>
            <MdSettingsPower /> Signout{' '}
          </div>
        </LinkContainer>
      </nav>
    );
  }
  private getCurEventsNavLink() {
    const { currentShow } = this.props;
    return (
      <LinkContainer
        to={{
          pathname: '/ShowEvents',
          state: {
            showId: currentShow != null ? currentShow.showId : -1,
          },
        }}
        className="nav-link"
      >
        <NavItem disabled={true}>
          <MdEventAvailable /> Show Events
        </NavItem>
      </LinkContainer>
    );
  }
  private getShowRegistrationNavLink() {
    const { currentShow } = this.props;
    return (
      <LinkContainer
        to={{
          pathname: '/ShowRegistration',
          state: {
            currentShow,
          },
        }}
        className="nav-link"
      >
        <NavItem disabled={true}>
          <FaUserPlus /> Registration
        </NavItem>
      </LinkContainer>
    );
  }
  private getShowFinalizationNavLink() {
    const { currentShow } = this.props;
    return (
      <LinkContainer
        to={{
          pathname: '/ShowFinalization',
          state: {
            currentShow,
          },
        }}
        className="nav-link"
      >
        <NavItem disabled={true}>
          <FaFlagCheckered /> Finalize
        </NavItem>
      </LinkContainer>
    );
  }
}
export default withRouter(NavMenu);
