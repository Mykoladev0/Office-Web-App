import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function getDogsByABKCNumbers(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
  };
  return request(`/api/Dogs/GetDogsByABKCNumber?abkcNo=${params.abkcNumber}`, requestOptions);
}

export async function startTransferRegistrations(params) {
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
  return request(`/api/v1/Transfers/requests/StartTransferRequest?dogId=${params.originalTableId}`, requestOptions);
}

export async function submitTransferRegistration(params) {
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

export async function draftTransferRegistration(params) {
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
  return request(`/api/v1/Transfers/requests/${params.id}`, requestOptions);
}

export async function queryTransferById(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Transfers/requests/${params.id}`, requestOptions);
}

export async function queryDeleteTransferReg(params) {
  const tkn = token();
  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Transfers/requests/${params}`, requestOptions);
}

export async function sidePanelListTransfer(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/v1/Transfers/requests/${params}`, requestOptions)
}