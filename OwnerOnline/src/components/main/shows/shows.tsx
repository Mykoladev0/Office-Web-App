import React, { useState, useEffect } from "react";
import "antd/dist/antd.css";
import { Card, Input, Typography, Radio, Dropdown, Modal, Button } from "antd";
import { FilterFilled, StopOutlined } from "@ant-design/icons";
import "../main.css";
import "./shows.css";
import * as showService from "../../../services/showsService";
import { Show } from "../../../models/showModel";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapMarkerAlt } from "@fortawesome/free-solid-svg-icons";
import { useHistory } from "react-router-dom";

const Shows: React.FC = () => {
  const history = useHistory();
  const { Meta } = Card;
  const { Search } = Input;
  const { Text } = Typography;
  const [shows, setShows] = useState<Show[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [filter, setFilter] = useState(1);
  const onChange = (e: any) => {
    setFilter(e.target.value);
    showService.searchShows(e.target.value, searchTerm).then((res: Show[]) => {
      console.log(res);
      setShows(res);
    });
  };
  const [visible, setVisible] = useState(false);
  const showModal = () => {
    setVisible(true);
  };
  const handleOk = () => {
    setVisible(false);
  };
  const handleCancel = () => {
    setVisible(false);
  };

  const onSearch = (value: string) => {
    setSearchTerm(value);
    showService.searchShows(filter, value).then((res: Show[]) => {
      console.log(res);
      setShows(res);
    });
  };

  const navigateTo = (id: number) => {
    history.push("show-detail/" + id);
  };
  useEffect(() => {
    showService.searchShows(filter, searchTerm).then((res: Show[]) => {
      console.log(res);
      setShows(res);
    });
  }, []); // eslint-disable-line react-hooks/exhaustive-deps
  return (
    <React.Fragment>
      <div className="search-input">
        <h2>Upcoming Shows</h2>
        <Search
          placeholder="Search show by name, location, date"
          onSearch={(value: string) => onSearch(value)}
        />
        <Dropdown
          overlay={
            <Radio.Group onChange={onChange} value={filter}>
              <h4>Filter By:</h4>
              <Radio value={1}>Upcoming Shows</Radio>
              <Radio value={2}>Register Shows</Radio>
              <Radio value={3}>Shows Histry</Radio>
            </Radio.Group>
          }
          trigger={["click"]}
        >
          <FilterFilled
            className="ant-dropdown-link"
            onClick={(e) => e.preventDefault()}
          />
        </Dropdown>
      </div>
      <div className="cards">
        {shows.map((show) => (
          <React.Fragment>
            {show.type === 1 ? (
              <Card>
                <div className="card-image">
                  <img
                    src={require("../../../assets/img/show.png")}
                    alt="logo"
                  />
                </div>
                <div className="card-content">
                  <h3>{show.showDate}</h3>
                  <Meta title={show.showName} />
                  <Text type="secondary">
                    Bread : <span>{show.breedsShown}</span>
                  </Text>
                  <Text className="location">
                    <FontAwesomeIcon
                      icon={faMapMarkerAlt}
                      className="icon-loc"
                    />
                    {show.address}
                  </Text>
                  <a onClick={() => navigateTo(show.showId)}>Register Now</a>
                </div>
              </Card>
            ) : show.type === 2 ? (
              <Card className="reg-show">
                <div className="card-image">
                  <div className="card-content">
                    <h3>{show.showDate}</h3>
                    <Meta title={show.showName} />
                    <Text className="location">
                      <FontAwesomeIcon
                        icon={faMapMarkerAlt}
                        className="icon-loc"
                      />
                      {show.address}
                    </Text>
                  </div>
                </div>
                <div className="card-content">
                  <h2>Registered Dogs</h2>
                  <div className="dog-avatar">
                    <img
                      src={require("../../../assets/img/dog.png")}
                      alt="logo"
                    />
                    <h3>Tommy</h3>
                  </div>
                  <a onClick={showModal}>Cancel Registration</a>
                </div>

                {/* cancel Modal Start here */}

                <Modal
                  visible={visible}
                  onOk={handleOk}
                  onCancel={handleCancel}
                  className="cancel-modal"
                >
                  <div className="cancel-modal-content">
                    {/* <div className="cancel-icon">
              
              </div> */}
                    <StopOutlined />

                    <h1>Cancel Show?</h1>
                    <h5>
                      Are you sure you want to cancel this show registration?
                    </h5>
                    <div className="confirm-modal-btn">
                      <Button
                        type="primary"
                        className="btn-card"
                        onClick={handleCancel}
                      >
                        <span>No</span>
                      </Button>
                      <Button
                        type="primary"
                        className="btn-card"
                        onClick={handleOk}
                      >
                        <span>Yes</span>
                      </Button>
                    </div>
                  </div>
                </Modal>
              </Card>
            ) : show.type === 3 ? (
              <Card className="reg-show">
                <div className="card-image">
                  <div className="card-content">
                    <h3>{show.showDate}</h3>
                    <Meta title={show.showName} />
                    <Text className="location">
                      <FontAwesomeIcon
                        icon={faMapMarkerAlt}
                        className="icon-loc"
                      />
                      {show.address}
                    </Text>
                  </div>
                </div>
                <div className="card-content">
                  <h2>Registered Dogs</h2>
                  <div className="dog-avatar">
                    <img
                      src={require("../../../assets/img/dog.png")}
                      alt="logo"
                    />
                    <h3>Daugher</h3>
                  </div>
                </div>
              </Card>
            ) : (
              ""
            )}
          </React.Fragment>
        ))}
      </div>
    </React.Fragment>
  );
};

export default Shows;
