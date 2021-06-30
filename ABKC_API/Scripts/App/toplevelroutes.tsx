import App from './app';
// import LoginPage from '../../Vendor/mr-pro/views/Pages/LoginPage.jsx';
// import LoginPage from './views/Authentication/Login';
import RegisterPage from '../../Vendor/mr-pro/views/Pages/RegisterPage.jsx';

//these are top level routes for the application
export const indexRoutes = [
  // { path: "/login", component: LoginPage },
  { path: '/register', component: RegisterPage },
  { path: '/', component: App },
];
