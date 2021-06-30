import { routerRedux } from 'dva/router';
import { accountLogin, getFakeCaptcha, queryCurrentUser } from '@/services/api';
import { setAuthority } from '@/utils/authority';
import { getPageQuery } from '@/utils/utils';
import { reloadAuthorized } from '@/utils/Authorized';

// function sleep(sec) {
//   return new Promise(resolve => setTimeout(resolve, sec * 1000));
// }

export default {
  namespace: 'login',

  state: {
    status: undefined,
  },

  effects: {
    *login({ payload }, { call, put }) {
      const response = yield call(accountLogin, payload);
      yield put({
        type: 'changeLoginStatus',
        payload: response,
      });
      // Login successfully
      if (response) {
        localStorage.setItem('user-token', response);
        reloadAuthorized();
        const urlParams = new URL(window.location.href);
        const params = getPageQuery();
        let { redirect } = params;
        if (redirect) {
          const redirectUrlParams = new URL(redirect);
          if (redirectUrlParams.origin === urlParams.origin) {
            redirect = redirect.substr(urlParams.origin.length);
            if (redirect.match(/^\/.*#/)) {
              redirect = redirect.substr(redirect.indexOf('#') + 1);
            }
          } else {
            window.location.href = redirect;
            return;
          }
        }

        /** */
        const currentUser = yield call(queryCurrentUser);
        localStorage.setItem('user-role', currentUser.abkcRolesUserBelongsTo[1]);
        yield put({
          type: 'saveCurrentUserData',
          payload: currentUser,
        });
        yield put({
          type: 'changeLoginStatus',
          payload: currentUser,
        });
        /** */
        if (currentUser && currentUser.id && localStorage.getItem('user-role') !== ['guest']) {
          setAuthority(currentUser.abkcRolesUserBelongsTo[1].toString());
          reloadAuthorized();
          // yield sleep(5);
          yield put(routerRedux.replace(redirect || '/'));
        }
      }
    },

    *getCaptcha({ payload }, { call }) {
      yield call(getFakeCaptcha, payload);
    },

    *logout(_, { put }) {
      yield put({
        type: 'changeLoginStatus',
        payload: {
          status: false,
          currentAuthority: 'guest',
        },
      });
      reloadAuthorized();
      yield put(
        routerRedux.push({
          pathname: '/user/login',
          // search: stringify({
          //   // redirect: window.location.href,
          // }),
        })
      );
    },
  },

  reducers: {
    changeLoginStatus(state, { payload }) {
      if (payload.currentAuthority === "guest") {
        localStorage.removeItem('user-token')
        localStorage.removeItem('user-role')
        setAuthority(payload.currentAuthority);
      }
      // else if (payload) {
      //   setAuthority("user");
      // }
      // const user = payload;
      // if (user && user.id) {
      //   setAuthority(user.abkcRolesUserBelongsTo[1].toString());
      // }
      return {
        ...state,
        status: payload.status,
        type: payload.type,
      };
    },
    saveCurrentUserData(state, action) {
      // console.log('action.payload -->', );
      return {
        ...state,
        currentUsers: action.payload || {},
      };
    },
  },
};