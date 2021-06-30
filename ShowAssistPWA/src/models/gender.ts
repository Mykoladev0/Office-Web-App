enum GenderTypes {
  Male = 'Male',
  Female = 'Female',
  NA = 'N/A',
}
const GenderList = Object.keys(GenderTypes).map(k => GenderTypes[k as any]);
const GenderFromString = (gender: string) => {
  const rtn: GenderTypes = GenderTypes[gender];
  return rtn || GenderTypes.NA;
};
export default GenderTypes;
export { GenderList, GenderFromString };
