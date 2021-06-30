import React, { Component } from 'react';
import List from '@material-ui/core/List';
import { NavLink } from 'react-router-dom';

import cx from 'classnames';
import { WithStyles, withStyles } from '@material-ui/core/styles';

import sidebarStyle from './sidebarStyle';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import { Collapse } from '@material-ui/core';

export interface NavigationLinksCompProps extends WithStyles<typeof sidebarStyle> {
  routes: any[];
  miniActive: boolean;
  curPath: string; //props.location.pathname
  color: string;
}

export interface NavigationLinksCompState {}

export default class NavigationLinksComp extends Component<
  NavigationLinksCompProps,
  NavigationLinksCompState
> {
  public state: NavigationLinksCompState = {};

  public constructor(props: NavigationLinksCompProps) {
    super(props);
    this.openCollapse = this.openCollapse.bind(this);
  }

  public render(): JSX.Element {
    const { classes, routes, miniActive, color } = this.props;
    return (
      <List className={classes.list}>
        {routes.map((prop, key) => {
          if (prop.redirect) {
            return null;
          }
          if (prop.collapse) {
            const navLinkClasses =
              classes.itemLink +
              ' ' +
              cx({
                [' ' + classes.collapseActive]: this.activeRoute(prop.sidebarName),
              });
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
            const itemIcon = classes.itemIcon;
            const caret = classes.caret;
            return (
              <ListItem key={key} className={classes.item}>
                <NavLink
                  to={'#'}
                  className={navLinkClasses}
                  onClick={() => this.openCollapse(prop.sidebarName)}
                >
                  <ListItemIcon className={itemIcon}>
                    <prop.icon />
                  </ListItemIcon>
                  <ListItemText
                    primary={prop.name}
                    secondary={
                      <b
                        className={
                          caret + ' ' + (this.state[prop.sidebarName] ? classes.caretActive : '')
                        }
                      />
                    }
                    disableTypography={true}
                    className={itemText}
                  />
                </NavLink>
                <Collapse in={this.state[prop.sidebarName]} unmountOnExit>
                  <List className={classes.list + ' ' + classes.collapseList}>
                    {prop.views.map((prop2, key2) => {
                      if (prop2.redirect) {
                        return null;
                      }
                      const navLinkClasses2 =
                        classes.collapseItemLink +
                        ' ' +
                        cx({ [' ' + classes[color]]: this.activeRoute(prop2.path) });
                      const collapseItemMini = classes.collapseItemMini;
                      return (
                        <ListItem key={key2} className={classes.collapseItem}>
                          <NavLink to={prop2.path} className={navLinkClasses2}>
                            <span className={collapseItemMini}>{prop2.mini}</span>
                            <ListItemText
                              primary={prop2.name}
                              disableTypography={true}
                              className={collapseItemText}
                            />
                          </NavLink>
                        </ListItem>
                      );
                    })}
                  </List>
                </Collapse>
              </ListItem>
            );
          }
          const navLinkClasses3 =
            classes.itemLink +
            ' ' +
            cx({
              [' ' + classes[color]]: this.activeRoute(prop.path),
            });
          const itemText3 =
            classes.itemText +
            ' ' +
            cx({
              [classes.itemTextMini]: miniActive,
            });
          return (
            <ListItem key={key} className={classes.item}>
              <NavLink to={prop.path} className={navLinkClasses3}>
                <ListItemIcon className={classes.itemIcon}>
                  <prop.icon />
                </ListItemIcon>
                <ListItemText primary={prop.name} disableTypography={true} className={itemText3} />
              </NavLink>
            </ListItem>
          );
        })}
      </List>
    );
  }
  private openCollapse(collapse) {
    /*
          var st = {};
    st[collapse] = !this.state[collapse];
    this.setState(st);
      */
    const collapseVal = !this.state[collapse];
    const st = {};
    st[collapse] = collapseVal;
    this.setState(st);
  }
  // verifies if routeName is the one active (in browser input)
  private activeRoute(routeName): boolean {
    const { curPath } = this.props;
    return curPath.indexOf(routeName) > -1;
  }
}
