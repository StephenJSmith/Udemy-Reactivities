import axios, { AxiosResponse } from 'axios';
import { IActivity } from '../models/activity';
import { history } from '../..';
import { toast } from 'react-toastify';

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(undefined, error => {
  if (error.message === 'Network Error' && !error.response) {
    toast.error('Newtork error - ensure API is running!');
  }

  const {status, data, config} = error.response;
  if (status === 404) {
    history.push('/notfound');
  }
  
  if (status === 400 && config.method === 'get' && data.errors.hasOwnProperty('id')) {
    history.push('/notfound');
  }

  if (status === 500) {
    toast.error('Server error - check the terminal for more info!');
  }

  throw error;
})

const activitiesUrl = '/activities';

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) => 
  new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms));

const requests = {
  get: (url: string) => axios.get(url).then(sleep(1000)).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(sleep(1000)).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(sleep(1000)).then(responseBody),
  del: (url: string) => axios.delete(url).then(sleep(1000)).then(responseBody)
}

const Activities = {
  list: (): Promise<IActivity[]> => requests.get(activitiesUrl),
  details: (id: string) => requests.get(`${activitiesUrl}/${id}`),
  create: (activity: IActivity) => requests.post(`${activitiesUrl}`, activity),
  update: (activity: IActivity) => requests.put(`${activitiesUrl}/${activity.id}`, activity),
  delete: (id: string) => requests.del(`${activitiesUrl}/${id}`)
}

export default {
  Activities
}


