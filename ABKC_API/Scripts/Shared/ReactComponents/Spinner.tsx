import React, { Component } from 'react';
import { RingLoader, BeatLoader } from 'react-spinners';

// tslint:disable:max-classes-per-file
export class Spinner extends Component<any, any> {
  public render() {
    return (
      <div className="p-5">
        <div className="row justify-content-center align-items-center">
          <div className="col">
            <RingLoader />
          </div>
        </div>
      </div>
    );
  }
}

export class BeatSpinner extends Component<any, any> {
  public render() {
    return (
      <div className="p-5">
        <div className="row justify-content-center align-items-center">
          <div className="col">
            <BeatLoader />
          </div>
        </div>
      </div>
    );
  }
}
