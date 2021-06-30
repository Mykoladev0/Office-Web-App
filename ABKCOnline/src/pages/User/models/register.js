import { acountRegister } from '@/services/api';
import { setAuthority } from '@/utils/authority';
import { reloadAuthorized } from '@/utils/Authorized';

export default {
  namespace: 'register',

  state: {
    status: undefined,
    message: ''
  },

  effects: {
    *submitSignUp({ payload }, { call, put }) {
      const response = yield call(acountRegister, payload);
      console.log('response', response)
      yield put({
        type: 'registerHandle',
        payload: response,
      });
    },
  },

  reducers: {
    registerHandle(state, { payload }) {
      setAuthority('user');
      reloadAuthorized();
      return {
        ...state,
        message: payload === true ? 'Once the account is approved by the ABKC Office, an email will be sent to the registered email to set up your password' : payload,
        status: payload === true ? true : false,
      };
    },
  },
};
