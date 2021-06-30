import React from "react";
import "antd/dist/antd.css";
import {
  Button,
  Input,
  Form,
  Select,
  DatePicker,
  Tag,
  Upload,
  Tabs,
  Radio,
} from "antd";
import {
  CloudUploadOutlined,
  DeleteOutlined,
  PlusOutlined,
} from "@ant-design/icons";
import "../main.css";
import "./registerDog.css";
import * as dogService from "../../../services/dogService";

const { TabPane } = Tabs;

const normFile = (e: { fileList: any }) => {
  console.log("Upload event:", e);

  if (Array.isArray(e)) {
    return e;
  }

  return e && e.fileList;
};

function log(e: any) {
  console.log(e);
}

const handleSubmit = (values: any) => {
  console.log(values);
  dogService.registerDog(values);
};

const RegisterDog: React.FC = () => {
  return (
    <React.Fragment>
      <div className="register-form">
        <div className="register-head">
          <h1>Dogs Information</h1>
          </div>
          <Tabs>
            <TabPane key="1">
              <div className="tags">
                <Tag className="ant-tag-2">
                  <span className="ant-delete"></span>
                  <DeleteOutlined className="anticon-delete" />
                </Tag>
                <Tag closable onClose={log} className="ant-tag-2">
                  <span className="ant-delete"></span>
                  <DeleteOutlined className="anticon-delete" />
                </Tag>
                <Tag closable onClose={log} className="ant-tag-add">
                  <PlusOutlined className="anticon-plus" />
                </Tag>
              </div>
              <Form
                name="basic"
                initialValues={{
                  remember: true,
                }}
                onFinish={handleSubmit}
              >
                <div className="form-row">
                  <Form.Item
                    label="REGISTRATION TYPE"
                    name="regType"
                    className="mr"
                  >
                    <Select>
                      <Select.Option value="Pedigree">Pedigree</Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item label="DOG NAME" name="dogName">
                    <Input />
                  </Form.Item>
                </div>
                <div className="form-row">
                  <Form.Item label="BREED" className="mr" name="breed">
                    <Select>
                      <Select.Option value="German Shefered">
                        German Shefered
                      </Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item label="GENDER" name="gender">
                    <Select>
                      <Select.Option value="male">MALE</Select.Option>
                      <Select.Option value="female">FEMALE</Select.Option>
                    </Select>
                  </Form.Item>
                </div>
                <div className="form-row">
                  <Form.Item
                    label="DATE OF BIRTH"
                    className="mr"
                    name="birthdate"
                  >
                    <DatePicker />
                  </Form.Item>
                  <Form.Item>
                    <Button
                      type="primary"
                      htmlType="submit"
                      className="btn-register"
                    >
                      Digitally Sign Registration
                    </Button>
                  </Form.Item>
                </div>

                <Form.Item
                  name="dragger"
                  valuePropName="fileList"
                  getValueFromEvent={normFile}
                  noStyle
                >
                  <div className="upload-content">
                    <Upload.Dragger name="files" action="/upload.do">
                      <p className="ant-upload-drag-icon">
                        <CloudUploadOutlined />
                      </p>

                      <p className="ant-upload-hint">
                        {" "}
                        Drag and drop photo here or just click for{" "}
                        <span>browser</span> files
                      </p>
                    </Upload.Dragger>
                  </div>
                </Form.Item>
              </Form>
            </TabPane>
            <TabPane key="2">
              <Form
                name="basic"
                className="payment-form"
                initialValues={{
                  remember: true,
                }}
                onFinish={handleSubmit}
              >
                <Radio.Group>
                  <Radio value={1}>Credit Card</Radio>
                  <Radio value={2}>Paypal</Radio>
                </Radio.Group>
                <div className="form-row">
                  <Form.Item label="CARD HOLDER NAME" className="mr">
                    <Select>
                      <Select.Option value="Pedigree">Pedigree</Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item label="DOG NAME">
                    <Input />
                  </Form.Item>
                </div>
                <div className="form-row">
                  <Form.Item label="EXPIRY DATE" className="mr select-date">
                    <Select>
                      <Select.Option value="Pedigree">
                        23 March, 2008
                      </Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    label="EXPIRY DATE"
                    className="mr date-select select-date"
                  >
                    <Select>
                      <Select.Option value="Pedigree">
                        23 March, 2008
                      </Select.Option>
                    </Select>
                  </Form.Item>

                  <Form.Item label="ENTER CVV COD" className="select-date">
                    <Input placeholder="CVV" />
                  </Form.Item>
                </div>
              </Form>
            </TabPane>
          </Tabs>
        
        <hr></hr>
        <div className="register-footer">
          <div className="register-footer-label">
            <label>Registration Free</label>
            <h2>200 USD</h2>
          </div>

          <Button type="primary" htmlType="submit" className="btn-register">
            Pay Now
          </Button>
        </div>
      </div>
    </React.Fragment>
  );
};

export default RegisterDog;
