import axios from 'axios';
import { BreedModel } from '../Models';

const endpoint = axios.create({
  // baseURL: (<any>window).coreApp.breedsApi.baseUrl
  baseURL: '/api/breeds',
});

export function getBreeds(): Promise<BreedModel[]> {
  return endpoint.get(`/GetBreeds`).then(({ data }) => data as BreedModel[]);
}
