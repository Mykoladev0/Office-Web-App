import axios from 'axios';
import moment from 'moment';
import { BaseServerResponse } from '../Models/BaseServerResponse';
import { ShowLookupInfo } from '../Models/Shows';
import { getAxiosHeader } from './baseAPI';

const endpoint = axios.create({
  // baseURL: (<any>window).coreApp.showsApi.baseUrl,
  baseURL: '/api/shows',
});

export async function getShowsCount(): Promise<number> {
  //
  const authHeader = await getAxiosHeader();
  return endpoint.get('/GetShowsCount', authHeader).then(({ data }) => data as number);
  // return authEndpoint().then(client =>
  //   client.get('/GetShowsCount').then(({ data }) => data as number)
  // );
}

export async function getUpcomingShows(
  maxCount: number = 4
): Promise<BaseServerResponse<ShowLookupInfo[]>> {
  const now = moment.utc().format('MM-DD-YYYY'); //get now in UTC
  const config = await getAxiosHeader();
  config['params'] = {
    startDate: now,
    maxCount,
  };
  return endpoint
    .get(
      `/GetUpcomingShows`,
      config
      // {
      //   params: {
      //     startDate: now,
      //     maxCount,
      //   },
      // },
    )
    .then(({ data, headers }) => {
      const total: number = headers['x-total-count'];
      return { data: data as ShowLookupInfo[], count: total };
    })
    .catch(error => {
      console.log(error);
      return Promise.reject(error.response);
    });
}
