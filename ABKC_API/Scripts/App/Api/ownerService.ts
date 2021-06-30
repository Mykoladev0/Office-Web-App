import { formatNumber, CountryCode, parseNumber } from 'libphonenumber-js';

import axios from 'axios';
import { OwnerModel, OwnerLookupInfo } from '../Models/Owner';
import { BaseServerResponse } from '../Models/BaseServerResponse';
import { getAxiosHeader } from './baseAPI';

const endpoint = axios.create({
  // baseURL: (<any>window).coreApp.ownersApi.baseUrl,
  baseURL: '/api/owners',
});

export function getOwnerById(id: number): Promise<OwnerModel> {
  return endpoint.get(`/${id.toString()}`).then(({ data }) => {
    const rtnData = data as OwnerModel;
    if (rtnData) {
      let countryCode: string = rtnData.country != null ? rtnData.country : 'US';
      // countryCode = countryCode.substring(0, 2);
      rtnData.country = countryCode;

      if (rtnData.phone) {
        const parsedNumber = parseNumber(rtnData.phone, { extended: true });
        if (parsedNumber && parsedNumber.valid) {
          rtnData.phone = formatNumber(parsedNumber, 'International'); //formatNumber(rtnData.phone, countryCode as CountryCode, 'International');
        } else {
          console.log(`$bad phone number format for ${rtnData.ownerId}, ${rtnData.phone}.`);
        }
      }
      if (rtnData.cell) {
        const parsedNumber = parseNumber(rtnData.cell, { extended: true });
        if (parsedNumber && parsedNumber.valid) {
          rtnData.phone = formatNumber(parsedNumber, 'International'); //formatNumber(rtnData.phone, countryCode as CountryCode, 'International');
        } else {
          console.log(`$bad cell number format for ${rtnData.ownerId}, ${rtnData.phone}.`);
        }
      }
    }
    return rtnData;
  });
}
export async function getOwnersCount(): Promise<number> {
  const authHeader = await getAxiosHeader();
  return endpoint.get('/GetOwnersCount', authHeader).then(({ data }) => data as number);
}

export function saveOwner(owner: OwnerModel): Promise<OwnerModel> {
  if (owner.ownerId > 0) {
    return endpoint
      .put(`/${owner.ownerId.toString()}`, JSON.stringify(owner), {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .then(({ data }) => data as OwnerModel)
      .catch(error => {
        console.log(error);
        return Promise.reject(error.response);
      });
  } else {
    return endpoint
      .post('/', JSON.stringify(owner), {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .then(({ data }) => data as OwnerModel)
      .catch(error => {
        console.log(error);
        return Promise.reject(error.response);
      });
  }
}

export function searchOwnersWithSimpleReturn(
  searchText: string
): Promise<BaseServerResponse<OwnerLookupInfo[]>> {
  return endpoint
    .get(`/GetMatchingOwners`, {
      params: {
        searchText,
      },
    })
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      return { data: data as OwnerLookupInfo[], count: total };
    });
}

export function getOwnersTableData(
  page: number,
  pageSize: number,
  sorted: any[],
  filtered: any[],
  isExternalFilter: boolean = false
): Promise<BaseServerResponse<OwnerModel[]>> {
  /*
https://github.com/Biarity/Sieve#send-a-request
?sorts=     LikeCount,CommentCount,-created         // sort by likes, then comments, then descendingly by date created 
&filters=   LikeCount>10, Title@=awesome title,     // filter to posts with more than 10 likes, and a title that contains the phrase "awesome title"
&page=      1                                       // get the first page...
&pageSize=  10                                      // ...which contains 10 posts
*/
  const postObject: any = {
    page: page,
    pageSize: pageSize,
  };
  if (sorted && sorted.length > 0) {
    postObject.sorts = sorted
      .map(x => {
        return x.desc ? '-' + x.id : x.id;
      })
      .join(',');
  }
  if (filtered && filtered.length > 0) {
    postObject.filters = filtered
      .map(x => {
        return `${x.id}@=${x.value}`;
      })
      .join(',');
  }
  return endpoint
    .post(`/GetOwnersForTable?externalFilter=${isExternalFilter}`, JSON.stringify(postObject), {
      headers: {
        'Content-Type': 'application/json',
      },
    })
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      const rtn: BaseServerResponse<OwnerModel[]> = {
        data: data as OwnerModel[],
        count: total,
      };
      return rtn;
    })
    .catch(response => {
      return { data: [], count: 0 };
    });
}
