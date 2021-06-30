// ##############################
// // // ValidationForms view styles
// #############################

import { cardTitle } from "../../material-dashboard-pro-react.jsx";
import customCheckboxRadioSwitch from "../../material-dashboard-react/customCheckboxRadioSwitch.jsx";

const validationFormsStyle = {
  ...customCheckboxRadioSwitch,
  cardTitle: {
    ...cardTitle,
    color: "#FFFFFF"
  },
  cardIconTitle: {
    ...cardTitle,
    marginTop: "15px",
    marginBottom: "0px"
  },
  formCategory: {
    marginBottom: "0",
    color: "#999999",
    fontSize: "14px",
    padding: "10px 0 10px"
  },
  center: {
    textAlign: "center"
  },
  justifyContentCenter: {
    justifyContent: "center"
  },
  registerButton: {
    float: "right"
  }
};

export default validationFormsStyle;



// WEBPACK FOOTER //
// ./src/assets/jss/material-dashboard-pro-react/views/validationFormsStyle.jsx