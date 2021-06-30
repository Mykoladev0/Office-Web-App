import {
  stringify
} from 'qs';
import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function queryProjectNotice() {
  return request('/api/project/notice');
}

export async function queryActivities() {
  return request('/api/activities');
}

export async function queryRule(params) {
  return request(`/api/rule?${stringify(params)}`);
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

export async function removeRule(params) {
  return request('/api/rule', {
    method: 'POST',
    body: {
      ...params,
      method: 'delete',
    },
  });
}

export async function addRule(params) {
  return request('/api/rule', {
    method: 'POST',
    body: {
      ...params,
      method: 'post',
    },
  });
}

export async function updateRule(params = {}) {
  return request(`/api/rule?${stringify(params.query)}`, {
    method: 'POST',
    body: {
      ...params.body,
      method: 'update',
    },
  });
}

// registration forms

export async function getBreedData() {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/Breeds/GetBreeds`, requestOptions);
}

export async function getColorData() {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/Dogs/GetColorList`, requestOptions);
}


export async function submitPaymentRegistration(params) {
  const registrationsToSubmit = params.regArray;
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: registrationsToSubmit
  };
  return request(`/api/v1/registrations/GetPaymentQuote`, requestOptions);
}
// export async function startRegistration(params) {
//   const tkn = token();

//   const data = params.dogInfo
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
//   return request(`/api/v1/PedigreeRegistration/StartPedigreeRegistration`, requestOptions);
// }

// export async function draftRegistration(params) {
//   const tkn = token();

//   const requestOptions = {
//     method: 'POST',
//     headers: {
//       accept: "application/json",
//       Authorization: `Bearer ${tkn}`
//     },
//     body: {
//       ...params
//     }
//   };
//   return request(`/api/v1/PedigreeRegistration`, requestOptions);
// }

// export async function editDraftRegistration(params) {
//   const tkn = token();

//   const requestOptions = {
//     method: 'POST',
//     headers: {
//       accept: "application/json",
//       Authorization: `Bearer ${tkn}`
//     },
//     body: params.info

//   };
//   return request(`/api/v1/PedigreeRegistration/${params.currentRegId}`, requestOptions);
// }

export async function uploadDocument(params) {
  const tkn = token();

  const data = new FormData();
  data.append('file', params.file)
  // data.append('id', params.id);
  data.append('documentType', params.documentType)
  data.append('registrationType', params.registrationType)
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: data
  };
  return request(`/api/v1/registrations/${params.id}/supportingdocument`, requestOptions);
}

export async function submitRegistration(params) {
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

export async function searchDamNameCall(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    // body: {searchText: params}
  };
  return request(`/api/Dogs/GetMatchingDogs?searchText=${params}`, requestOptions);
}

// eof

export async function fakeSubmitForm(params) {
  return request('/api/forms', {
    method: 'POST',
    body: params,
  });
}

export async function fakeChartData() {
  return Promise.resolve([]);
  // return request('/api/fake_chart_data');
}

export async function queryTags() {
  return request('/api/tags');
}

export async function queryBasicProfile() {
  return request('/api/profile/basic');
}

export async function queryAdvancedProfile() {
  return request('/api/profile/advanced');
}

export async function queryFakeList(params) {
  return request(`/api/fake_list?${stringify(params)}`);
}

// user accounts
export async function getPendingUsers() {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/pending`, requestOptions);
}

export async function activatePendingUser(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/activate/${params}`, requestOptions);
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

export async function suspendUserAccount(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/suspend/${params}`, requestOptions);
}

export async function unSuspendUserAccount(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Users/unSuspend/${params}`, requestOptions);
}

export async function issueRefundApi(params) {
  const tkn = token();
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/Payment/issueRefundForRegistration?registrationId=${params.regId}&registrationType=${params.regType}&reason=${params.commentValue}`, requestOptions);
}

export async function getRepresentatives() {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/users/representatives`, requestOptions);
}

export async function getSupportingDocs(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    responseType: 'blob',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/${params.id}/supportingDocument?documentType=${params.val}&regType=${params.regType}`, requestOptions);
}

export async function rejectRegistrations(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/reject/${params.id}?reasonForRejection=${params.val}&registrationType=${params.type}`, requestOptions);
}

