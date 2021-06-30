import axios from "axios";
import { baseUrl } from "../envirnment";

const config = {
  headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
};

export const get = (url: string) => {
  return new Promise((resolve, reject) => {
    axios
      .get(baseUrl + url, config)
      .then((res) => {
        resolve(res);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const post = (url: string, obj: any = {}) => {
  return new Promise((resolve, reject) => {
    axios
      .post(baseUrl + url, obj, config)
      .then((res) => {
        resolve(res);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const put = (url: string, obj: any = {}) => {
  return new Promise((resolve, reject) => {
    axios
      .put(baseUrl + url, obj, config)
      .then((res) => {
        resolve(res);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const deleteMethod = (url: string) => {
  return new Promise((resolve, reject) => {
    axios
      .delete(baseUrl + url, config)
      .then((res) => {
        resolve(res);
      })
      .catch((error) => {
        reject(error);
      });
  });
};
