import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function validateAbkcNumber(params) {
  const tkn = token();
  const data = params;
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
  };
  return request(`/api/v1/PuppyRegistration/searchForPuppyByABKCNumber?abkcNumber=${data.abkcNumber}`, requestOptions);
}

export async function startPuppyRegistrations(params) {
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
  return request(`/api/v1/PuppyRegistration/StartPuppyRegistration?dogId=${data.originalTableId}`, requestOptions);
}

// export async function submitPuppyRegistrationForms(params) {
//   const tkn = token();
//   const data = params;
//   const requestOptions = {
//     method: 'POST',
//     headers: {
//       accept: "application/json",
//       Authorization: `Bearer ${tkn}`
//     },
//     body: {
//       ...data
//     }
//   };
//   return request(`/api/v1/PuppyRegistration/${params.id}`, requestOptions);
// }

export async function draftPuppyRegistrationForms(params) {
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
  return request(`/api/v1/PuppyRegistration/${params.id}`, requestOptions);
}

export async function submitPuppyRegistrationForms(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/${params.currentRegId}/officeSubmit?registrationType=${params.registrationType}&cashPaid=0`, requestOptions);
}

export async function queryPuppyById(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PuppyRegistration/${params.id}`, requestOptions);
}

export async function queryDeletePuppyReg(params) {
  const tkn = token();
  const requestOptions = {
    method: 'DELETE',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PuppyRegistration/${params}`, requestOptions);
}

export async function generatePuppyCertificatePDF(registrationId) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    responseType: 'blob',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/PuppyRegistration/PermanentRegistrationCertificate/${registrationId}`, requestOptions)

}

export async function sidePanelListPuppy(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/PuppyRegistration/${params}`, requestOptions)
}