export async function approveRegistrations(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/${params.id}/approve?comments=${params.val}&registrationType=${params.type}`, requestOptions);
}

export async function requestRegistrationsInfo(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/requestinfo/${params.id}?infoNeeded=${params.val}&registrationType=${params.type}`, requestOptions);
}

// export async function requestDeletePedigree(params) {
//   const tkn = token();

//   const requestOptions = {
//     method: 'DELETE',
//     headers: {
//       accept: "application/json",
//       Authorization: `Bearer ${tkn}`
//     }
//   };
//   return request(`/api/v1/PedigreeRegistration/${params}`, requestOptions);
// }

export async function filterRepData(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/getByRepresentative?id=${params.val}&pendingOnly=${params.pending}`, requestOptions);
}

// export async function sidePanelList(params) {
//   const tkn = token();

//   const requestOptions = {
//     method: 'GET',
//     headers: {
//       accept: "application/json",
//       Authorization: `Bearer ${tkn}`
//     }
//   };

//   return request(`/api/v1/PedigreeRegistration/${params}`, requestOptions)
// }

export async function searchOwnerList(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/v1/registrations/searchByOwner?searchTxt=${params.val}?pendingOnly=${params.pending}`, requestOptions)
}

export async function searchOwnerNameCall(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/Owners/GetMatchingOwners?searchText=${params}`, requestOptions)
}

export async function searchDogList(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/v1/registrations/searchByDogName?searchTxt=${params.val}?pendingOnly=${params.pending}`, requestOptions)
}

export async function filterData() {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };

  return request(`/api/v1/registrations/pending`, requestOptions)
}

export async function filterPendingId(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/user/${params}/pending`, requestOptions)
}

export async function getRegistrations(params) {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/user/${params}`, requestOptions)
}

export async function approveAllRegData(params) {
  const tkn = token();

  const requestOptions = {
    method: 'POST',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/approvalall/${params.val}`, requestOptions)
}

// update registration fee for rep
export async function updateRegistrationFeeCall(payload) {
  const tkn = token();
  const {
    updatedFees
  } = payload;
  const requestOptions = {
    method: 'PUT',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    },
    body: {
      ...updatedFees
    },
  };
  return request(`/api/v1/Users/representatives/${payload.id}/updateFees`, requestOptions)
}
// owners manage

export async function getOwners() {
  const tkn = token();
  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/users/owners`, requestOptions);
}

export async function getOwnerDetailsCall(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/Owners/${params}`, requestOptions);
}

export async function getRegOwners(params) {
  const tkn = token();

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: "application/json",
      Authorization: `Bearer ${tkn}`
    }
  };
  return request(`/api/v1/registrations/getByOwner?id=${params.val}&pendingOnly=${params.pending}`, requestOptions);
}

export async function removeFakeList(params) {
  const {
    count = 5, ...restParams
  } = params;
  return request(`/api/fake_list?count=${count}`, {
    method: 'POST',
    body: {
      ...restParams,
      method: 'delete',
    },
  });
}

export async function addFakeList(params) {
  const {
    count = 5, ...restParams
  } = params;
  return request(`/api/fake_list?count=${count}`, {
    method: 'POST',
    body: {
      ...restParams,
      method: 'post',
    },
  });
}

export async function updateFakeList(params) {
  const {
    count = 5, ...restParams
  } = params;
  return request(`/api/fake_list?count=${count}`, {
    method: 'POST',
    body: {
      ...restParams,
      method: 'update',
    },
  });
}

export async function accountLogin(params) {
  return request(`/api/v1/Account/GetToken`, {
    method: 'POST',
    body: params,
  });
}


export async function acountRegister(params) {
  const data = new FormData();
  data.append('EmailAddress', params.EmailAddress)
  data.append('RoleRequested', params.RoleRequested)
  data.append('firstName', params.firstName)
  data.append('lastName', params.lastName)
  return request(`/api/v1/Account/Register`, {
    method: 'POST',
    body: data,
  });
}

export async function queryNotices(params = {}) {
  return request(`/api/notices?${stringify(params)}`);
}

export async function getFakeCaptcha(mobile) {
  return request(`/api/captcha?mobile=${mobile}`);
}
