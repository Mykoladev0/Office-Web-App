import request from '@/utils/request';

const token = () => localStorage.getItem('user-token');

export async function searchDogNameCall(searchText: string) {
  const tkn = token();

  if (searchText.length < 3) {
    return [];
    // return new Promise((resolve) => {
    //   resolve([]);
    // });
  }

  const requestOptions = {
    method: 'GET',
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${tkn}`,
    },
    // body: {searchText: params}
  };
  return request(`/api/Dogs/GetMatchingDogs?searchText=${searchText}`, requestOptions);
}
