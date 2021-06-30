import axios from 'axios';

import {
  saveClassTemplates,
  retrieveClassTemplates,
  saveStyles,
  retrieveStyles,
  saveBreeds,
  retrieveBreeds,
} from '../database/wireindexdb';
// import { BaseServerResponse } from '../Models/BaseServerResponse';
// import { ShowLookupInfo } from '../Models/Shows';
let baseURL: string;

if (process.env.REACT_APP_API_URL && process.env.REACT_APP_API_URL !== '') {
  baseURL = `${process.env.REACT_APP_API_URL}/api/breeds`;
} else if ((<any>window).coreApp) {
  baseURL = (<any>window).coreApp.showsApi.baseUrl;
}
const endpoint = axios.create({
  baseURL,
});

export function getClassTemplates(): Promise<any> {
  return endpoint
    .get(`/GetClassTemplates`)
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //save data to the indexdb
      saveClassTemplates(data)
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return { classTemplates: data, count: total };
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving class templates to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      return { classTemplates: data, count: total };
    })
    .catch(error => {
      console.log(error);
      //try to get the shows from the local db
      return retrieveClassTemplates().then(offlineData => {
        if (!offlineData) {
          offlineData = [];
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return { classTemplates: offlineData, count: offlineData.length };
      });
      //return Promise.reject(error.response);
    });
}

export function getStyles(): Promise<any> {
  return endpoint
    .get(`/GetStyles`)
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //save data to the indexdb
      saveStyles(data)
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return { styles: data, count: total };
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving styles to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      return { styles: data, count: total };
    })
    .catch(error => {
      console.log(error);
      //try to get the shows from the local db
      return retrieveStyles().then(offlineData => {
        if (!offlineData) {
          offlineData = [];
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return { styles: offlineData, count: offlineData.length };
      });
      //return Promise.reject(error.response);
    });
}

export function getBreeds(): Promise<any> {
  return endpoint
    .get(`/GetBreeds`)
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //save data to the indexdb
      saveBreeds(data)
        .then(() => {
          //TODO: set last update (with id?) of save in cache
          return { breeds: data, count: total };
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving styles to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      return { breeds: data, count: total };
    })
    .catch(error => {
      console.log(error);
      //try to get the shows from the local db
      return retrieveBreeds().then(offlineData => {
        if (!offlineData) {
          offlineData = [];
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return { breeds: offlineData, count: offlineData.length };
      });
      //return Promise.reject(error.response);
    });
}
