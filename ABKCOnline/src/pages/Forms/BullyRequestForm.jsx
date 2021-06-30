import React, { PureComponent } from 'react';
// import moment from 'moment';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import {
  Form,
  Input,
  Button,
  Card,
  LocaleProvider,
  Col,
  Spin,
  Row,
  Divider,
  Anchor
} from 'antd';
import enGB from 'antd/lib/locale-provider/en_GB';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import GridContent from '@/components/PageHeaderWrapper/GridContent';
import { ChartCard } from '@/components/Charts';
import styles from './style.less';

const FormItem = Form.Item;
const { Link } = Anchor;

@connect(({ loading, list, form }) => ({
  list,
  breedsList: form.breeds,
  status: form.status,
  statusStart: form.statusStart,
  colorList: form.colors,
  registration: form.registration,
  startRegistration: form.startRegistration,
  startBullyRequest: form.startBullyRequest,
  startTransferRegistration: form.startTransferRegistration,
  originalTableId: form.originalTableId,
  damNameList: form.damNameList,
  sireNameList: form.sireNameList,
  searchDamName: form.searchDamName,
  ownerName: form.ownerName,
  searchOwnerName: form.searchOwnerName,
  ownerDetails: form.ownerDetails,
  spinner: form.spinner,
  submitting: loading.effects['form/submitRegularForm']
}))
@Form.create()
class BullyRequestForm extends PureComponent {
  state = {
    buttonType: 'start',
    showStartSubmission: true,
    currentRegId: '',
    isSearching: false,
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'form/setStatus'
    });
  }

  componentWillReceiveProps(nextProps) {
    if (!nextProps.spinner) {
      this.setState({
        isSearching: false,
      });
    }
  }

  getButtonId = (e, id, regId) => {
    this.setState(
      {
        buttonType: id,
        currentRegId: regId
      },
      () => {
        this.handleSubmitDraft();
      }
    );
  };

  onUploadDocument = (e, id, val) => {
    const { dispatch } = this.props;
    const fileset = e.target.files;
    dispatch({
      type: 'form/submitSupportinngDoc',
      payload: {
        id,
        file: fileset[0],
        documentType: val,
        registrationType: 'bullyId'
      }
    });
  };

  handleSubmitDraft = () => {
    const { dispatch, form } = this.props;
    const { buttonType, currentRegId } = this.state;
    const role = localStorage.getItem('user-role');
    if (buttonType === 'start') {
      form.validateFields(['abkcNumber'], errors => {
        const value = form.getFieldsValue();
        if (!errors) {
          this.setState({
            showStartSubmission: false,
            isSearching: true,
          });
          dispatch({
            type: 'form/startBullyRequest',
            payload: { abkcNumber: value.abkcNumber }
          });
        }
      });
    } else if (buttonType === 'submit') {
      form.validateFields(['abkcNumber'], errors => {
        if (!errors) {
          this.setState({
            showStartSubmission: false,
            isSearching: true
          });
          if (role === 'ABKCOffice' || role === 'Administrators') {
            dispatch({
              type: 'form/submitBullyForm',
              payload: { currentRegId, registrationType: 'bullyId' },
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'bullyId'
              }
            });
          } else if (role === 'Representatives' || role === 'Owners') {
            const regArray = [];
            regArray.push({
              registrationId: currentRegId,
              registrationType: 'bullyId'
            });
            dispatch({
              type: 'form/submitRegistrationPaymentForm',
              payload: { currentRegId, registrationType: 'bullyId' },
              content: {
                currentRegId,
                redirect: true,
                callSubmit: true,
                registrationType: 'bullyId',
                regArray,
                draftApi: 'bullyId'
              }
            });
          }
        }
      });
    }
  };

  render() {
    const { submitting, statusStart, startBullyRequest } = this.props;
    const { showStartSubmission, isSearching, isEditing } = this.state;
    const {
      form: { getFieldDecorator }
    } = this.props;

    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 8 },
        md: { span: 8 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
        md: { span: 16 }
      }
    };

    const draftFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 10 }
      }
    };

    let regId = '';
    if (statusStart && startBullyRequest) {
      regId = startBullyRequest.id;
    }
    return (
      // eslint-disable-next-line react/jsx-filename-extension
      <LocaleProvider locale={enGB}>
        <PageHeaderWrapper
          className={styles.formHeader}
          title={<FormattedMessage id="form.bully.header.text" />}
        >
          <Card bordered={false}>
            <Anchor affix={false}>
              <Link href="/registration-list" title="Back" />
            </Anchor>
            <Form style={{ marginTop: 8 }}>
              <div>
                <Row gutter={16}>
                  <h2>
                    <FormattedMessage id="form.dogInfo.text" />
                  </h2>
                  <Divider />
                </Row>
                <Spin spinning={isSearching}>
                  <Row gutter={24}>
                    <Col lg={12} md={12} sm={24}>
                      <FormItem
                        {...formItemLayout}
                        label={<FormattedMessage id="form.abkcNumber.label" />}
                      >
                        {getFieldDecorator('abkcNumber', {
                          rules: [
                            {
                              required: true,
                              message: formatMessage({
                                id: 'validation.abkcNumber.required'
                              })
                            }
                          ]
                        })(
                          <Input
                            placeholder={formatMessage({
                              id: 'form.abkcNumber.placeholder'
                            })}
                          />
                        )}
                      </FormItem>
                    </Col>
                  </Row>
                </Spin>
                {statusStart && !showStartSubmission && (
                  <React.Fragment>
                    <GridContent className={styles.standardList}>
                      <div className={styles.standardList}>
                        <Row
                          gutter={24}
                          className="detailed-info full-detail-cover"
                          id="detail-content-row"
                          onLoad={event => this.myHandleLoadEvent(event)}
                        >
                          <Col lg={12} md={24}>
                            <Card className={styles.tabsCard}>
                              <div className="dog-info">
                                <Row style={{ "font-weight": "bold" }}>
                                  <FormattedMessage id="form.dogName.label" />
                                  <h4>
                                    {startBullyRequest &&
                                      startBullyRequest.dogInfo &&
                                      startBullyRequest.dogInfo.dogName}
                                  </h4>
                                </Row>
                                <Row style={{ "font-weight": "bold" }}>
                                  <FormattedMessage id="form.breed.label" />
                                  <h4>
                                    {startBullyRequest &&
                                      startBullyRequest.dogInfo &&
                                      startBullyRequest.dogInfo.breed}
                                  </h4>
                                </Row>
                                <Row style={{ "font-weight": "bold" }}>
                                  <FormattedMessage id="menu.list.color.label" />
                                  <h4>
                                    {startBullyRequest &&
                                      startBullyRequest.dogInfo &&
                                      startBullyRequest.dogInfo.color}
                                  </h4>
                                </Row>
                                <Row style={{ "font-weight": "bold" }}>
                                  <FormattedMessage id="menu.list.microchip.label" />
                                  <h4>
                                    {startBullyRequest &&
                                      startBullyRequest.dogInfo &&
                                      startBullyRequest.dogInfo
                                        .microchipNumber}
                                  </h4>
                                </Row>
                              </div>
                            </Card>
                          </Col>
                        </Row>
                      </div>
                    </GridContent>
                    <Row gutter={24} className={styles.UploadContent}>
                      <Col lg={1} md={1} sm={24} />
                      <Col lg={8} md={8} sm={24}>
                        <ChartCard
                          bordered={false}
                          title="Front Photo"
                          footer={
                            <input
                              name="docFile"
                              type="file"
                              onChange={e =>
                                this.onUploadDocument(e, regId, 'frontPhoto')
                              }
                            />
                          }
                          contentHeight={46}
                        />
                      </Col>
                    </Row>
                  </React.Fragment>
                )}
                <Row gutter={16}>
                  {(!statusStart || showStartSubmission) && (
                    <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
                      <Button
                        type="primary"
                        id="start"
                        style={{ marginLeft: 8 }}
                        htmlType="button"
                        loading={submitting}
                        onClick={e => this.getButtonId(e, 'start', regId)}
                      >
                        <FormattedMessage id="form.start.registration" />
                      </Button>
                    </FormItem>
                  )}
                  {statusStart && !showStartSubmission && (
                    <FormItem {...draftFormLayout} style={{ marginTop: 32 }}>
                      <Button
                        type="primary"
                        id="submit"
                        style={{ marginLeft: 8 }}
                        htmlType="button"
                        loading={submitting}
                        onClick={e => this.getButtonId(e, 'submit', regId)}
                      >
                        <FormattedMessage id="form.submit.now" />
                      </Button>
                    </FormItem>
                  )}
                  <Divider />
                </Row>
              </div>
            </Form>
          </Card>
        </PageHeaderWrapper>
      </LocaleProvider>
    );
  }
}

export default BullyRequestForm;