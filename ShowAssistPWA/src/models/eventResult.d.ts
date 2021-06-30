interface EventResult {
  id: number;
  showId: number;
  breed: string;
  style: string;
  class: string;
  winning_ABKC: string;
  points: number;
  champWin: boolean;
  champPoints: number;
  noComp: boolean;
  comments?: string;
  createDate: Date;
  modifyDate: Date;
  modifiedBy: string;
  dogId: number;
  armbandNumber: number;
  classTemplateId: number;
  styleId: number;
}
