import React, { Component } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import PerfectScrollbar from 'perfect-scrollbar';

export interface SideBarWrapperProps {
  className: string;
  userComp: new (props: any) => Component;
  linksComp: new (props: any) => Component;
  headerLinksComp: new (props: any) => Component;
}
type Props = SideBarWrapperProps & RouteComponentProps<any>;

export default class SidebarWrapper extends Component<Props, any> {
  public refs: {
    sidebarWrapper: HTMLInputElement;
  };
  private ps: PerfectScrollbar;

  public componentDidMount() {
    if (navigator.platform.indexOf('Win') > -1) {
      this.ps = new PerfectScrollbar(this.refs.sidebarWrapper, {
        suppressScrollX: true,
        suppressScrollY: false,
      });
    }
  }
  public componentWillUnmount() {
    if (navigator.platform.indexOf('Win') > -1) {
      this.ps.destroy();
    }
  }
  public render() {
    const { className, userComp, linksComp, headerLinksComp } = this.props;
    //, headerLinks, links
    return (
      <div className={className} ref="sidebarWrapper">
        {userComp}
        {/* {headerLinks}
        {links} */}
        {/* {headerLinksComp} */}
        {linksComp}
      </div>
    );
  }
}
