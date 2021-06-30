export interface Show {
  showId: number;
  showName: string;
  showDate: string | null;
  address: string;
  city: string;
  state: string;
  zip: string;
  international: boolean | null;
  insurancePolicy: string;
  insuranceExpires: string | null;
  entriesAllowed: number | null;
  clubId: number | null;
  firstShow: boolean | null;
  appRecvd: boolean | null;
  dateApproved: string | null;
  feePaid: number | null;
  abkchosted: boolean | null;
  judge1: string;
  judge2: string;
  ringSteward: string;
  paperwork1: string;
  paperwork2: string;
  abkcrep: string;
  authLetterRecvd: boolean | null;
  dateLetterRecvd: string | null;
  breedsShown: string;
  stylesShown: string;
  classesClosed: boolean | null;
  createDate: string | null;
  modifyDate: string | null;
  modifiedBy: string;
  comments: string;
  judgeId: number | null;
  finalizedDate: string | null;
  type:number;
}