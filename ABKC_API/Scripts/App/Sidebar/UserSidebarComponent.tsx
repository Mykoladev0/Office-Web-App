import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
import { withAuth } from '@okta/okta-react';

import { WithStyles, withStyles } from '@material-ui/core/styles';
import cx from 'classnames';

import sidebarStyle from './sidebarStyle';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import Collapse from '@material-ui/core/Collapse';

import { BaseProps } from '../../App/Models';
import { checkAuthentication } from '../../Utilities/AuthenticationHelper';

import avatar from '../../../Vendor/mr-pro/assets/img/avatar.jpg';

export interface UserSideBarComponentProps extends WithStyles<typeof sidebarStyle>, BaseProps {
  bgWhite: boolean;
  miniActive: boolean;
}
export interface UserSideBarComponentState {
  openAvatar: boolean;
  authenticated: any;
  userInfo: any;
}

export default withAuth(
  class UserSideBarComponent extends React.Component<
    UserSideBarComponentProps,
    UserSideBarComponentState
  > {
    public state: UserSideBarComponentState = {
      openAvatar: false,
      authenticated: null,
      userInfo: null,
    };
    private checkAuthentication: () => {} = null;
    public constructor(props: UserSideBarComponentProps) {
      super(props);
      this.checkAuthentication = checkAuthentication.bind(this);
    }

    public componentDidMount() {
      this.checkAuthentication();
    }
    public componentDidUpdate() {
      this.checkAuthentication();
    }
    public render(): JSX.Element {
      const { classes, bgWhite, miniActive, auth } = this.props;
      const { openAvatar, authenticated, userInfo } = this.state;
      const userWrapperClass = classes.user + ' ' + cx({ [classes.whiteAfter]: bgWhite });
      const displayName: string = userInfo ? userInfo.name : 'Loading User';
      const itemText =
        classes.itemText +
        ' ' +
        cx({
          [classes.itemTextMini]: miniActive,
        });
      const collapseItemText =
        classes.collapseItemText +
        ' ' +
        cx({
          [classes.collapseItemTextMini]: miniActive,
        });
      if (!authenticated) {
        return null;
      }

      return (
        <div className={userWrapperClass}>
          <div className={classes.photo}>
            <img src={avatar} className={classes.avatarImg} alt="..." />
          </div>
          <List className={classes.list}>
            <ListItem className={classes.item + ' ' + classes.userItem}>
              <NavLink
                to={'#'}
                className={classes.itemLink + ' ' + classes.userCollapseButton}
                onClick={() => this.setState({ openAvatar: !openAvatar })}
              >
                <ListItemText
                  primary={displayName}
                  secondary={
                    <b
                      className={
                        classes.caret +
                        ' ' +
                        classes.userCaret +
                        ' ' +
                        (openAvatar ? classes.caretActive : '')
                      }
                    />
                  }
                  disableTypography={true}
                  className={itemText + ' ' + classes.userItemText}
                />
              </NavLink>
              <Collapse in={this.state.openAvatar} unmountOnExit>
                <List className={classes.list + ' ' + classes.collapseList}>
                  <ListItem className={classes.collapseItem}>
                    <NavLink to="#" className={classes.itemLink + ' ' + classes.userCollapseLinks}>
                      <span className={classes.collapseItemMini}>{'MP'}</span>
                      <ListItemText
                        primary={'My Profile'}
                        disableTypography={true}
                        className={collapseItemText}
                      />
                    </NavLink>
                  </ListItem>
                  <ListItem className={classes.collapseItem}>
                    <NavLink to="#" className={classes.itemLink + ' ' + classes.userCollapseLinks}>
                      <span className={classes.collapseItemMini}>{'EP'}</span>
                      <ListItemText
                        primary={'Edit Profile'}
                        disableTypography={true}
                        className={collapseItemText}
                      />
                    </NavLink>
                  </ListItem>
                  <ListItem className={classes.collapseItem}>
                    <NavLink to="#" className={classes.itemLink + ' ' + classes.userCollapseLinks}>
                      <span className={classes.collapseItemMini}>{'S'}</span>
                      <ListItemText
                        primary={'Settings'}
                        disableTypography={true}
                        className={collapseItemText}
                      />
                    </NavLink>
                  </ListItem>
                  <ListItem className={classes.collapseItem}>
                    <a
                      href="javascript:void(0)"
                      className={classes.itemLink + ' ' + classes.userCollapseLinks}
                      onClick={e => {
                        auth.logout('/');
                      }}
                    >
                      <span className={classes.collapseItemMini}>{'LO'}</span>
                      <ListItemText
                        primary={'Logout'}
                        disableTypography={true}
                        className={collapseItemText}
                      />
                    </a>
                  </ListItem>
                </List>
              </Collapse>
            </ListItem>
          </List>
        </div>
      );
    }
  }
);
