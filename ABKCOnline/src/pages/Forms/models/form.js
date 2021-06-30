import {
  routerRedux
} from 'dva/router';
import {
  notification
} from 'antd';
import {
  fakeSubmitForm,
  getBreedData,
  getColorData,
  uploadDocument,
  submitRegistration,
  searchDamNameCall,
  searchOwnerNameCall,
  getOwnerDetailsCall,
  submitPaymentRegistration
} from '@/services/api';
import {
  startRegistration,
  draftRegistration,
  getRegInfoCall
} from '@/services/pedigreeApi';
import {
  submitJuniorHandlersRegistrationForms,
  startJuniorHandlersRegistrationForms,
  editJuniorHandlersRegistrationForms
} from '@/services/juniorHandlersApi';
import {
  startLitterRegistrations,
  draftLitterRegistrationForms,
  submitLitterRegistrationForms
} from '@/services/litterApi';
import {
  startPuppyRegistrations,
  validateAbkcNumber,
  draftPuppyRegistrationForms,
  submitPuppyRegistrationForms
} from '@/services/puppyApi';
import {
  getDogsByABKCNumbers,
  startTransferRegistrations,
  draftTransferRegistration,
  submitTransferRegistration
} from '@/services/dogTransferApi';
import {
  startBullyRequest,
  submitBullyRegistration
} from '@/services/bullyApi';
import { debug } from 'util';

