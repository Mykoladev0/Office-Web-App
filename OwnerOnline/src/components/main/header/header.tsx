import React from "react";
import { Layout, Avatar, Dropdown } from "antd";
import "../main.css";
import "./header.css";
import { UserOutlined } from "@ant-design/icons";
import {
  MenuUnfoldOutlined,
  MenuFoldOutlined,
  DownOutlined,
} from "@ant-design/icons";

const Header: React.FC<{
  toggleClicked: any;
  toggleState: boolean;
  Title: string;
}> = ({ toggleClicked, toggleState ,Title}) => {
  const { Header } = Layout;
  const childtoggle = () => {
    toggleClicked();
  };
  return (
    <React.Fragment>
      <Header className="head">
        {React.createElement(
          toggleState ? MenuUnfoldOutlined : MenuFoldOutlined,
          {
            className: "trigger",
            onClick: childtoggle,
          }
        )}
        <h1>{Title}</h1>
        <Avatar size="large" icon={<UserOutlined />} />
        <span className="user-label">James Brooks</span>
        <Dropdown
          overlay={
            <ul>
              <li>Sign Out</li>
              <li>Profile</li>
            </ul>
          }
          trigger={["click"]}
        >
          <DownOutlined
            className="ant-dropdown-link"
            onClick={(e) => e.preventDefault()}
          />
        </Dropdown>
      </Header>
    </React.Fragment>
  );
};

export default Header;
