// ##############################
// // // ExtendedForms view styles
// #############################

import { cardTitle } from "../../material-dashboard-pro-react.jsx";
import customSelectStyle from "../../material-dashboard-react/customSelectStyle.jsx";
import customCheckboxRadioSwitch from "../../material-dashboard-react/customCheckboxRadioSwitch.jsx";

const extendedFormsStyle = {
  ...customCheckboxRadioSwitch,
  ...customSelectStyle,
  cardTitle,
  cardIconTitle: {
    ...cardTitle,
    marginTop: "15px",
    marginBottom: "0px"
  },
  label: {
    cursor: "pointer",
    paddingLeft: "0",
    color: "rgba(0, 0, 0, 0.26)",
    fontSize: "14px",
    lineHeight: "1.428571429",
    fontWeight: "400",
    display: "inline-flex"
  }
};

export default extendedFormsStyle;



// WEBPACK FOOTER //
// ./src/assets/jss/material-dashboard-pro-react/views/extendedFormsStyle.jsx