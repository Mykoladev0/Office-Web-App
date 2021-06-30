import React, { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
// import isEqual from 'lodash/isEqual';

import Row from 'reactstrap/lib/Row';
import Col from 'reactstrap/lib/Col';
import Modal from 'reactstrap/lib/Modal';
import ModalHeader from 'reactstrap/lib/ModalHeader';
import ModalBody from 'reactstrap/lib/ModalBody';
import ModalFooter from 'reactstrap/lib/ModalFooter';

import Button from 'reactstrap/lib/Button';

import 'react-table/react-table.css';
// import { getShowDetails } from '../api/showService';

import ShowEventsTable from './ShowEventsTable';
import ShowDetails from './ShowDetails';
import { getShowDetails, saveShowEvents } from '../api/showService';
import { getClassTemplates, getStyles, getBreeds } from '../api/classAndStyleService';
import AddEvent from './AddEvent';

// Importing and using in a jsx
import modalStyles from '../styles/ShowEvents.css';

export interface ShowEventsProps extends RouteComponentProps<any> {
  showId: number;
}

export interface ShowEventsState {
  showId: number;
  show: any;
  isLoading: boolean;
  addEventModalShown: boolean;
  classTemplates: any[];
  styles: any[];
  breeds: any[];
  curEventDetails: any;
}

export default withRouter(
  class ShowEvents extends Component<ShowEventsProps, ShowEventsState> {
    public static getDerivedStateFromProps(nextProps, prevState) {
      // do things with nextProps.someProp and prevState.cachedSomeProp
      return {
        showId: nextProps.location.state.showId
          ? nextProps.location.state.showId
          : nextProps.showId,
      };
    }
    public state: ShowEventsState = {
      showId: null,
      show: null,
      isLoading: true,
      addEventModalShown: false,
      styles: [],
      classTemplates: [],
      breeds: [],
      curEventDetails: {},
    };
    public constructor(props: ShowEventsProps) {
      super(props);
      this.handleAddEventClick = this.handleAddEventClick.bind(this);
      this.hideEventModal = this.hideEventModal.bind(this);
      this.handleDetailChanged = this.handleDetailChanged.bind(this);
      this.SaveEventDetails = this.SaveEventDetails.bind(this);
    }

    public componentDidMount() {
      const { showId } = this.state;
      getShowDetails(showId).then(show => {
        this.setState({ show: show.data, isLoading: false });
      });
      getBreeds().then(({ breeds }) => {
        this.setState({ breeds });
      });
      getClassTemplates().then(({ classTemplates }) => {
        //clean templates (getting distinct on name and sorting by sortorder)
        if (classTemplates && Array.isArray(classTemplates)) {
          const distinct = classTemplates
            .filter((obj, pos, arr) => {
              return arr.map(mapObj => mapObj['name']).indexOf(obj['name']) === pos;
            })
            .sort((a, b) => a.sortOrder - b.sortOrder);
          this.setState({ classTemplates: distinct });
        } else {
          this.setState({ classTemplates: [] });
        }
      });
      getStyles().then(({ styles }) => {
        this.setState({ styles });
      });
    }

    public render(): JSX.Element {
      const {
        show,
        isLoading,
        addEventModalShown,
        curEventDetails,
        classTemplates,
        styles,
        breeds,
      } = this.state;
      const canSave: boolean =
        curEventDetails !== null &&
        curEventDetails.results !== undefined &&
        Object.keys(curEventDetails.results[curEventDetails.results.length - 1]).length > 0;
      return (
        <div>
          <Row>
            <Col>
              {show && <ShowDetails show={show} handleAddEventFn={this.handleAddEventClick} />}
            </Col>
          </Row>
          <br />
          <Row>
            <Col>
              <ShowEventsTable show={show} isLoading={isLoading} />
            </Col>
          </Row>
          <br />
          <Row>
            <Col />
          </Row>
          {addEventModalShown && (
            <Modal
              isOpen={addEventModalShown}
              toggle={this.hideEventModal}
              style={modalStyles.modalFull}
              centered={true}
            >
              <ModalHeader toggle={this.hideEventModal}>Add Event to Current Show</ModalHeader>
              <ModalBody>
                <AddEvent
                  showId={show.showId}
                  handleDetailChanged={this.handleDetailChanged}
                  eventDetails={curEventDetails}
                  classTemplates={classTemplates}
                  styles={styles}
                  breeds={breeds}
                />
              </ModalBody>
              <ModalFooter>
                <Button color="success" disabled={!canSave} onClick={this.SaveEventDetails}>
                  Save Show Event
                </Button>{' '}
                <Button color="secondary" onClick={this.hideEventModal}>
                  Cancel
                </Button>
              </ModalFooter>
            </Modal>
          )}
        </div>
      );
    }

    private SaveEventDetails() {
      const { curEventDetails, showId } = this.state;
      saveShowEvents(showId, curEventDetails).then(savedEvents =>
        //want to enable this to force refreshing of show
        getShowDetails(showId).then(show => {
          this.setState({
            show: show.data,
            isLoading: false,
            addEventModalShown: false,
            curEventDetails: {},
          });
        })
      );
    }

    private handleAddEventClick() {
      //show modal
      this.setState({ addEventModalShown: true });
    }
    private hideEventModal() {
      this.setState({ addEventModalShown: false, curEventDetails: {} });
    }
    private handleDetailChanged(eventDetails) {
      this.setState({ curEventDetails: eventDetails });
    }
    // private GetWinner(event) {
    //   //find winner in event and return abkc
    //   if (!event.results || event.results === null) {
    //     return null;
    //   }
    //   if (event.results.length === 1) {
    //     return event.results[0];
    //   }
    //   const winner = event.results.reduce(
    //     (prev, current) => (prev.points > current.points ? prev : current)
    //   );
    //   return winner;
    // }
  }
);
