import {
  getPedigreeRegistrations, requestDeletePedigree, sidePanelListPedigree, getRegInfoCall,
} from '@/services/pedigreeApi';
import {
  queryLitterById, queryDeleteReg, sidePanelListLitter,
} from '@/services/litterApi';
import {
  getJuniorHandlerById, deleteJuniorHandlersById, sidePanelListJunior,
} from '@/services/juniorHandlersApi';
import {
  queryDeletePuppyReg, queryPuppyById, sidePanelListPuppy,
} from '@/services/puppyApi';
import {
  queryDeleteTransferReg, queryTransferById, sidePanelListTransfer,
} from '@/services/dogTransferApi';
import {
  queryDeleteBullyRequest, sidePanelListBully,
} from '@/services/bullyApi';

import {
  denyPendingUser, resetPassword
} from '@/services/userApi';
import {
  queryFakeList, filterRepData, approveAllRegData, getRepresentatives, getSupportingDocs, searchOwnerList, rejectRegistrations, approveRegistrations, requestRegistrationsInfo, filterData, searchDogList, getRegOwners, removeFakeList, addFakeList, updateFakeList, getOwners, queryCurrentUser, getPendingUsers, activatePendingUser,
  filterPendingId, getRegistrations, updateRegistrationFeeCall, suspendUserAccount, unSuspendUserAccount, issueRefundApi
} from '@/services/api';
import moment from 'moment';
import {
  message
} from 'antd';

