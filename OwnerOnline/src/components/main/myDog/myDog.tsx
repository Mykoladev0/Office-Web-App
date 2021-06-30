import React, { useState, useEffect } from "react";
import "antd/dist/antd.css";
import { Card, Menu, Dropdown, Button, Input, Modal, Typography } from "antd";
import { MoreOutlined } from "@ant-design/icons";
import * as dogService from "../../../services/dogService";
import { Dog } from "../../../models/dogModel";
import "../main.css";
import "./myDog.css";

const MyDog: React.FC = () => {
  useEffect(() => {
    dogService.getDogs().then((res: Dog[]) => {
      console.log(res);
      setDogList(res);
    });
  }, []);
  const [dogList, setDogList] = useState<Dog[]>([]);
  const [visible, setVisible] = useState(false);

  const { Search } = Input;
  const { Text } = Typography;

  const handleCancel = () => {
    setVisible(false);
  };
  const showModal = () => {
    setVisible(true);
  };

  const handleSearch = (value: string) => {
    console.log(value);
    dogService.searchDogs(value).then((res: Dog[]) => {
      console.log(res);
      setDogList(res);
    });
  };

  const handleOk = () => {
    setVisible(false);
  };

  return (
    <React.Fragment>
      <div className="search-input">
        <h2>Dogs</h2>
        <Search
          placeholder="input search text"
          onSearch={(value: string) => handleSearch(value)}
        />
      </div>
      <div className="cards">
        {dogList.map((dog: Dog) => (
          <Card>
            <div className="card-image">
              <img src={require("../../../assets/img/dog.png")} alt="logo" />
            </div>
            <div className="card-content">
              <div className="card-head">
                <h2>{dog.DogName}</h2>
                {
                  <Dropdown
                    overlay={
                      <Menu>
                        <Menu.Item key="0">Scan QR Code</Menu.Item>
                      </Menu>
                    }
                    trigger={["click"]}
                  >
                    <a
                      href="!#"
                      className="card-dropdown"
                      onClick={e => e.preventDefault()}
                    >
                      <MoreOutlined />
                    </a>
                  </Dropdown>
                }
              </div>
              <div className="card-content-body">
                <div className="card-detail">
                  <Text type="secondary">
                    Gender : <span>{dog.Gender}</span>
                  </Text>
                  <Text type="secondary">
                    Bread : <span>{dog.Breed}</span>
                  </Text>
                  <Text type="secondary">
                    Date of Birth : <span>{dog.Birthdate}</span>
                  </Text>
                </div>

                <Button type="primary" className="btn-card" onClick={showModal}>
                  <span>View</span>
                </Button>
              </div>
            </div>
          </Card>
        ))}
        <Modal
          visible={visible}
          onOk={handleOk}
          onCancel={handleCancel}
          className="view-modal"
        >
          <div className="modal-img">
            <img src={require("../../../assets/img/dog.png")} alt="logo" />
          </div>

          <div className="modal-content">
            <h5>YOUR DOG</h5>
            <div className="card-detail modal-detail">
              <h1>Dagher</h1>
              <Text type="secondary">
                Gender : <span>Male</span>
              </Text>
              <Text type="secondary">
                Bread : <span>German</span>
              </Text>
              <Text type="secondary">
                Date of Birth : <span>23 March 2008</span>
              </Text>
              <Text type="secondary">
                Owner Name : <span>James Brooks</span>
              </Text>
            </div>
            <div className="modal-btn">
              <Button type="primary" className="btn-card" onClick={showModal}>
                <span>View Pedigree</span>
              </Button>
              <Button
                type="primary"
                className="btn-card"

              >
                <span>Generate PDF ancestry</span>
              </Button>
              <Button type="primary" className="btn-card">
                <span>Register for show</span>
              </Button>
              <Button type="primary" className="btn-card">
                <span>View Event Badge</span>
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </React.Fragment>
  );
};

export default MyDog;
