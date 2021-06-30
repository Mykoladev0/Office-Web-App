import {
  query as queryUsers,
  queryCurrent,
  queryCurrentUser
} from '@/services/userApi';

export default {
  namespace: 'user',

  state: {
    list: [],
    currentUser: {},
    currentUsers: {}
  },

  effects: {
    * fetch(_, {
      call,
      put
    }) {
      const response = yield call(queryUsers);
      yield put({
        type: 'save',
        payload: response,
      });
    },
    * fetchCurrent(_, {
      call,
      put
    }) {
      const response = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: response,
      });
    },
    * fetchCurrentUser(_, {
      call,
      put
    }) {
      const response = yield call(queryCurrentUser);
      yield put({
        type: 'saveCurrentUserData',
        payload: response,
      });
    },
  },

  reducers: {
    save(state, action) {
      return {
        ...state,
        list: action.payload,
      };
    },
    saveCurrentUser(state, action) {
      return {
        ...state,
        currentUser: action.payload || {},
      };
    },
    saveCurrentUserData(state, action) {
      return {
        ...state,
        currentUsers: action.payload || {},
      };
    },
    changeNotifyCount(state, action) {
      return {
        ...state,
        currentUser: {
          ...state.currentUser,
          notifyCount: action.payload.totalCount,
          unreadCount: action.payload.unreadCount,
        },
      };
    },
  },
};
