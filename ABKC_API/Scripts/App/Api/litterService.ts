import axios from 'axios';
import { getAxiosHeader } from './baseAPI';

const endpoint = axios.create({
  // baseURL: (<any>window).coreApp.littersApi.baseUrl,
  baseURL: '/api/litters',
});

export async function getLittersCount(): Promise<number> {
  const authHeader = await getAxiosHeader();
  return endpoint.get('/GetLittersCount', authHeader).then(({ data }) => data as number);
}