export default {
  namespace: 'list',

  state: {
    list: [],
    listData: [],
    sidePanelDetails: [],
    filters: [],
    representatives: [],
    docsData: '',
    owners: [],
    ownerListData: [],
    editList: {},
    docsStatus: false,
    spinner: false,
    showStatus: false,
  },
  // prettier-ignore
  effects: {
    * fetch({ payload }, { call, put }) {
      const response = yield call(queryFakeList, payload);
      yield put({
        type: 'queryList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * fetchData({ payload }, { call, put }) {
      const response = yield call(getPedigreeRegistrations, payload);
      yield put({
        type: 'initialRegList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * fetchRepData({ payload }, { call, put }) {
      const response = yield call(getRepresentatives, payload);
      yield put({
        type: 'repList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * filterPending({ payload }, { call, put }) {
      const response = yield call(filterData, payload);
      yield put({
        type: 'initialRegList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * filterData({ payload }, { call, put }) {
      if (payload === "Pending") {
        const response = yield call(filterData, payload);
        yield put({
          type: 'queryList3',
          payloadData: payload,
          payload: Array.isArray(response) ? response : [],
        });
      } else if (payload === "All" || payload === "night") {
        yield put({
          type: 'queryList3',
          payloadData: payload,
        });
      }
    },
    * filterByRep({ payload }, { call, put }) {
      const response = yield call(filterRepData, payload);
      yield put({
        type: 'queryListRep',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * filterByDates({ payload }, { put }) {
      yield put({
        type: 'queryList4',
        payload
      });
    },
    * approveAllByRep({ payload }, { call, put }) {
      const response = yield call(approveAllRegData, payload);
      yield put({
        type: 'approveAll',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * fetchSingleData({ payload, regType }, { call, put }) {
      let response;
      if (regType === 'Pedigree') {
        response = yield call(sidePanelListPedigree, payload);
      }
      else if (regType === 'Transfer') {
        response = yield call(sidePanelListTransfer, payload);
      }
      else if (regType === 'JuniorHandler') {
        response = yield call(sidePanelListJunior, payload);
      }
      else if (regType === 'Litter') {
        response = yield call(sidePanelListLitter, payload);
      }
      else if (regType === 'Bully') {
        response = yield call(sidePanelListBully, payload);
      }
      else if (regType === 'Puppy') {
        response = yield call(sidePanelListPuppy, payload);
      }

      yield put({
        type: 'sideData',
        payloadDatas: Array.isArray(response) ? response : [],
        payload
      });
    },
    * searchByOwner({ payload }, { call, put }) {
      const response = yield call(searchOwnerList, payload);
      yield put({
        type: 'searchData',
        payloadDatas: Array.isArray(response) ? response : [],
        payload: payload.val
      });
    },

    * searchByDog({ payload }, { call, put }) {
      const response = yield call(searchDogList, payload);
      yield put({
        type: 'searchDogData',
        payloadDatas: Array.isArray(response) ? response : [],
        payload: payload.val
      });
    },

    * supportingDocs({ payload }, { call, put }) {
      const response = yield call(getSupportingDocs, payload);
      yield put({
        type: 'docsList',
        payload: response,
      });
    },

    * rejectRegistration({ payload }, { call, put }) {
      const response = yield call(rejectRegistrations, payload);
      if (response) {
        const resp = yield call(getPedigreeRegistrations, payload)
        yield put({
          type: 'initialRegList',
          payload: Array.isArray(resp) ? resp : [],
        });
      }
      yield put({
        type: 'approveAll',
        payload: Array.isArray(response) ? response : [],
      });
    },

    * approveRegistration({ payload }, { call, put }) {
      const response = yield call(approveRegistrations, payload);
      if (response) {
        const resp = yield call(getPedigreeRegistrations, payload)
        yield put({
          type: 'initialRegList',
          payload: Array.isArray(resp) ? resp : [],
        });
      }
      yield put({
        type: 'approveAll',
        payload: Array.isArray(response) ? response : [],
      });
    },

    * requestRegistrationInfo({ payload }, { call, put }) {
      const response = yield call(requestRegistrationsInfo, payload);
      if (response) {
        const resp = yield call(getPedigreeRegistrations, payload)
        yield put({
          type: 'initialRegList',
          payload: Array.isArray(resp) ? resp : [],
        });
      }
      yield put({
        type: 'approveAll',
        payload: Array.isArray(response) ? response : [],
      });
    },

    * deletePedigree({ payload }, { call, put }) {
      const response = yield call(requestDeletePedigree, payload);
      yield put({
        type: 'deleteReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Pedigree deleted successfully.')
      } else {
        message.error('Pedegree deletion failed.')
      }
    },

    * deleteLitter({ payload }, { call, put }) {
      const response = yield call(queryDeleteReg, payload);
      yield put({
        type: 'deleteLitterReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Litter deleted successfully.')
      } else {
        message.error('Litter deletion failed.')
      }
    },
    // Junior handler
    * deleteJuniorHandler({ payload }, { call, put }) {
      const response = yield call(deleteJuniorHandlersById, payload);
      yield put({
        type: 'deleteJuniorHandlerReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Jr.handler deleted successfully.')
      } else {
        message.error('Jr.handler deletion failed.')
      }
    },
    * getJuniorHandlersById({ payload }, { call, put }) {
      const response = yield call(getJuniorHandlerById, payload);
      yield put({
        type: 'getJuniorHandlersData',
        payload: response,
      });
    },

    // puppy
    * deletePuppy({ payload }, { call, put }) {
      const response = yield call(queryDeletePuppyReg, payload);
      yield put({
        type: 'deletePuppyReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Puppy deleted successfully.')
      } else {
        message.error('Puppy deletion failed.')
      }
    },
    // Puppy manage
    * getPuppyById({ payload }, { call, put }) {
      const response = yield call(queryPuppyById, payload);
      yield put({
        type: 'getPuppyByIdData',
        payload: response,
      });
    },

    // owners manage
    * fetchOwnerData({ payload }, { call, put }) {
      const response = yield call(getOwners, payload);
      yield put({
        type: 'ownerList',
        payload: Array.isArray(response) ? response : [],
      });
    },

    * fetchOwnerRegData({ payload }, { call, put }) {
      const response = yield call(getRegOwners, payload);
      yield put({
        type: 'ownerRegList',
        payload: Array.isArray(response) ? response : [],
      });
    },

    * appendFetch({ payload }, { call, put }) {
      const response = yield call(queryFakeList, payload);
      yield put({
        type: 'appendList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * submit({ payload }, { call, put }) {
      let callback;
      if (payload.id) {
        callback = Object.keys(payload).length === 1 ? removeFakeList : updateFakeList;
      } else {
        callback = addFakeList;
      }
      const response = yield call(callback, payload); // post
      yield put({
        type: 'queryList',
        payload: response,
      });
    },
    * getRegInfo({ payload }, { call, put }) {
      const response = yield call(getRegInfoCall, payload);
      yield put({
        type: 'getRegInfoData',
        payload: response
      });
    },
    * getCurrentUser(_, { call, put }) {
      const response = yield call(queryCurrentUser);
      if (response && response.id) {
        const roleName = response.abkcRolesUserBelongsTo[1]
        localStorage.setItem('user-role', roleName)
      }
      yield put({
        type: 'saveCurrentUserData',
        payload: response,
      });
    },

    // Litter manage
    * getLitterById({ payload }, { call, put }) {
      const response = yield call(queryLitterById, payload);
      yield put({
        type: 'getLitterByIdData',
        payload: response,
      });
    },

    // transfer
    * deleteTransferRegistration({ payload }, { call, put }) {
      const response = yield call(queryDeleteTransferReg, payload);
      yield put({
        type: 'deleteTransferReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Transfer registration deleted successfully.')
      } else {
        message.error('Transfer registration deletion failed.')
      }
    },
    * getTransferRegById({ payload }, { call, put }) {
      const response = yield call(queryTransferById, payload);
      yield put({
        type: 'getTransferRegByIdData',
        payload: response,
      });
    },
    // bully request
    * deleteBullyRequest({ payload }, { call, put }) {
      const response = yield call(queryDeleteBullyRequest, payload);
      yield put({
        type: 'deleteBullyReg',
        id: payload,
        payload: Array.isArray(response) ? response : [],
      });
      if (response) {
        message.success('Bully deleted successfully.')
      } else {
        message.error('Bully deletion failed.')
      }
    },
    // pending users
    * fetchPendingUsers({ payload }, { call, put }) {
      const response = yield call(getPendingUsers, payload);
      yield put({
        type: 'pendingUsers',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * activatePendingUser({ payload }, { call, put }) {
      const response = yield call(activatePendingUser, payload);
      if (response === true) {
        message.success('Activate User Successfully');
        yield put({
          type: 'removeUsers',
          id: payload,
          payload: Array.isArray(response) ? response : [],
        });
      } else {
        message.error(response);
      }
    },
    * denyPendingUserRequest({ payload }, { call, put }) {
      const response = yield call(denyPendingUser, payload);
      if (response === true) {
        message.success('User Request Successfully Denied')
        yield put({
          type: 'removeUsers',
          id: payload,
          payload: Array.isArray(response) ? response : [],
        });
      } else {
        message.error(response);
      }
    },
    * resetPasswordRequest({ payload }, { call,
      // put
    }) {
      const response = yield call(resetPassword, payload);
      if (response === true) {
        message.success('User Password Reset Sent Successfully');
        // yield put({
        //   type: 'removeUsers',
        //   id: payload,
        //   payload: Array.isArray(response) ? response : [],
        // });
      } else {
        message.error(response);
      }
    },
    * searchBySubmitted({ payload }, { put }) {
      yield put({
        type: 'submittedByData',
        payload: payload.val
      });
    },
    // owners/rep
    * filterRegById({ payload }, { call, put }) {
      const response = yield call(getRegistrations, payload);
      yield put({
        type: 'initialRegList',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * filterPendingById({ payload }, { call, put }) {
      const response = yield call(filterPendingId, payload);
      yield put({
        type: 'pendingById',
        payload: Array.isArray(response) ? response : [],
      });
    },
    * updateRegistrationFee({ payload }, { call, put }) {
      const response = yield call(updateRegistrationFeeCall, payload);
      if (response === true) {
        message.success('Registrtaion Fee Updated Successfully')
        const response2 = yield call(getRepresentatives, payload);
        yield put({
          type: 'repList',
          payload: Array.isArray(response2) ? response2 : [],
        });
      } else {
        message.error(response)
      }
    },
    * suspendAccountOwner({ payload }, { call, put }) {
      const response = yield call(suspendUserAccount, payload);
      if (response === true) {
        message.success('User Suspended Succuessfully')
        const response2 = yield call(getOwners, payload);
        yield put({
          type: 'ownerList',
          payload: Array.isArray(response2) ? response2 : [],
        });
      } else {
        message.error(response)
      }
    },
    * unSuspendAccountOwner({ payload }, { call, put }) {
      const response = yield call(unSuspendUserAccount, payload);
      if (response === true) {
        message.success('User Unsuspended Succuessfully')
        const response2 = yield call(getOwners, payload);
        yield put({
          type: 'ownerList',
          payload: Array.isArray(response2) ? response2 : [],
        });
      } else {
        message.error(response)
      }
    },
    * suspendAccountRep({ payload }, { call, put }) {
      const response = yield call(suspendUserAccount, payload);
      if (response === true) {
        message.success('User Suspended Succuessfully')
        const response2 = yield call(getRepresentatives, payload);
        yield put({
          type: 'repList',
          payload: Array.isArray(response2) ? response2 : [],
        });
      } else {
        message.error(response)
      }
    },
    * unSuspendAccountRep({ payload }, { call, put }) {
      const response = yield call(unSuspendUserAccount, payload);
      if (response === true) {
        message.success('User Unsuspended Succuessfully')
        const response2 = yield call(getRepresentatives, payload);
        yield put({
          type: 'repList',
          payload: Array.isArray(response2) ? response2 : [],
        });
      } else {
        message.error(response)
      }
    },
    // Issue Refund
    * issueRefund({ payload }, { call }) {
      const response = yield call(issueRefundApi, payload);
      if (response === true) {
        message.success('Issue Refund Succuessfully')
      } else {
        message.error(response)
      }
    },
  },

  reducers: {
    saveCurrentUserData(state, action) {
      return {
        ...state,
        currentUsers: action.payload || {},
      };
    },
    queryList(state, action) {
      return {
        ...state,
        list: action.payload,
        filters: action.payload,
      };
    },
    initialRegList(state, action) {
      return {
        ...state,
        listData: action.payload,
        filters: action.payload,
        sidePanelDetails: action.payload,
        showStatus: false,
      };
    },
    queryListRep(state, action) {
      return {
        ...state,
        listData: action.payload,
        sidePanelDetails: action.payload,
      };
    },
    repList(state, action) {
      return {
        ...state,
        representatives: action.payload,
      };
    },
    approveAll(state) {
      return {
        ...state,
      };
    },
    docsList(state, action) {
      return {
        ...state,
        docsData: action.payload,
        docsStatus: true,
        spinner: false,
      };
    },
    queryList3(state, action) {
      const dataVal = action.payloadData.trim();
      let searchRes = state.filters;
      if (dataVal === 'Pending') {
        // searchRes = state.filters.filter(scenario =>
        //   scenario.registrationStatus.match(dataVal)
        // )
        searchRes = action.payload;
      } else if (dataVal === 'night') {
        searchRes = state.filters.filter(scenario => scenario.overnightRequested === true);
      } else {
        searchRes = state.filters;
      }
      return {
        ...state,
        filters: state.filters,
        listData: searchRes,
        sidePanelDetails: searchRes,
      };
    },
    deleteReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.filters.filter(data =>
          data.registrationType === 'Pedigree' ? data.id !== action.id : data
        ),
      };
    },
    deleteLitterReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.filters.filter(data =>
          data.registrationType === 'Litter' ? data.id !== action.id : data
        ),
      };
    },
    queryList4(state, action) {
      const dataVal = action.payload.dateString;
      const startDate = dataVal[0];
      const endDate = new Date(dataVal[1]);
      const nextDay = new Date();
      nextDay.setDate(endDate.getDate() + 1);
      const toDate = moment(nextDay).format();
      let searchRes = state.filters;
      searchRes = state.filters.filter(
        scenario => scenario.dateSubmitted > startDate && scenario.dateSubmitted < toDate
      );
      return {
        ...state,
        filters: state.filters,
        listData: searchRes,
        sidePanelDetails: searchRes,
      };
    },
    sideData(state, action) {
      return {
        ...state,
        sidePanelDetails: state.listData.filter(data => data.id === action.payload),
      };
    },
    searchData(state, action) {
      const dataVal = action.payload.trim();
      let searchRes = state.filters;
      const title = new RegExp(dataVal, 'gi');
      if (dataVal.length) {
        searchRes = state.filters.filter(scenario =>
          scenario.dogInfo.owner ?
            scenario.dogInfo.owner.firstName.match(title) ||
            scenario.dogInfo.owner.lastName.match(title) :
            ''
        );
      } else {
        searchRes = state.filters;
      }
      return {
        ...state,
        sidePanelDetails: searchRes,
        listData: searchRes,
        filters: state.filters,
        list: searchRes,
      };
    },
    searchDogData(state, action) {
      const dataVal = action.payload.trim();
      let searchRes = state.filters;
      const title = new RegExp(dataVal, 'gi');
      if (dataVal.length) {
        searchRes = state.filters.filter(scenario => scenario.dogInfo.dogName.match(title));
      } else {
        searchRes = state.filters;
      }
      return {
        ...state,
        sidePanelDetails: searchRes,
        listData: searchRes,
        filters: state.filters,
        list: searchRes,
      };
    },
    ownerList(state, action) {
      return {
        ...state,
        owners: action.payload,
      };
    },
    ownerRegList(state, action) {
      return {
        ...state,
        ownerListData: action.payload,
      };
    },
    appendList(state, action) {
      return {
        ...state,
        list: state.list.concat(action.payload),
      };
    },
    getRegInfoData(state, action) {
      return {
        ...state,
        editList: action.payload,
        editPedigreeInfo: action.payload,
        editPedigreeStart: action.payload ? true : false,
      };
    },
    getLitterByIdData(state, action) {
      return {
        ...state,
        editLitterList: action.payload,
      };
    },
    getJuniorHandlersData(state, action) {
      return {
        ...state,
        editJuniorHandlersList: action.payload,
        showStatus: true,
      };
    },
    deleteJuniorHandlerReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.filters.filter(data =>
          data.registrationType === 'JuniorHandler' ? data.id !== action.id : data
        ),
      };
    },
    getPuppyByIdData(state, action) {
      return {
        ...state,
        editPuppyList: action.payload,
      };
    },
    deletePuppyReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.filters.filter(data =>
          data.registrationType === 'Puppy' ? data.id !== action.id : data
        ),
      };
    },
    // transfer
    deleteTransferReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.listData.filter(data => data.id !== action.id),
      };
    },
    getTransferRegByIdData(state, action) {
      return {
        ...state,
        editTransferRegList: action.payload,
      };
    },
    deleteBullyReg(state, action) {
      return {
        ...state,
        listData: state.listData,
        filters: state.listData.filter(data => data.id !== action.id),
      };
    },
    // pending users
    pendingUsers(state, action) {
      return {
        ...state,
        pendingUsersList: action.payload,
      };
    },
    removeUsers(state, action) {
      return {
        ...state,
        pendingUsersList: state.pendingUsersList.filter(data => data.id !== action.id),
      };
    },
    submittedByData(state, action) {
      const dataVal = action.payload.trim();
      let searchRes = state.filters;
      const title = new RegExp(dataVal, 'gi');
      if (dataVal.length) {
        searchRes = state.filters.filter(scenario => scenario.submittedBy.loginName.match(title));
      } else {
        searchRes = state.filters;
      }
      return {
        ...state,
        sidePanelDetails: searchRes,
        listData: searchRes,
        filters: state.filters,
        list: searchRes,
      };
    },
    pendingById(state, action) {
      return {
        ...state,
        listPendingData: action.payload,
        filters: action.payload,
        sidePanelDetails: action.payload,
      };
    },
  },
};