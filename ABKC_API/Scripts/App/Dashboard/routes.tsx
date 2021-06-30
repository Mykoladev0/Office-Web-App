import Dashboard from '@material-ui/icons/Dashboard';
import Pets from '@material-ui/icons/Pets';

import Person from '@material-ui/icons/Person';
import People from '@material-ui/icons/People';
import EventNote from '@material-ui/icons/EventNote';
import ChildFriendly from '@material-ui/icons/ChildFriendly';
import Add from '@material-ui/icons/Add';
import ViewList from '@material-ui/icons/ViewList';

import DashboardPage from '../views/Dashboard/Dashboard';
import ProfilePage from '../views/Profile/Profile';
import { Sample } from '../views/Profile/Sample';
import { DogsViewFunc } from '../views/Dogs/DogsView';
import { OwnersViewFunc } from '../views/Owners/OwnerRouting';
export default [
  {
    path: '/',
    sidebarName: 'Dashboard',
    navbarName: 'Material Dashboard',
    name: 'Home',
    icon: Dashboard,
    component: DashboardPage,
  },
  {
    path: '/dogs',
    collapse: true,
    sidebarName: 'Dogs',
    navbarName: 'Dogs',
    mini: 'DOG',
    name: 'Dog Management',
    icon: Pets,
    component: DogsViewFunc,
    views: [
      {
        path: '/dogs',
        name: 'Dog List',
        sidebarName: 'DogList',
        navbarName: 'Dog List',
        mini: 'DL',
        icon: ViewList,
        component: DogsViewFunc,
      },
      {
        path: '/dogs/new',
        name: 'Add New Dog',
        sidebarName: 'NewDog',
        navbarName: 'New Dog',
        mini: '+',
        icon: Add,
        component: DogsViewFunc,
      },
    ],
  },
  {
    path: '/owners',
    collapse: true,
    sidebarName: 'Owners',
    navbarName: 'Owners',
    mini: 'OWN',
    name: 'Owner Management',
    icon: People,
    component: OwnersViewFunc,
    views: [
      {
        path: '/owners',
        name: 'Owner List',
        sidebarName: 'OwnerList',
        navbarName: 'Owner List',
        mini: 'OL',
        icon: ViewList,
        component: OwnersViewFunc,
      },
      {
        path: '/owners/new',
        name: 'Add New Owner',
        sidebarName: 'NewOwner',
        navbarName: 'New Owner',
        mini: '+',
        icon: Add,
        component: OwnersViewFunc,
      },
    ],
  },
  {
    path: '/home/shows',
    sidebarName: 'Shows',
    navbarName: 'Shows',
    name: 'Shows',
    mini: 'SA',
    icon: EventNote,
    component: Sample,
  },
  {
    path: '/home/litters',
    sidebarName: 'Litters',
    navbarName: 'Litters',
    name: 'Litters',
    mini: 'L',
    icon: ChildFriendly,
    component: Sample,
  },
  // {
  //   path: '/home/Profile',
  //   sidebarName: 'Profile',
  //   navbarName: 'Profile',
  //   mini: 'P',
  //   name: 'Profile',
  //   icon: Person,
  //   component: ProfilePage,
  // },

  { redirect: true, path: '/', to: '/', navbrName: 'Redirect' },
];
