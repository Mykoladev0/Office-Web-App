import React, { Component } from 'react';
import { withRouter, RouteComponentProps, Link } from 'react-router-dom';
import dashboardStyle from '../../../../Vendor/mr-pro/assets/jss/material-dashboard-react/views/dashboardStyle.jsx';

// @material-ui/core
import withStyles, { WithStyles } from '@material-ui/core/styles/withStyles';
import Tooltip from '@material-ui/core/Tooltip';
// import Grid from "@material-ui/core/Grid";
// @material-ui/icons
import ContentCopy from '@material-ui/icons/ContentCopy';
import Store from '@material-ui/icons/Store';
import InfoOutline from '@material-ui/icons/InfoOutline';
import PeopleOutline from '@material-ui/icons/PeopleOutline';
import Warning from '@material-ui/icons/Warning';
import DateRange from '@material-ui/icons/DateRange';
import LocalOffer from '@material-ui/icons/LocalOffer';
import Update from '@material-ui/icons/Update';
import Accessibility from '@material-ui/icons/Accessibility';
import BugReport from '@material-ui/icons/BugReport';
import Code from '@material-ui/icons/Code';
import Cloud from '@material-ui/icons/Cloud';
import ArtTrack from '@material-ui/icons/ArtTrack';
import Refresh from '@material-ui/icons/Refresh';
import Edit from '@material-ui/icons/Edit';
import Place from '@material-ui/icons/Place';

import LocalPlay from '@material-ui/icons/LocalPlay';
import CheckBox from '@material-ui/icons/CheckBox';

// core components
import { GridItem, GridContainer } from '../../../../Vendor/mr-pro/components/Grid';

import {
  Table,
  Tasks,
  CustomTabs,
  Button,
  CustomInput,
} from '../../../../Vendor/mr-pro/components';
import {
  Card,
  CardBody,
  CardHeader,
  CardIcon,
  CardFooter,
} from '../../../../Vendor/mr-pro/components/Card';

// import Tasks from "../../../../Vendor/material-react/components/Tasks/Tasks.jsx";
// import CustomTabs from "../../../../Vendor/material-react/components/CustomTabs/CustomTabs.jsx";
// import Danger from "../../../../Vendor/material-react/components/Typography/Danger.jsx";
import { Danger, Primary } from '../../../../Vendor/mr-pro/components/Typography';

// import { bugs, website, server } from "../../../../Vendor/material-react/variables/general";
import { bugs, website, server } from '../../../../Vendor/mr-pro/variables/general.jsx';

import priceImage1 from '../../../../Vendor/mr-pro/assets/img/card-2.jpg';
import priceImage2 from '../../../../Vendor/mr-pro/assets/img/card-3.jpg';
import priceImage3 from '../../../../Vendor/mr-pro/assets/img/card-1.jpg';
import Pets from '@material-ui/icons/Pets';
import ViewList from '@material-ui/icons/ViewList';
import ChildFriendly from '@material-ui/icons/ChildFriendly';
import { getAllDogsCount } from '../../Api/dogService';
import { getOwnersCount } from '../../Api/ownerService';
import { getLittersCount } from '../../Api/litterService';
import { getShowsCount } from '../../Api/showService';
import Search from '@material-ui/icons/Search';
import DogCard from '../Dogs/DogCard/DogCard';
import OwnerCard from '../Owners/OwnerCard/OwnerCard';
import UpcomingShows from '../Shows/UpcomingShows';

export interface DashboardProps extends WithStyles<typeof dashboardStyle> {
  // classes: ClassesInterface;
}
type Props = DashboardProps & RouteComponentProps;
interface DashBoardState {
  ownerCount: number;
  litterCount: number;
  showCount: number;
}

class Dashboard extends React.Component<Props, DashBoardState> {
  public state = {
    dogCount: 0,
    ownerCount: 0,
    litterCount: 0,
    showCount: 0,
  };
  public constructor(props) {
    super(props);
  }

  public componentDidMount() {
    getLittersCount().then(litterCount => this.setState({ litterCount }));
    getShowsCount().then(showCount => {
      this.setState({ showCount });
    });
  }

