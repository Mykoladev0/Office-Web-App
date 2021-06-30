import fetch from 'dva/fetch';
import {
  notification
} from 'antd';
import router from 'umi/router';
import hash from 'hash.js';
import {
  isAntdPro
} from './utils';
import {
  setAuthority
} from '@/utils/authority';


const codeMessage = {
  200: 'The server successfully returned the requested data.',
  201: 'New or modified data is successful.',
  202: 'A request has entered the background queue(asynchronous task).',
  204: 'The data was deleted successfully.',
  400: 'The request was made with an error and the server did not perform any operations to create or modify data.',
  401: 'User does not have permission (token, username, password is incorrect).',
  403: 'The user is authorized, but access is forbidden.',
  404: 'The request is made for a record that does not exist and the server does not operate.',
  406: 'The format of the request is not available.',
  410: 'The requested resource is permanently deleted and will not be retrieved.',
  422: 'A validation error occurred when creating an object.',
  500: 'An error occurred on the server. Please check the server.',
  502: 'Gateway error.',
  503: 'The service is unavailable and the server is temporarily overloaded or maintained.',
  504: 'The gateway timed out.',
};

const apiHost = 'https://api.abkconline.com';
// const apiHost = 'http://localhost:12758';


const checkStatus = response => {
  if (response.status >= 200 && response.status < 300) {
    return response;
  }
  if (response.status === 401) {
    // @HACK
    window.g_app._store.dispatch({
      type: 'login/logout',
    });
    return;
  }

  const errortext = codeMessage[response.status] || response.statusText;
  notification.error({
    message: `Request error ${response.status}: ${response.fullUrl}`,
    description: errortext,
  });

  return response;
  // const error = new Error(errortext);
  // error.name = response.status;
  // error.response = response;
  // throw error;
};

const cachedSave = (response, hashcode) => {
  /**
   * Clone a response data and store it in sessionStorage
   * Does not support data other than json, Cache only json
   */
  const contentType = response.headers.get('Content-Type');
  if (contentType && contentType.match(/application\/json/i)) {
    // All data is saved as text
    response
      .clone()
      .text()
      .then(content => {
        // sessionStorage.setItem(hashcode, content);
        // sessionStorage.setItem(`${hashcode}:timestamp`, Date.now());
      });
  }
  return response;
};

/**
 * Requests a URL, returning a promise.
 *
 * @param  {string} url       The URL we want to request
 * @param  {object} [option] The options we want to pass to "fetch"
 * @return {object}           An object containing either "data" or "err"
 */
export default async function request(url, option) {
  const options = {
    expirys: isAntdPro(),
    ...option,
  };

  // modify url to use api host
  const fullUrl = apiHost + url;
  /**
   * Produce fingerprints based on url and parameters
   * Maybe url has the same parameters
   */
  const fingerprint = fullUrl + (options.body ? JSON.stringify(options.body) : '');
  const hashcode = hash
    .sha256()
    .update(fingerprint)
    .digest('hex');

  const defaultOptions = {
    // credentials: 'include', // removing because of CORS with *.  Should put back in when locking down CORs sites TB:4.5.19
  };
  const newOptions = {
    // headers: {
    //   'Access-Control-Allow-Origin': '*',
    // },
    ...defaultOptions,
    ...options,

  };
  if (
    newOptions.method === 'POST' ||
    newOptions.method === 'PUT' ||
    newOptions.method === 'DELETE'
  ) {
    if (!(newOptions.body instanceof FormData)) {
      newOptions.headers = {
        Accept: 'application/json',
        'Content-Type': 'application/json; charset=utf-8',
        ...newOptions.headers,
      };
      newOptions.body = JSON.stringify(newOptions.body);
    } else {
      // newOptions.body is FormData
      newOptions.headers = {
        Accept: 'application/json',
        ...newOptions.headers,
      };
    }
  }

  const expirys = options.expirys && 60;
  // options.expirys !== false, return the cache,
  if (options.expirys !== false) {
    const cached = sessionStorage.getItem(hashcode);
    const whenCached = sessionStorage.getItem(`${hashcode}:timestamp`);
    if (cached !== null && whenCached !== null) {
      const age = (Date.now() - whenCached) / 1000;
      if (age < expirys) {
        const response = new Response(new Blob([cached]));
        return response.json();
      }
      sessionStorage.removeItem(hashcode);
      sessionStorage.removeItem(`${hashcode}:timestamp`);
    }
  }


  const data = await fetch(fullUrl, newOptions);
  const status = await checkStatus(data);
  const response = await cachedSave(status, hashcode)
  // DELETE and 204 do not return data by default
  // using .json will report an error.
  if (newOptions.method === 'DELETE' || response.status === 204) {
    return response.text();
  }
  if (newOptions.responseType === 'blob') {
    const responseData = {
      fileName: '',
      fileUrl: '',
      fileType: '',
    };
    response.headers.forEach((val, key) => {
      // if (val.includes("filename=")) {
      if (key.toLowerCase() === 'content-disposition') {
        const sections = val.split(';');
        responseData.fileName = sections[1].split('=').pop();
        responseData.fileName = responseData.fileName.split('"').join('');
      }
      if (key.toLowerCase() === 'content-type') {
        responseData.fileType = val;
      }
    });
    const {
      body
    } = response;
    const responseObj = await new Response(body)
    const blobData = await responseObj.blob();
    const fileUrl = await URL.createObjectURL(blobData)
    responseData.fileUrl = fileUrl;
    return responseData
  }
  return response.json();
}
