import React, { useState } from "react";
import "antd/dist/antd.css";
import { Layout, Tabs } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import "./main.css";
import MyDog from "./myDog/myDog";
import Shows from "./shows/shows";
import RegisterDog from "./registerDog/registerDog";
import Header from "./header/header";

const MainPage: React.FC = () => {
  const { TabPane } = Tabs;
  const [currentTab, setTab] = useState("1");
  const [toggle, setToggle] = useState(false);
  const toggleClicked = () => {
    setToggle(!toggle);
  };
  const { Sider, Content } = Layout;
  const tabClicked = (key: string) => {
    setTab(key);
    console.log(key);
  };

  return (
    <Layout className="main-screen">
      <Sider trigger={null} collapsible collapsed={toggle}>
        <div className="sider-logo">
          <img src={require("../../assets/img/logo.png")} alt="logo" />
        </div>
        <div className="toggle-bar">
          {React.createElement(toggle ? PlusOutlined : PlusOutlined, {
            className: "trigger",
            onClick: toggleClicked,
          })}
          {/* <PlusOutlined /> */}
        </div>
        <Tabs onChange={(key: string) => tabClicked(key)}>
          <TabPane tab="My Dogs" key="1"></TabPane>
          <TabPane tab="Shows" key="2"></TabPane>
          <TabPane tab="Register Dog" key="3"></TabPane>
        </Tabs>
      </Sider>
      <Layout>
        <Header
          toggleClicked={() => toggleClicked()}
          toggleState={toggle}
          Title={
            currentTab === "1"
              ? "My dog"
              : currentTab === "2"
              ? "Shows"
              : currentTab === "3"
              ? "Register Dog"
              : ""
          }
        />
        {currentTab === "1" ? (
          <Content>
            <MyDog></MyDog>
          </Content>
        ) : currentTab === "2" ? (
          <Content className="shows">
            <Shows></Shows>
          </Content>
        ) : currentTab === "3" ? (
          <Content className="site-layout-background">
            <RegisterDog></RegisterDog>
          </Content>
        ) : (
          ""
        )}
      </Layout>
    </Layout>
  );
};

export default MainPage;
