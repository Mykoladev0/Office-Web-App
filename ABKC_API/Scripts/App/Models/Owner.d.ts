export interface OwnerLookupInfo {
  // id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  ownerId: number;
}

interface OwnerModel {
  // id: number;
  ownerId: number;
  lastName: string;
  middleInitial: string;
  firstName: string;
  fullName: string;
  address1: string;
  address2: string;
  address3: string;
  city: string;
  state: string;
  zip: string;
  country: string;
  international: boolean;
  email: string;
  phone: string;
  cell: string;
  dualSignature: boolean;
  comments: string;
  createDate: string;
  modifyDate: string;
  modifiedBy: string;
  webPassword?: any;
}
