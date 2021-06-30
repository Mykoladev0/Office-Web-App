import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function startLitterRegistrations(params) {
  const tkn = token();
  const data = params.dogInfo
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
  return request(`/api/Litters/StartRegistration?damId=${data.damId}&sireId=${data.sireId}`, requestOptions);
}

export async function draftLitterRegistrationForms(params) {
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
  return request(`/api/Litters/${params.id}`, requestOptions);
}

export async function submitLitterRegistrationForms(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
  };
  return request(`/api/v1/registrations/${params.currentRegId}/officeSubmit?registrationType=${params.registrationType}&cashPaid=0`, requestOptions);
}

export async function queryLitterById(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/Litters/${params.id}`, requestOptions);
}

export async function queryDeleteReg(params) {
  const tkn = token();
  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/Litters/${params}`, requestOptions);
}

export async function generateLitterReport(litterRegId) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    responseType: 'blob',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/litters/LitterReportFromRegistration/${litterRegId}`, requestOptions)
}

export async function sidePanelListLitter(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/Litters/${params}`, requestOptions)
}