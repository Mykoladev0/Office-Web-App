import React from 'react';
import ReactDOM from 'react-dom';
import BrowserRouter from 'react-router-dom';

import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';
import App from './app';
// import bannerIcon1 from '../../Content/images/banner1.svg';
// const vsImg: HTMLImageElement = document.getElementById('VSImage') as HTMLImageElement;
// vsImg.src = bannerIcon1;

//configure logrocket
LogRocket.init('bju6uk/abkc_dev');
setupLogRocketReact(LogRocket);

//wire up parent react component to the root div
ReactDOM.render(
  <BrowserRouter>
    <App />
  </BrowserRouter>,
  document.getElementById('root')
);
