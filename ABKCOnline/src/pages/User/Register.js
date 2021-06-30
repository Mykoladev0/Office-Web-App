import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import { Form, Input, Button, Select, Icon } from 'antd';
import styles from './Register.less';

const FormItem = Form.Item;
const { Option } = Select;

@connect(({ register, loading }) => ({
  register,
  submitting: loading.effects['register/submit'],
}))

@Form.create()
class Register extends Component {
  state = {
    confirmDirty: false,
    visible: false,
    RoleRequested: 'Select role'
  };

  componentWillUnmount() {
    clearInterval(this.interval);
  }

  onGetCaptcha = () => {
    let count = 59;
    // this.setState({ count });
    this.interval = setInterval(() => {
      count -= 1;
      // this.setState({ count });
      if (count === 0) {
        clearInterval(this.interval);
      }
    }, 1000);
  };

  getPasswordStatus = () => {
    const { form } = this.props;
    const value = form.getFieldValue('password');
    if (value && value.length > 9) {
      return 'ok';
    }
    if (value && value.length > 5) {
      return 'pass';
    }
    return 'poor';
  };

  handleSubmit = e => {
    e.preventDefault();
    const { form, dispatch } = this.props;
    form.validateFields({ force: true }, (err, values) => {
      if (!err) {
        const { RoleRequested } = this.state;
        dispatch({
          type: 'register/submitSignUp',
          payload: {
            ...values,
            RoleRequested,
          },
        });
      }
    });
  };

  handleConfirmBlur = e => {
    const { value } = e.target;
    const { confirmDirty } = this.state;
    this.setState({ confirmDirty: confirmDirty || !!value });
  };

  checkConfirm = (rule, value, callback) => {
    const { form } = this.props;
    if (value && value !== form.getFieldValue('password')) {
      callback(formatMessage({ id: 'validation.password.twice' }));
    } else {
      callback();
    }
  };

  checkPassword = (rule, value, callback) => {
    const { visible, confirmDirty } = this.state;
    if (!value) {
      this.setState({
        // help: formatMessage({ id: 'validation.password.required' }),
        visible: !!value,
      });
      callback('error');
    } else {
      this.setState({
        // help: '',
      });
      if (!visible) {
        this.setState({
          visible: !!value,
        });
      }
      if (value.length < 6) {
        callback('error');
      } else {
        const { form } = this.props;
        if (value && confirmDirty) {
          form.validateFields(['confirm'], { force: true });
        }
        callback();
      }
    }
  };

  changePrefix = () => {

  };

  changeValue = value => {
    this.setState({
      RoleRequested: value,
    });
  };

  render() {
    const { form, submitting, register } = this.props;
    const { message, status } = register;
    const { getFieldDecorator } = form;
    return (
      <div className={styles.main}>
        <div className={styles.registerFormControl}>
          <div className={styles.regBrandCon}>
            <img src={require("../../assets/img/abkc_logo.png")} alt="logo" />
          </div>
          <div className={styles.textCenter}>
            <h1 className={styles.titleRegister}><FormattedMessage id="app.register.register" /></h1>
          </div>
          {message && !status && <p className={styles.errorMessage}>{message}</p>}
          {message && status && <p className={styles.successMessage}>{message}</p>}

          <Form onSubmit={this.handleSubmit}>
            {/* <FormItem label={<FormattedMessage id="form.title.label" />}>
              {getFieldDecorator('title', {
                rules: [
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.title.required' }),
                  },
                ],
              })(<Input placeholder={formatMessage({ id: 'form.title.placeholder' })} />)}
            </FormItem> */}
            <FormItem>
              {getFieldDecorator('EmailAddress', {
                rules: [
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.email.required' }),
                  },
                  {
                    type: 'email',
                    message: formatMessage({ id: 'validation.email.wrong-format' }),
                  },
                ],
              })(
                <Input prefix={<Icon type="mail" />} size="large" placeholder={formatMessage({ id: 'form.mail.placeholder' })} />
              )}
            </FormItem>
            <FormItem>
              {getFieldDecorator('firstName', {
                rules: [
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.firstName.required' }),
                  }
                ],
              })
                (<Input prefix={<Icon type="user" />} size="large" placeholder={formatMessage({ id: 'form.firstName.placeholder' })} />)}
            </FormItem>
            <FormItem>
              {getFieldDecorator('lastName', {
                rules: [
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.lastName.required' }),
                  }
                ],
              })
                (<Input prefix={<Icon type="user" />} size="large" placeholder={formatMessage({ id: 'form.lastName.placeholder' })} />)}
            </FormItem>
            <Select
              size="large"
              placeholder={formatMessage({ id: 'form.role.placeholder' })}
              onChange={this.changeValue}
              style={{ width: '100%' }}
            >
              <Option value="owner">Owner</Option>
              <Option value="representative">Representative</Option>
            </Select>
            <FormItem style={{ paddingTop: "30px" }}>
              <Button
                size="large"
                loading={submitting}
                className={styles.submit}
                type="primary"
                htmlType="submit"
              >
                <FormattedMessage id="app.register.register" />
              </Button>

            </FormItem>
            <Link className={styles.login} to="/User/Login">
              <FormattedMessage id="app.register.sign-in" />
            </Link>
          </Form>
        </div>
      </div>
    );
  }
}

export default Register;