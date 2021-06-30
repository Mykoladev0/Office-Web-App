import { showsgData } from "../models/static";
import { Show } from "../models/showModel";

export const getShows: Show[] | any = () => {
  return new Promise((resolve, reject) => {
    // TODO : need to bind with API
    resolve(showsgData);

    //in case of error
    reject(false);
  });
};

export const searchShows: Show[] | any = (type: number, param: string) => {
  return new Promise((resolve, reject) => {
    // TODO : need to bind with API
    if (param !== "") {
      resolve(
        showsgData.filter(
          show =>
            show.showName.toLowerCase().indexOf(param.toLowerCase()) > -1 &&
            show.type === type
        )
      );
    } else {
      resolve(showsgData.filter(show => show.type === type));
    }
    //in case of error
    reject(false);
  });
};

export const getShowById: Show | any = (id: number) => {
  return new Promise((resolve, reject) => {
    // TODO : need to bind with API
    if (id) {
      resolve(
        showsgData.find(
          show =>
            show.showId === Number(id)
        )
      );
    }
    //in case of error
    reject(false);
  });
};
