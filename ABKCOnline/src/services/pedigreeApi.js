import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function startRegistration(params) {
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
  return request(`/api/v1/PedigreeRegistration/StartPedigreeRegistration`, requestOptions);
}

export async function draftRegistration(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: {
      ...params
    }
  };
  return request(`/api/v1/PedigreeRegistration`, requestOptions);
}

export async function editDraftRegistration(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: params.info

  };
  return request(`/api/v1/PedigreeRegistration/${params.currentRegId}`, requestOptions);
}
export async function getPedigreeRegistrations() {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/`, requestOptions);
}

export async function requestDeletePedigree(params) {
  const tkn = token();

  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PedigreeRegistration/${params}`, requestOptions);
}

export async function sidePanelListPedigree(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/v1/PedigreeRegistration/${params}`, requestOptions)
}


export async function getRegInfoCall(payload) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PedigreeRegistration/${payload.id}`, requestOptions)
}

export async function generatePedigreePDF(abkcDogId) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    responseType: 'blob',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PedigreeRegistration/PedigreePDFFromIdNumber/${abkcDogId}?useNewSystem=true`, requestOptions)

}
