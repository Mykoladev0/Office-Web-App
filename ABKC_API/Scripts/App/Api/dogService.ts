import axios from 'axios';
import { DogModel, BasicDogLookupInfo } from '../Models/Dog';
import { BaseServerResponse } from '../Models/BaseServerResponse';
import { getAxiosHeader } from './baseAPI';

const endpoint = axios.create({
  // baseURL: (<any>window).coreApp.dogsApi.baseUrl,
  baseURL: '/api/dogs',
});

export async function getDogById(id: number): Promise<DogModel> {
  const authHeader = await getAxiosHeader();
  return endpoint.get(`/${id.toString()}`, authHeader).then(({ data }) => data as DogModel);
}

export async function saveDog(dog: DogModel): Promise<DogModel> {
  const config = await getAxiosHeader();

  if (dog.id > 0) {
    return endpoint
      .put(`/${dog.id.toString()}`, JSON.stringify(dog), config)
      .then(({ data }) => data as DogModel);
  } else {
    config['Content-Type'] = 'application/json';
    return endpoint
      .post(
        '/',
        JSON.stringify(dog),
        config
        //{
        // headers: {
        //   'Content-Type': 'application/json',
        // },
        //}
      )
      .then(({ data }) => data as DogModel);
  }
}
export async function getDistinctColors(): Promise<string[]> {
  const config = await getAxiosHeader();

  return endpoint.get(`/GetColorList`, config).then(({ data }) => data as string[]);
}

export async function getAllDogsCount(): Promise<number> {
  const config = await getAxiosHeader();
  return endpoint.get(`/GetAllDogsCount`, config).then(({ data }) => data as number);
}

export async function getDogsRecordCount(filters: any): Promise<number> {
  const config = await getAxiosHeader();
  config['Content-Type'] = 'application/json';
  const postData: any = {};
  if (filters && filters !== '') {
    postData.filters = filters;
  }
  return endpoint
    .post(
      `/GetDogsCount`,
      JSON.stringify(postData),
      config
      // {
      //   headers: {
      //     'Content-Type': 'application/json',
      //   },
      // }
    )
    .then(({ data }) => data as number);
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
      return { data: data as BasicDogLookupInfo[], count: total };
    });
}

//handleRetrievedDataFn: (response: any)=>{}
// tslint:disable:align
export function getDogsTableData(
  page: number,
  pageSize: number,
  sorted: any[],
  filtered: any[]
): Promise<BaseServerResponse<DogModel[]>> {
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

  return (
    endpoint
      .post('/GetDogsForTable', JSON.stringify(postObject), {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      // .then(({ data }) => (data as DogModel));
      .then(({ data, headers }) => {
        const total: number = headers['x-total-count'];
        const rtn: BaseServerResponse<DogModel[]> = {
          data: data as DogModel[],
          count: total,
        };
        return rtn;
      })
      // .then(response => handleRetrievedDataFn(response))
      .catch(response => {
        return { data: [], count: 0 };
      })
  );
}
