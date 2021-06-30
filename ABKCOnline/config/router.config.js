export default [
  // user
  {
    path: '/user',
    component: '../layouts/UserLayout',
    routes: [{
      path: '/user',
      redirect: '/user/login'
    },
    {
      path: '/user/login',
      component: './User/Login'
    },
    {
      path: '/user/register',
      component: './User/Register'
    },
    {
      path: '/user/register-result',
      component: './User/RegisterResult'
    },
    ],
  },
  // app
  {
    path: '/',
    component: '../layouts/BasicLayout',
    Routes: ['src/pages/Authorized'],
    // authority: ['ABKCOffice', 'Owners', 'Representatives'],
    routes: [
      // dashboard
      {
        path: '/',
        redirect: '/dashboard'
      },
      {
        path: '/dashboard',
        name: 'Dashboard',
        icon: 'dashboard',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
        component: './Dashboard/Analysis',
      },
      // forms
      {
        path: '/pedigree-registration-form',
        component: './Forms/PedigreeRegistrationForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      {
        path: '/litter-registration-form',
        component: './Forms/LitterRegistrationForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      {
        path: '/junior-handlers-registration-form',
        component: './Forms/JuniorHandlersRegistrationForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      {
        path: '/puppy-registration-form',
        component: './Forms/PuppyRegistrationForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      {
        path: '/registration-list',
        name: 'Registration List',
        icon: 'bars',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
        component: './Forms/RegistrationList',
      },
      {
        path: '/transfer-registration-form',
        // name: 'Litter Registration',
        // icon: 'bars',
        component: './Forms/TransferRegistrationForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      {
        path: '/bully-request-form',
        // name: 'Litter Registration',
        // icon: 'bars',
        component: './Forms/BullyRequestForm',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
      },
      //  list
      {
        path: '/registrationList',
        name: 'Pending Registrations',
        icon: 'table',
        authority: ['ABKCOffice', 'Administrators', 'Representatives', 'Owners'],
        component: './List/PendingRegistrationList',
      },
      {
        path: '/manageRepresentatives',
        name: 'Representative List',
        icon: 'user',
        authority: ['ABKCOffice', 'Administrators'],
        component: './Profile/RepresentativeProfile',
      },
      {
        path: '/manageOwners',
        name: 'Owner List',
        icon: 'idcard',
        authority: ['ABKCOffice', 'Administrators'],
        component: './Profile/OwnerProfile',
      },
      {
        path: '/pending-users',
        name: 'Pending Users',
        icon: 'exclamation-circle',
        authority: ['ABKCOffice', 'Administrators'],
        component: './Profile/PendingUsers',
      },
      {
        component: '404',
      },
    ],
  },
];