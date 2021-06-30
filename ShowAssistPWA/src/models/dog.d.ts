import GenderTypes from './gender';

export interface BasicDogLookupInfo {
  id: number;
  dogName: string;
  bullyId: number;
  abkcNo: string;
  ownerId: number;
  breed: string;
  birthdate: Date;
  gender: GenderTypes;
}
