import axios from 'axios';
import moment from 'moment';
import {
  saveShows,
  retrieveShows,
  retrieveShowById,
  saveShowEventsLocally,
  registerDogLocally,
  saveParticipantsWithDogInfo,
  getShowParticipants,
} from '../database/wireindexdb';
import { BasicDogLookupInfo } from '../models/dog';
import { BaseServerResponse } from '../models/BaseServerResponse';
import { ShowParticipantInfo } from '../models/show';

// import { BaseServerResponse } from '../Models/BaseServerResponse';
// import { ShowLookupInfo } from '../Models/Shows';
let baseURL: string;

if (process.env.REACT_APP_API_URL && process.env.REACT_APP_API_URL !== '') {
  baseURL = `${process.env.REACT_APP_API_URL}/api/shows`;
} else if ((<any>window).coreApp) {
  baseURL = (<any>window).coreApp.showsApi.baseUrl;
}
const endpoint = axios.create({
  baseURL,
});

export function getShowsCount(): Promise<number> {
  return endpoint.get('/GetShowsCount').then(({ data }) => data as number);
}

export function getUpcomingShows(maxCount: number = 30): Promise<any> {
  const now = moment.utc().format('MM-DD-YYYY'); //get now in UTC
  return endpoint
    .get(`/GetUpcomingShows`, {
      params: {
        startDate: now,
        maxCount,
      },
    })
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //save data to the indexdb
      saveShows(data)
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return { data: data, count: total };
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
      return retrieveShows().then(offlineData => {
        if (!offlineData) {
          offlineData = [];
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return { data: offlineData, count: offlineData.length };
      });
      //return Promise.reject(error.response);
    });
}

export function getShowDetails(showId): Promise<any> {
  // const now = moment.utc().format('MM-DD-YYYY'); //get now in UTC
  return endpoint
    .get(`/${showId}`)
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      //save data to the indexdb
      saveShows([data])
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          return { data: data, count: total };
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
      return retrieveShowById(showId).then(offlineData => {
        if (!offlineData) {
          offlineData = null;
          //messageNoData();
        } else {
          //should let user know data came from cache
          //messageOffline();
          //updateUI(offlineData);
        }
        return { data: offlineData };
      });
      //return Promise.reject(error.response);
    });
}

export function saveShowEvents(showId: number, eventDetails: any): Promise<any> {
  //get results out of event details
  //post or put each one depending on if it has an id > 0
  //take result and add it to show at the appropriate event in the indexdb
  const {
    classTemplate: { classId },
    style: { id },
    results,
    breed: { breed },
  } = eventDetails;
  if (!results || results.length === 0) {
    return null;
  }
  return SaveResults(showId, classId, id, results).then(finalResults =>
    saveShowEventsLocally(
      showId,
      eventDetails.classTemplate.name,
      breed,
      eventDetails.gender,
      eventDetails.style.styleName,
      finalResults
    ).then(() => finalResults)
  );
}

async function SaveResults(
  showId: number,
  classId: number,
  styleId: number,
  results: any
): Promise<EventResult[]> {
  const finalResults: EventResult[] = [];
  for (const r of results) {
    if (Object.keys(r).length > 0) {
      const { armbandNumber, dogId, points } = r;
      const payload = {
        showId,
        resultId: r.Id != null ? r.Id : 0,
        classId,
        styleId,
        armbandNumber,
        dogId,
        points,
      };
      const d = await SaveResult(payload);
      finalResults.push(d);
    }
  }
  return finalResults;
}

async function SaveResult(payload: any) {
  if (payload.id && payload.id > 0) {
    return await endpoint
      .put(`/${payload.id.toString()}`, JSON.stringify(payload))
      .then(({ data }) => data)
      .catch(error => {
        //still need to save it locally WITHOUT an id
        return payload;
      });
  } else {
    return await endpoint
      .post('/', JSON.stringify(payload), {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .then(({ data }) => data)
      .catch(error => {
        //still need to save it locally WITHOUT an id and other details
        return payload;
      });
  }
}

//register dog for show

export function registerDogForShow(
  showId: number,
  dog: BasicDogLookupInfo,
  armbandNumber: number
): Promise<any> {
  if (!dog) {
    return null;
  }
  return RegisterDogOnServer(showId, dog, armbandNumber).then(registeredDog =>
    registerDogLocally(registeredDog).then(() => registeredDog)
  );
}
async function RegisterDogOnServer(showId: number, dog: BasicDogLookupInfo, armbandNumber: number) {
  const payload: any = {
    id: 0, //placeholder
    dogId: dog.id,
    showId,
    armbandNumber,
  };
  return await endpoint
    .post('/RegisterForShow', JSON.stringify(payload), {
      headers: {
        'Content-Type': 'application/json',
      },
    })
    .then(({ data }) => data)
    .catch(error => {
      //still need to save it locally WITHOUT an id and other details
      return payload;
    });
}

export function searchParticipants(
  showId: number,
  searchText: string
): Promise<BaseServerResponse<ShowParticipantInfo[]>> {
  return endpoint
    .get(`/GetParticipantsForShow`, {
      params: {
        showId,
      },
    })
    .then(({ data, headers }) => {
      // const total: number = headers['x-total-count'];
      //return { data: data as BasicDogLookupInfo[], count: total };
      return saveParticipantsWithDogInfo(data)
        .then(() => {
          //TODO: set last update (with id?) of shows save in cache
          data = filterParticipants(searchText, data);
          return { data: data as ShowParticipantInfo[], count: data.length };
        })
        .catch(err => {
          //record error somehow?
          console.log(`Error ${err} saving participants to indexDB`);
          // return { data: data, count: total };
          return Promise.reject(err.response);
        });
      // return { data: data, count: total };
    })
    .catch(error => {
      console.log(error);
      //try to get the shows from the local db
      return getShowParticipants(showId).then(participants => {
        participants = filterParticipants(searchText, participants);
        return { data: participants, count: participants.length };
      });
      //return Promise.reject(error.response);
    });
}

function filterParticipants(searchText, dogList: ShowParticipantInfo[]) {
  if (!dogList || !Array.isArray(dogList)) {
    return [];
  }
  const filtered = dogList.filter(p => {
    const d = p.dog;
    let found: boolean =
      d.dogName.toLowerCase().startsWith(searchText.toLowerCase()) ||
      (d.abkcNo && d.abkcNo.startsWith(searchText)) ||
      d.bullyId.toString().startsWith(searchText);
    found = found || (p.armbandNumber && p.armbandNumber.toString().startsWith(searchText));
    return found;
  });
  return filtered;
}
