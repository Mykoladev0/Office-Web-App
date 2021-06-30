import BasicDogLookupInfo from './dog';

export interface ShowParticipantInfo {
  id?: number;
  armbandNumber?: number;
  showId: number;
  dateRegistered?: Date;
  dog: BasicDogLookupInfo;
}
