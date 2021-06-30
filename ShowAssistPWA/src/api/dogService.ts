import axios from 'axios';
import { BasicDogLookupInfo } from '../models/dog';
import { BaseServerResponse } from '../models/BaseServerResponse';
import { saveDogs, retrieveDogById, retrieveDogs } from '../database/wireindexdb';

let baseURL: string;

if (process.env.REACT_APP_API_URL && process.env.REACT_APP_API_URL !== '') {
  baseURL = `${process.env.REACT_APP_API_URL}/api/dogs`;
} else if ((<any>window).coreApp) {
  baseURL = (<any>window).coreApp.showsApi.baseUrl;
}
const endpoint = axios.create({
  baseURL,
});

export function getDogById(id: number): Promise<BasicDogLookupInfo> {
  return endpoint
    .get(`/${id.toString()}`)
    .then(({ data }) => {
      //data as BasicDogLookupInfo
      //save data to the indexdb
      saveDogs([data])
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return data as BasicDogLookupInfo;
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving shows to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      return data as BasicDogLookupInfo;
    })
    .catch(error => {
      //try to get the shows from the local db
      return retrieveDogById(id).then(offlineData => {
        if (!offlineData) {
          offlineData = null;
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return offlineData as BasicDogLookupInfo;
      });
      //return Promise.reject(error.response);
    });
}

export function searchDogsWithSimpleReturn(
  searchText: string
): Promise<BaseServerResponse<BasicDogLookupInfo[]>> {
  return endpoint
    .get(`/GetMatchingDogs`, {
      params: {
        searchText,
      },
    })
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //return { data: data as BasicDogLookupInfo[], count: total };
      saveDogs(data)
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return { data: data as BasicDogLookupInfo[], count: total };
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving shows to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      return { data: data, count: total };
    })
    .catch(error => {
      console.log(error);
      //try to get the shows from the local db
      return filterOfflineDogs(searchText, false);
      //return Promise.reject(error.response);
    });
}

export function filterDogs(searchText, dogList: BasicDogLookupInfo[]) {
  if (!dogList || !Array.isArray(dogList)) {
    return [];
  }
  return dogList.filter(d => {
    let found: boolean =
      d.dogName.toLowerCase().startsWith(searchText.toLowerCase()) ||
      (d.abkcNo && d.abkcNo.startsWith(searchText)) ||
      d.bullyId.toString().startsWith(searchText);
    if (d.hasOwnProperty('armbandNumber')) {
      found = found || (d['armbandNumber'] && d['armbandNumber'].toString().startsWith(searchText));
    }
    return found;
  });
}

const filterOfflineDogs = (
  searchText,
  searchAllDogs: boolean = false
): Promise<BaseServerResponse<BasicDogLookupInfo[]>> => {
  return retrieveDogs().then(offlineData => {
    if (!offlineData) {
      offlineData = [];
      //messageNoData();
    }
    offlineData = filterDogs(searchText, offlineData);
    // offlineData = offlineData.filter(
    //   d =>
    //     d.dogName.toLowerCase().startsWith(searchText.toLowerCase()) ||
    //     (d.abkcNo && d.abkcNo.startsWith(searchText)) ||
    //     d.bullyId.toString().startsWith(searchText)
    // );
    return { data: offlineData as BasicDogLookupInfo[], count: offlineData.length };
  });
};
