import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

// eslint-disable-next-line import/prefer-default-export
export async function startBullyRequest(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    data: {}
  };
  return request(`/api/BullyID/RequestBullyId?dogId=${params.originalTableId}`, requestOptions);
}

export async function submitBullyRegistration(params) {
  const tkn = token();
  const data = params;
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: {
      ...data
    }
  };
  return request(`/api/v1/registrations/${params.currentRegId}/officeSubmit?registrationType=${params.registrationType}&cashPaid=0`, requestOptions);
}

export async function queryDeleteBullyRequest(params) {
  const tkn = token();
  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
  };
  return request(`/api/BullyID/${params}`, requestOptions);
}

export async function sidePanelListBully(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/BullyID/${params}`, requestOptions)
}