  public render(): JSX.Element {
    const { classes } = this.props;
    const { litterCount, showCount } = this.state;
    return (
      <div>
        <GridContainer container>
          <GridItem xs={12} sm={6} md={3}>
            <DogCard />
          </GridItem>
          <GridItem xs={12} sm={6} md={3}>
            <OwnerCard />
          </GridItem>
          <GridItem xs={12} sm={6} md={3}>
            <Card>
              <CardHeader color="danger" stats icon>
                <CardIcon color="danger">
                  <LocalPlay />
                </CardIcon>
                <p className={classes.cardCategory}>Total Shows</p>
                <h3 className={classes.cardTitle}>{showCount.toLocaleString()}</h3>
              </CardHeader>
              <CardFooter stats>
                <div className={classes.stats}>
                  <Primary>
                    <CheckBox />
                  </Primary>
                  <Link to="/Shows">View All Shows</Link>
                </div>
              </CardFooter>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={6} md={3}>
            <Card>
              <CardHeader color="info" stats icon>
                <CardIcon color="info">
                  <ChildFriendly />
                </CardIcon>
                <p className={classes.cardCategory}>Litters</p>
                <h3 className={classes.cardTitle}>{litterCount.toLocaleString()}</h3>
              </CardHeader>
              <CardFooter stats>
                <div className={classes.stats}>
                  <Primary>
                    <ViewList />
                  </Primary>
                  <Link to="/Litters">View All Litters</Link>
                </div>
              </CardFooter>
            </Card>
          </GridItem>
        </GridContainer>
        <GridContainer>
          <GridItem xs={12} sm={12} md={6}>
            <CustomTabs
              title="Waiting Approval:"
              headerColor="primary"
              tabs={[
                {
                  tabName: 'Dogs',
                  tabIcon: BugReport,
                  tabContent: (
                    <Tasks checkedIndexes={[0, 3]} tasksIndexes={[0, 1, 2, 3]} tasks={bugs} />
                  ),
                },
                {
                  tabName: 'Owners',
                  tabIcon: Code,
                  tabContent: <Tasks checkedIndexes={[0]} tasksIndexes={[0, 1]} tasks={website} />,
                },
                {
                  tabName: 'Shows',
                  tabIcon: Cloud,
                  tabContent: (
                    <Tasks checkedIndexes={[1]} tasksIndexes={[0, 1, 2]} tasks={server} />
                  ),
                },
                {
                  tabName: 'Litters',
                  tabIcon: Cloud,
                  tabContent: (
                    <Tasks checkedIndexes={[1]} tasksIndexes={[0, 1, 2]} tasks={server} />
                  ),
                },
              ]}
            />
          </GridItem>
          <GridItem xs={12} sm={12} md={6}>
            <UpcomingShows classes={classes} />
          </GridItem>
        </GridContainer>
        {/* <h3>Manage Listings</h3>
        <br />
        <GridContainer>
          <GridItem xs={12} sm={12} md={4}>
            <Card product className={classes.cardHover}>
              <CardHeader image className={classes.cardHeaderHover}>
                <a href="#pablo" onClick={e => e.preventDefault()}>
                  <img src={priceImage1} alt="..." />
                </a>
              </CardHeader>
              <CardBody>
                <div className={classes.cardHoverUnder}>
                  <Tooltip
                    id="tooltip-top"
                    title="View"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="transparent" simple justIcon>
                      <ArtTrack className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Edit"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="success" simple justIcon>
                      <Refresh className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Remove"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="danger" simple justIcon>
                      <Edit className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                </div>
                <h4 className={classes.cardProductTitle}>
                  <a href="#pablo" onClick={e => e.preventDefault()}>
                    Cozy 5 Stars Apartment
                  </a>
                </h4>
                <p className={classes.cardProductDescription}>
                  The place is close to Barceloneta Beach and bus stop just 2 min by walk and near
                  to "Naviglio" where you can enjoy the main night life in Barcelona.
                </p>
              </CardBody>
              <CardFooter product>
                <div className={classes.price}>
                  <h4>$899/night</h4>
                </div>
                <div className={`${classes.stats} ${classes.productStats}`}>
                  <Place /> Barcelona, Spain
                </div>
              </CardFooter>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={12} md={4}>
            <Card product className={classes.cardHover}>
              <CardHeader image className={classes.cardHeaderHover}>
                <a href="#pablo" onClick={e => e.preventDefault()}>
                  <img src={priceImage2} alt="..." />
                </a>
              </CardHeader>
              <CardBody>
                <div className={classes.cardHoverUnder}>
                  <Tooltip
                    id="tooltip-top"
                    title="View"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="transparent" simple justIcon>
                      <ArtTrack className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Edit"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="success" simple justIcon>
                      <Refresh className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Remove"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="danger" simple justIcon>
                      <Edit className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                </div>
                <h4 className={classes.cardProductTitle}>
                  <a href="#pablo" onClick={e => e.preventDefault()}>
                    Office Studio
                  </a>
                </h4>
                <p className={classes.cardProductDescription}>
                  The place is close to Metro Station and bus stop just 2 min by walk and near to
                  "Naviglio" where you can enjoy the night life in London, UK.
                </p>
              </CardBody>
              <CardFooter product>
                <div className={classes.price}>
                  <h4>$1.119/night</h4>
                </div>
                <div className={`${classes.stats} ${classes.productStats}`}>
                  <Place /> London, UK
                </div>
              </CardFooter>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={12} md={4}>
            <Card product className={classes.cardHover}>
              <CardHeader image className={classes.cardHeaderHover}>
                <a href="#pablo" onClick={e => e.preventDefault()}>
                  <img src={priceImage3} alt="..." />
                </a>
              </CardHeader>
              <CardBody>
                <div className={classes.cardHoverUnder}>
                  <Tooltip
                    id="tooltip-top"
                    title="View"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="transparent" simple justIcon>
                      <ArtTrack className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Edit"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="success" simple justIcon>
                      <Refresh className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                  <Tooltip
                    id="tooltip-top"
                    title="Remove"
                    placement="bottom"
                    classes={{ tooltip: classes.tooltip }}
                  >
                    <Button color="danger" simple justIcon>
                      <Edit className={classes.underChartIcons} />
                    </Button>
                  </Tooltip>
                </div>
                <h4 className={classes.cardProductTitle}>
                  <a href="#pablo" onClick={e => e.preventDefault()}>
                    Beautiful Castle
                  </a>
                </h4>
                <p className={classes.cardProductDescription}>
                  The place is close to Metro Station and bus stop just 2 min by walk and near to
                  "Naviglio" where you can enjoy the main night life in Milan.
                </p>
              </CardBody>
              <CardFooter product>
                <div className={classes.price}>
                  <h4>$459/night</h4>
                </div>
                <div className={`${classes.stats} ${classes.productStats}`}>
                  <Place /> Milan, Italy
                </div>
              </CardFooter>
            </Card>
          </GridItem>
        </GridContainer> */}
      </div>
    );
  }
}

export default withRouter(withStyles(dashboardStyle)(Dashboard));