export default {
  namespace: 'form',

  state: {
    step: {
      payAccounts: 'ant-design@alipay.com',
      receiverAccount: 'test@example.com',
      receiverName: 'Alex',
      amount: '500'
    },
    breeds: [],
    colors: [],
    registration: {},
    status: false,
    statusStart: false,
    documents: [],
    damNameList: [],
    sireNameList: [],
    searchDamName: '',
    ownerName: [],
    searchOwnerName: '',
    ownerDetails: {},
    spinner: true,
    editList: {},
    submitSuccess: false
  },

  effects: {
    * submitRegularForm({
      payload
    }, {
      call
    }) {
      yield call(fakeSubmitForm, payload);
      notification.success('提交成功');
    },
    * submitStepForm({
      payload
    }, {
      call,
      put
    }) {
      yield call(fakeSubmitForm, payload);
      yield put({
        type: 'saveStepFormData',
        payload
      });
      yield put(routerRedux.push('/form/step-form/result'));
    },
    * getBreeds({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(getBreedData, payload);
      yield put({
        type: 'getBreedDetails',
        payload: Array.isArray(response) ? response : []
      });
    },
    * getColors({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(getColorData, payload);
      yield put({
        type: 'getColorDetails',
        payload: Array.isArray(response) ? response : []
      });
    },
    * setStatus({
      payload
    }, {
      put
    }) {
      yield put({
        type: 'updateStatus'
      });
    },
    * setSpinnerStatus({
      payload
    }, {
      put
    }) {
      yield put({
        type: 'updateSpinnerStatus'
      });
    },
    * startPedigreeRegistration({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(startRegistration, payload);
      yield put({
        type: 'startRegistrationDetails',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Pedigree registration started successfully.' });
      } else {
        notification.error({ message: 'Pedigree registration failed.' });
      }
    },
    * draftRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(draftRegistration, payload);
      if (response && content.redirect) {
        yield put(routerRedux.replace('/registration-list'));
      }
      yield put({
        type: 'registrationDetails',
        payload: response,
        docs: payload.documentTypesProvided
      });
      if (response && response.id) {
        notification.success({ message: 'Pedigree registration drafted successfully.' });
      } else {
        notification.error({ message: 'Pedigree registration failed.' });
      }
    },
    * submitSupportinngDoc({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(uploadDocument, payload);
      yield put({
        type: 'registrationUpload',
        payload: response,
        docs: payload.documentTypesProvided
      });
      if (response) {
        notification.success({ message: 'Document uploaded successfully.' });
      } else {
        notification.error({ message: 'Document uploading failed.' });
      }
    },
    * submitRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(draftRegistration, payload);
      if (response && content.callSubmit) {
        const response2 = yield call(submitRegistration, content);
        // redirect
        if (response2 && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
      }
      yield put({
        type: 'registrationDetails',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Form submitted successfully.' });
      } else {
        notification.error({ message: 'Form submission failed.' });
      }
    },
    // payment submit
    * submitRegistrationPaymentForm({ payload, content }, { call, put }) {
      if (content.draftApi === 'bullyId') {
        const response = yield call(submitPaymentRegistration, content);
        // redirect
        if (response && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
        yield put({
          type: 'registrationDetails',
          payload: response
        });
        if (response && response.id) {
          notification.success({ message: 'Form submitted successfully.' });
        } else {
          notification.error({ message: 'Form submission failed.' });
        }
      } else {
        const api = content.draftApi;
        let response;
        if (api === 'draftRegistration') {
          response = yield call(draftRegistration, payload);
        } else if (api === 'draftLitterRegistrationForms') {
          response = yield call(draftLitterRegistrationForms, payload);
        } else if (api === 'editJuniorHandlersRegistrationForms') {
          response = yield call(editJuniorHandlersRegistrationForms, payload);
        } else if (api === 'draftPuppyRegistrationForms') {
          response = yield call(draftPuppyRegistrationForms, payload);
        } else if (api === 'draftTransferRegistration') {
          response = yield call(draftTransferRegistration, payload);
        }
        if (response && content.callSubmit) {
          const response2 = yield call(submitPaymentRegistration, content);
          // redirect
          if (response2 && content.redirect) {
            yield put({
              type: 'paymentQuote',
              payload: response2,
            });
            // yield put(routerRedux.replace('/registration-list'));
          }
        }
        if (response && response.id) {
          yield put({
            type: 'registrationDetails',
            payload: response,
          });
          notification.success({ message: 'Registration details updated successfully.' });
        } else {
          notification.error({ message: 'Form submission failed.' });
        }
      }
    },
    * submitAdvancedForm({
      payload
    }, {
      call
    }) {
      yield call(fakeSubmitForm, payload);
      notification.success('提交成功');
    },
    * searchDamName({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(searchDamNameCall, payload);
      yield put({
        type: 'searchDamNameList',
        payload: response,
        searchDamName: payload
      });
    },
    * searchOwnerName({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(searchOwnerNameCall, payload);
      yield put({
        type: 'searchOwnerNameList',
        payload: response,
        searchOwnerName: payload
      });
    },
    * getOwnerDetails({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(getOwnerDetailsCall, payload);
      yield put({
        type: 'getOwnerDetailsList',
        payload: response
      });
    },

    // Litter Registration
    * startLitterRegistration({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(startLitterRegistrations, payload);
      yield put({
        type: 'startLitterRegistrationDetails',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Litter registration started successfully.' });
      } else {
        notification.error({ message: 'Litter registration failed.' });
      }
    },
    * draftLitterRegistrationForm({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(draftLitterRegistrationForms, payload);
      yield put({
        type: 'litterRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Draft litter registration successfully.' });
      } else {
        notification.error({ message: 'Draft litter registration failed.' });
      }
    },
    * submitLitterRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(draftLitterRegistrationForms, payload);
      if (response && content.callSubmit) {
        const response2 = yield call(submitLitterRegistrationForms, content);
        if (response2 && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
      }
      yield put({
        type: 'litterRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Form submitted successfully.' });
      } else {
        notification.error({ message: 'Form submission failed.' });
      }
    },

    // junior handler registratoion
    * startJuniorHandlersRegistrationForm({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(
        startJuniorHandlersRegistrationForms,
        payload
      );
      if (response && response.id) {
        yield put(routerRedux.replace('/registration-list'));
      }
      yield put({
        type: 'startJuniorHandlersRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Jr. handler registration started successfully.' });
      } else {
        notification.error({ message: 'Jr.handler registration initialization failed.' });
      }
    },
    * draftJuniorHandlersRegistrationForm({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(editJuniorHandlersRegistrationForms, payload);
      if (response && response.id) {
        yield put(routerRedux.replace('/registration-list'));
      }
      yield put({
        type: 'draftJuniorHandlersRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Jr.handler draft registered successfully.' });
      } else {
        notification.error({ message: 'Jr.handler draft registration failed.' });
      }
    },
    * submitJuniorHandlersRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(editJuniorHandlersRegistrationForms, payload);
      if (response && content.callSubmit) {
        const response2 = yield call(
          submitJuniorHandlersRegistrationForms,
          content
        );
        if (response2) {
          notification.success({ message: 'Jr.handler registered successfully.' });
        } else {
          notification.error({ message: 'Jr.handler registration failed.' });
        }
        if (response2 && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
      }
      yield put({
        type: 'submitJuniorHandlersRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Jr.handler draft registered successfully.' });
      } else {
        notification.error({ message: 'Jr.handler draft registration failed.' });
      }
    },

    // Puppy registration
    *validateAbkcNumber({ payload }, { call, put }) {
      yield put({
        type: 'updateSpinnerStatus'
      });
      const response = yield call(validateAbkcNumber, payload);
      if (response.length && response[0].originalTableId) {
        const regResponse = yield call(startPuppyRegistrations, {
          originalTableId: response[0].originalTableId
        });
        if (regResponse && regResponse.id) {
          notification.success({ message: 'Puppy registered succesfully.' });
          yield put({
            type: 'startPuppyRegistrationDetails',
            payload: {
              regResponse,
              originalTableId: response[0].originalTableId
            }
          });
        } else {
          notification.error({ message: "Error:", description: `${regResponse}` });
          yield put({
            type: 'validateAbkcNumbers',
            payload: response
          });
        }
      } else {
        yield put({
          type: 'validateAbkcNumbers',
          payload: response
        });
        notification.error({
          message: 'Error',
          description: 'The dog with mentioned ABKC number is not valid for puppy registration.',
        });
      }
    },
    * draftPuppyRegistrationForm({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(draftPuppyRegistrationForms, payload);
      yield put({
        type: 'puppyRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Puppy draft registered successfully.' });
      } else {
        notification.error({ message: 'Puppy draft registration failed.' });
      }
    },
    * submitPuppyRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(draftPuppyRegistrationForms, payload);
      if (response && content.callSubmit) {
        const response2 = yield call(submitPuppyRegistrationForms, content);
        if (response2 && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
        if (response && response.id) {
          notification.success({ message: 'Puppy registered successfully.' });
        } else {
          notification.error({ message: 'Puppy registration failed.' });
        }
      }
      yield put({
        type: 'puppyRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Puppy draft registered successfully.' });
      } else {
        notification.error({ message: 'Puppy draft registration failed.' });
      }
    },

    // dog transfer
    * getDogsByABKCNumber({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(getDogsByABKCNumbers, payload);
      if (response && response.originalTableId) {
        const regResponse = yield call(startTransferRegistrations, {
          originalTableId: response.originalTableId
        });
        yield put({
          type: 'startTransferRegistrationDetails',
          payload: regResponse
        });
        if (regResponse && regResponse.id) {
          notification.success({
            message: 'Dog transfer registration initialized successfully.'
          });
        } else {
          notification.error({ message: "Error", description: `${regResponse}` });
        }
      } else {
        yield put({
          type: 'startTransferRegistrationDetails',
          payload: response
        });
        notification.error({ message: 'Dog with entered ABKC number does not exist.' });
      }
    },
    * draftTransferRegistrationForm({
      payload
    }, {
      call,
      put
    }) {
      const response = yield call(draftTransferRegistration, payload);
      yield put({
        type: 'transferRegistrationList',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Dog transfer registration drafted successfully.' });
      } else {
        notification.error({ message: 'Dog transfer registration failed.' });
      }
    },
    * submitTransferRegistrationForm({
      payload,
      content
    }, {
      call,
      put
    }) {
      const response = yield call(draftTransferRegistration, payload);
      if (response && content.callSubmit) {
        const response2 = yield call(submitTransferRegistration, content);
        if (response2 === true) {
          notification.success({ message: 'Dog transfer registered successfully.' });
        } else {
          notification.error({ message: 'Dog transfer registration failed.' });
        }
        if (response2 && content.redirect) {
          yield put(routerRedux.replace('/registration-list'));
        }
      }
      else {
        notification.error({ message: 'Dog transfer registration failed.' });
      }
      yield put({
        type: 'transferRegistrationList',
        payload: response
      });
    },
    // bully requests
    * startBullyRequest({
      payload
    }, {
      call,
      put
    }) {
      yield put({
        type: 'updateSpinnerStatus'
      });
      const response = yield call(getDogsByABKCNumbers, payload);
      if (response && response.originalTableId) {
        const regResponse = yield call(startBullyRequest, {
          originalTableId: response.originalTableId
        });
        yield put({
          type: 'startBullyRequests',
          payload: regResponse
        });
        if (regResponse && regResponse.id) {
          notification.success({ message: 'Bully request registered successfully.' });
        } else {
          notification.error({ message: "Error", description: `${regResponse}` });
        }
      } else {
        yield put({
          type: 'startBullyRequests',
          payload: response
        });
        notification.error({ message: 'Dog with entered ABKC number does not exist.' });
      }
    },
    * submitBullyForm({ payload, content }, { call, put }) {
      const response = yield call(submitBullyRegistration, payload);
      if (response && content.redirect) {
        yield put(routerRedux.replace('/registration-list'));
      }
      yield put({
        type: 'registrationDetails',
        payload: response
      });
      if (response && response.id) {
        notification.success({ message: 'Bully request registered successfully.' });
      } else {
        notification.error({ message: 'Bully request registration failed.' });
      }
    },

    // edit list pedigree
    * getRegInfo({ payload }, { call, put }) {
      const response = yield call(getRegInfoCall, payload);
      yield put({
        type: 'getRegInfoData',
        payload: response
      });
    },

    *redirectPage({ payload }, { call, put }) {
      yield put(routerRedux.replace('/registration-list'));
    }
  },

  reducers: {
    saveStepFormData(state, {
      payload
    }) {
      return {
        ...state,
        step: {
          ...state.step,
          ...payload
        }
      };
    },
    getBreedDetails(state, action) {
      return {
        ...state,
        breeds: action.payload
      };
    },
    getColorDetails(state, action) {
      return {
        ...state,
        colors: action.payload
      };
    },
    startRegistrationDetails(state, action) {
      return {
        ...state,
        startRegistration: action.payload,
        statusStart: action.payload ? true : false,
        status: false,
        spinner: false
      };
    },
    registrationDetails(state, action) {
      return {
        ...state,
        registration: action.payload,
        status: action.payload.id ? true : false,
        documents: action.docs,
        editList: action.payload,
      };
    },
    editRegistrationDetails(state, action) {
      return {
        ...state,
        registration: action.payload,
        statusStart: true
      };
    },
    registrationUpload(state) {
      return {
        ...state
      };
    },
    searchDamNameList(state, action) {
      return {
        ...state,
        damNameList: action.payload.filter(
          e => e.gender.toLowerCase() === 'female'
        ),
        sireNameList: action.payload.filter(
          e => e.gender.toLowerCase() === 'male'
        ),
        searchDamName: action.searchDamName,
        spinner: action.payload ? false : true
      };
    },
    searchOwnerNameList(state, action) {
      return {
        ...state,
        ownerName: action.payload,
        searchOwnerName: action.searchOwnerName,
        spinner: false
      };
    },
    getOwnerDetailsList(state, action) {
      return {
        ...state,
        ownerDetails: action.payload
      };
    },
    updateStatus(state, action) {
      return {
        ...state,
        statusStart: false
      };
    },
    updateSpinnerStatus(state, action) {
      return {
        ...state,
        spinner: true
      };
    },

    // Litter Registration
    startLitterRegistrationDetails(state, action) {
      return {
        ...state,
        startLitterRegistration: action.payload,
        statusStart: action.payload ? true : false,
        status: false
      };
    },
    litterRegistrationList(state, action) {
      return {
        ...state,
        submitLitterRegistration: action.payload,
        statusStart: action.payload ? true : false,
        status: false
      };
    },

    // junior
    submitJuniorHandlersRegistrationList(state, action) {
      return {
        ...state,
        submitJuniorHandlersRegistration: action.payload,
        status: false
      };
    },
    editJuniorHandlersRegistrationList(state, action) {
      return {
        ...state,
        editJuniorList: action.payload,
        status: false
      };
    },

    // Puppy Registration
    validateAbkcNumbers(action, state) {
      return {
        ...state,
        validateAbkcNumber: action.payload,
        isAbkcNumberValid: action.payload ? true : false,
        status: false,
        spinner: false
      };
    },
    startPuppyRegistrationDetails(state, action) {
      return {
        ...state,
        startPuppyRegistration: action.payload.regResponse,
        originalTableId: action.payload.originalTableId,
        statusStart: action.payload ? true : false,
        status: false,
        spinner: false
      };
    },
    puppyRegistrationList(state, action) {
      return {
        ...state,
        submitPuppyRegistration: action.payload,
        statusStart: action.payload ? true : false,
        status: false
      };
    },
    startTransferRegistrationDetails(state, action) {
      return {
        ...state,
        startTransferRegistration: action.payload,
        statusStart: action.payload && action.payload.id ? true : false,
        status: false,
        spinner: false
      };
    },
    transferRegistrationList(state, action) {
      return {
        ...state,
        submitTransferRegistration: action.payload,
        statusStart: action.payload ? true : false,
        status: false
      };
    },
    // bully requests
    startBullyRequests(state, action) {
      return {
        ...state,
        startBullyRequest: action.payload,
        statusStart: action.payload && action.payload.id ? true : false,
        status: false,
        spinner: false
      };
    },

    // junior handler
    startJuniorHandlersRegistrationList(state, action) {
      return {
        ...state,
        startJuniorHandler: action.payload,
        statusStart: action.payload ? true : false,
        status: false
      };
    },
    // pedigree paymentquote
    paymentQuote(state, action) {
      return {
        ...state,
        pedigreePaymentQuote: action.payload,
        submitSuccess: action.payload ? true : false,
        statusStart: action.payload ? true : false,
        status: false
      };
    },

    // edit list pedigree
    getRegInfoData(state, action) {
      return {
        ...state,
        editList: action.payload,
        editPedigreeInfo: action.payload,
        editPedigreeStart: action.payload ? true : false,
      };
    },
  }
};