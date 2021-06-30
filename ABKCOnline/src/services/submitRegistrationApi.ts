import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function checkRegistrationsForValidity(registrations: any[]) {
  const tkn = token();
  const data = registrations.map(r => {
    return { registrationId: r.id, registrationType: r.registrationType };
  });
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${tkn}`,
    },
    body: data,
  };
  return request(`/api/v1/registrations/checkRegistrationsForValidity`, requestOptions);
}

export async function getPaymentQuote(registrations: any[]) {
  const tkn = token();
  const data = registrations
    .filter(r => r.isValid)
    .map(r => {
      return { registrationId: r.id, registrationType: r.registrationType };
    });
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${tkn}`,
    },
    body: data,
  };
  return request(`/api/v1/registrations/GetPaymentQuote`, requestOptions);
}

export async function finalizeTransaction(
  registrations: any[],
  stripeToken: string,
  totalAmount: number
) {
  const tkn = token();
  const data = {
    tokenId: stripeToken,
    amount: totalAmount,
    registrations,
    // .filter(r => r.isValid)
    // .map(r => {
    //   return { registrationId: r.id, registrationType: r.registrationType };
    // }),
  };
  const requestOptions = {
    method: 'POST',
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${tkn}`,
    },
    body: data,
  };
  return request(`/api/v1/Payment/finalizeTransaction`, requestOptions);
}
