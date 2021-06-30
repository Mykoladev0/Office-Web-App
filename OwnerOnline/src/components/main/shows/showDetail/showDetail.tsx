import React, { useState, useEffect } from "react";
import { Button, Modal, Typography, Layout, Card } from "antd";
import "../../main.css";
import "./showDetail.css";
import { ArrowLeftOutlined } from "@ant-design/icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapMarkerAlt } from "@fortawesome/free-solid-svg-icons";
import Header from "../../header/header";
import * as showService from "../../../../services/showsService";
import { Show } from "../../../../models/showModel";
import * as dogService from "../../../../services/dogService";
import { Dog } from "../../../../models/dogModel";

const { Content } = Layout;

const ShowDetail: React.FC = (props: any) => {
  const { Text } = Typography;
  const [registerModal, setRegisterModal] = useState(false);
  const [confirmModal, setConfirmModal] = useState(false);
  const [toggle, setToggle] = useState(false);
  const [showDetail, setShowDetail] = useState<any>();
  useEffect(() => {
    showService.getShowById(props.match.params.id).then((res: Show) => {
      setShowDetail(res);
    });
    dogService.getDogs().then((res: Dog[]) => {
      setDogList(res);
    });
  }, []); // eslint-disable-line react-hooks/exhaustive-deps
  const [dogList, setDogList] = useState<Dog[]>([]);
  const [selectedDogs, setSelectedDogs] = useState<Dog[]>([]);
  const toggleClicked = () => {
    setToggle(!toggle);
  };
  const showSelectionModal = () => {
    setRegisterModal(true);
  };
  const showConfirmModal = () => {
    setRegisterModal(false);
    setConfirmModal(true);
  };
  const confirmationOk = () => {
    setConfirmModal(false);
  };
  const handleSelection = (dog: Dog) => {
    if (selectedDogs.find((x) => x == dog)) {
      setSelectedDogs(selectedDogs.filter((x) => x != dog));
    } else {
      setSelectedDogs([...selectedDogs, dog]);
    }
    console.log("check", selectedDogs);
  };
  const confirmationCancel = () => {
    setConfirmModal(false);
  };
  const registerOk = () => {
    setRegisterModal(false);
  };
  const registerCancel = () => {
    setRegisterModal(false);
  };
  return (
    
    <React.Fragment>
      
      <Layout>
        <Header
          toggleClicked={() => toggleClicked}
          toggleState={toggle}
          Title="Show Detail"
        />
     
      <Content>
      <div className="show-detail">
        <a href="/">
          <ArrowLeftOutlined />
          Back
        </a>
        <div className="show-detail-content">
          <div className="show-detail-img">
            <img src={require("../../../../assets/img/dog.png")} alt="logo" />
          </div>
          
          <div className="modal-content card-detail">
            <div className="date">
              <hr></hr>
              <span>{showDetail?.showDate}</span>
            </div>
            <Text strong className="mb">
              {showDetail?.showName}
            </Text>
            <Text type="secondary">
              Location : <span>{showDetail?.address}</span>
            </Text>
            <Text type="secondary">
              Date of show : <span>{showDetail?.showDate}</span>
            </Text>
            <Text type="secondary">
              Breeds : <span>{showDetail?.breedsShown}</span>
            </Text>
            <Text type="secondary">
              classes : <span>{showDetail?.stylesShown}</span>
            </Text>
            <Button
              type="primary"
              className="btn-card"
              onClick={showSelectionModal}
            >
              <span>Register for show</span>
            </Button>
          </div>
          {/* Register Modal */}
          <Modal
            title="Register Your Dog for Show"
            visible={registerModal}
            onOk={registerOk}
            onCancel={registerCancel}
            className="show-modal"
          >
            <div className="modal-cards">
              {dogList.map((dog: any) => (
                <label className="label">
                  <input
                    type="checkbox"
                    onChange={() => handleSelection(dog)}
                  />
                  <Card>
                    <img
                      src={require("../../../../assets/img/dog.png")}
                      alt="logo"
                    />
                    <h2>{dog.DogName}</h2>
                  </Card>
                </label>
              ))}
            </div>
            <Button
              type="primary"
              className="btn-card"
              onClick={showConfirmModal}
            >
              <span>Submit</span>
            </Button>
          </Modal>

          {/* Confirmation Modal start here */}

          <Modal
            visible={confirmModal}
            onOk={confirmationOk}
            onCancel={confirmationCancel}
            className="confirm-modal"
          >
            <div className="confirm-modal-content">
              <img
                src={require("../../../../assets/img/confirm.png")}
                alt="logo"
              />
              <span>Are you sure you want to register your dogs for?</span>
              <h1>American bully kennel club show</h1>
              <span>Sep 16, 2020</span>
              <div className="confirm-modal-images">
                {selectedDogs.map((dog) => (
                  <div>
                    <img
                      src={require("../../../../assets/img/dog.png")}
                      alt="logo"
                    />
                    <h2>{dog.DogName}</h2>
                  </div>
                ))}
              </div>
              <div className="confirm-modal-btn">
                <Button
                  type="primary"
                  className="btn-card"
                  onClick={confirmationCancel}
                >
                  <span>No</span>
                </Button>
                <Button
                  type="primary"
                  className="btn-card"
                  onClick={confirmationOk}
                >
                  <span>Yes</span>
                </Button>
              </div>
            </div>
          </Modal>
        </div>
      </div>
      </Content>
      </Layout>
    </React.Fragment>
  );
};

export default ShowDetail;
