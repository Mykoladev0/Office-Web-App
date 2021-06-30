import { AxiosPromise } from 'axios';
//okta libraries
import OktaAuth from '@okta/okta-auth-js';
import config from '../../App/app.config';

async function getAxiosHeader() {
  let axiosConfig = {};

  //   const user = JSON.parse(localStorage.getItem("user")) || null;
  const authClient = new OktaAuth(config);
  const userToken = await getAuthTokenFromSession();
  if (userToken) {
    axiosConfig = {
      headers: getAuthHeader(userToken),
    };
  }
  return axiosConfig;
}
async function getAuthTokenFromSession() {
  const authClient = new OktaAuth(config);
  const token = await authClient.tokenManager.get('idToken');
  //   return localStorage.getItem('user_Token');
  return token;
}

function getAuthHeader(userToken: any) {
  if (userToken) {
    return { Authorization: `Bearer ${userToken.idToken}` };
  }
  return {};
}

function handleAuthorizedResponse(apiResponse: AxiosPromise<any>) {
  return apiResponse
    .then(({ data }) => data)
    .catch(({ response, message }) => {
      //unauthorized, log out?
      if (response.status === 401) {
        // auto logout if 401 response returned from api
        // logoutUser();
        location.reload(true); //force a refresh?
      }
      const error = (response.data && message) || response.statusText;
      return Promise.reject(error);
    });
}
// function logoutUser() {
//   // remove user from local storage to log user out
//   //   localStorage.removeItem("user");
//   removeUserFromCache();
// }
export { getAxiosHeader, handleAuthorizedResponse, getAuthHeader };
