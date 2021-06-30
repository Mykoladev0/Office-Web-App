import * as requestService from "./requestService";
import { dogData } from "../models/static";
import { Dog } from "../models/dogModel";
import * as notificationService from "./notificationService";

export const getDogs: Dog[] | any = () => {
  return new Promise((resolve, reject) => {
    // TODO : need to bind with API
    resolve(dogData);

    //in case of error
    reject(false);
  });
};

export const searchDogs: Dog[] | any  = (param: string) => {
  return new Promise((resolve, reject) => {
    // TODO : need to bind with API
    if (param !== "") {
      const filteredArray = dogData.filter(
        dog => dog.DogName.toLowerCase().indexOf(param.toLowerCase()) > -1
      );
      resolve(filteredArray);
    } else {
      resolve(dogData);
    }
    //in case of error
    reject(false);
  });
};

export const registerDog: Dog | any  = (dog: Dog) => {
  return new Promise((resolve, reject) => {
    requestService
      .post("CreateDogAsync", {
        ...dog,
        "modifiedBy": "User 1"
      })
      .then(res => {
        notificationService.showNotification(
          "Successful Registration",
          "Your dog has been registered"
        );
        console.log(res);
        resolve(res);
      })
      .catch(err => {
        notificationService.showNotification(
          "Registration Failed",
          "Your dog was not registered"
        );
        console.log(err);
        reject(err);
      });
  });
};