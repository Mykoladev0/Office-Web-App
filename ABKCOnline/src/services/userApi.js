import request from '@/utils/request';

// const token = localStorage.getItem('user-token');
const token = () => localStorage.getItem('user-token');

export async function query() {
  return request('/api/users');
}

export async function queryCurrent() {
  // return request('/api/currentUser');
  return queryCurrentUser();

}

export async function queryCurrentUser() {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Account/GetCurrentUserInformation`, requestOptions);
}

export async function denyPendingUser(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/remove/${params}`, requestOptions);
}

export async function resetPassword(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/${params}/resetpassword`, requestOptions);
}

export async function resendActivationEmail(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/${params}/resendActivationEmail`, requestOptions);
}
