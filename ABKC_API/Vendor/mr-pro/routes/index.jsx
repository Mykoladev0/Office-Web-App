import Pages from "layouts/Pages.jsx";
import RTL from "layouts/RTL.jsx";
import Dashboard from "layouts/Dashboard.jsx";
import Documentation from "layouts/Documentation/Documentation.jsx";

var indexRoutes = [
  { path: "/rtl", component: RTL },
  { path: "/pages", component: Pages },
  { path: "/documentation", component: Documentation },
  { path: "/", component: Dashboard }
];

export default indexRoutes;



// WEBPACK FOOTER //
// ./src/routes/index.jsx