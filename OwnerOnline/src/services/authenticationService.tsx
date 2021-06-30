import * as requestService from "./requestService";
import * as notificationService from "./notificationService";

export const login = (username: string, password: string) => {
  return new Promise((resolve, reject) => {
    localStorage.setItem("user", username);
    requestService
      .post("Account/GetToken", {
        userName: username,
        password: password
      })
      .then((res: any) => {
        console.log(res);
        notificationService.showNotification(
          "Successful Login",
          "You are now logged in"
        );
        localStorage.setItem("token", res.data);
        resolve(res);
      })
      .catch(err => {
        notificationService.showNotification(
          "Login failed",
          "User name or password is invalid"
        );
        console.log(err);
        reject(err);
      });
  });
};

export const signUp = (firstname: string, lastname: string, email: string) => {
  return new Promise((resolve, reject) => {
    requestService
      .post("Account/Register", {
        emailAddress: email,
        roleRequested: 1,
        firstName: firstname,
        lastName: lastname
      })
      .then(res => {
        notificationService.showNotification(
          "Registration Successful",
          "Your account has been created"
        );
        console.log(res);
        resolve(res);
      })
      .catch(err => {
        notificationService.showNotification(
          "Registration Failed",
          "Your account could not be created"
        );
        console.log(err);
        reject(err);
      });
  });
};

export const setToken = (token:string) => {
    localStorage.setItem("token", token);
  };

export const userAuthenticated = () => {
  if (localStorage.getItem("user")) {
    return true;
  }
  return false;
};
