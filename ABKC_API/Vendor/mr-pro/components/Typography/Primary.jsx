import React from "react";
import PropTypes from "prop-types";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";

import typographyStyle from "../../assets/jss/material-dashboard-react/components/typographyStyle.jsx";

function Primary({ ...props }) {
  const { classes, children } = props;
  return (
    <div className={classes.defaultFontStyle + " " + classes.primaryText}>
      {children}
    </div>
  );
}

Primary.propTypes = {
  classes: PropTypes.object.isRequired
};

const Primary = withStyles(typographyStyle)(Primary);
export {Primary};


// WEBPACK FOOTER //
// ./src/components/Typography/Primary.jsx