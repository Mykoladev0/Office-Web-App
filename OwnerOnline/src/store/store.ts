import { compose, createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import rootReducer from "./reducers";
import { loadState } from "./localStorage";

const middleware = [thunk];
const persistedState = loadState();
const store = createStore(
  rootReducer,
  persistedState,
  compose(applyMiddleware(...middleware))
);

export default store;
