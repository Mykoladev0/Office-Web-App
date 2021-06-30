import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function startJuniorHandlersRegistrationForms(params) {
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
  return request(`/api/JuniorHandlers/registrations/StartRegistration`, requestOptions);
}

export async function editJuniorHandlersRegistrationForms(params) {
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
  return request(`/api/JuniorHandlers/registrations`, requestOptions);
}

export async function submitJuniorHandlersRegistrationForms(params) {
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

export async function getJuniorHandlerById(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/JuniorHandlers/registrations/${params.id}`, requestOptions);
}

export async function deleteJuniorHandlersById(params) {
  const tkn = token();
  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/JuniorHandlers/registrations/${params}`, requestOptions);
}

export async function generateJuniorHandlerCertificate(registrationId) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    responseType: 'blob',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/JuniorHandlers/CertificateFromRegistration/{id}/${registrationId}`, requestOptions)

}

export async function sidePanelListJunior(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/JuniorHandlers/registrations/${params}`, requestOptions)
}