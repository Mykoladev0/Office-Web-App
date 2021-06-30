import React, { PureComponent } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import locale from 'antd/lib/date-picker/locale/en_US';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import GridContent from '@/components/PageHeaderWrapper/GridContent';
import ReactTooltip from 'react-tooltip';
import {
  List,
  Card,
  Row,
  Col,
  Input,
  Icon,
  Button,
  Modal,
  Avatar,
  Form,
  DatePicker,
  Select,
  Carousel,
  Divider,
  Tabs,
} from 'antd';
import { formatMessage, FormattedMessage } from 'umi/locale';
import styles from './PendingRegistrationList.less';

// moment.locale('en');
const { TabPane } = Tabs;
const { Option, OptGroup } = Select;
const { Search } = Input;
const { RangePicker } = DatePicker;

@connect(({ list, loading }) =>
  ({
    list,
    loading: loading.models.list,
  })
)
@Form.create()
class PendingRegistrationList extends PureComponent {
  constructor(props) {
    super(props);
    this.myHandleImageLoadEvent = this.myHandleImageLoadEvent.bind(this);
  }

  state = {
    conentWrapperMinHeight: '',
    sidePanelDatas: [],
    filterVal: "All",
    searchVal: 'Owner',
    currentRepId: '',
    pendingRegistration: 0,
    supportingDocType: '',
    viewMore: false,
    contentMinHeight: '',
    showRejection: false,
    showRejectError: false,
    showApproval: false,
    showRequestInfo: false,
    showRequestError: false,
    visible: false,
    updated: true,
    isDocSearching: false,
    show: false
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/fetch',
      payload: {
        count: 5,
      },
    });
    dispatch({
      type: 'list/fetchRepData'
    });
    this.setState({
      show: !this.state.show
    })
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.list && nextProps.list.docsStatus && (!nextProps.spinner)) {
      this.setState({ isDocSearching: false })
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { viewMore, updated } = this.state;
    const { dispatch, list } = this.props;
    const role = localStorage.getItem('user-role')
    if (updated && list.currentUsers && list.currentUsers.id) {
      // eslint-disable-next-line react/no-did-update-set-state
      this.setState({
        viewMore: false,
        updated: false,
      })
      if (role === 'ABKCOffice' || role === 'Administrators') {
        dispatch({
          type: 'list/filterPending'
        });
      }
      else if (role === 'Representatives' || role === 'Owners') {
        dispatch({
          type: 'list/filterPendingById',
          payload: list.currentUsers.id,
        });
      }
    }
  }

  displayPanelData = (e, data) => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/fetchSingleData',
      payload: data.id,
      regType: data.registrationType
    });
  }

  handleChangeFilter = (value) => {
    this.setState({
      filterVal: value
    })
    const { dispatch } = this.props;
    dispatch({
      type: 'list/filterData',
      payload: value,
    });
  }

  selectSearchField = (value) => {
    this.setState({
      searchVal: value
    })
  }

  selectSupportingDocType = (value, id) => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/supportingDocs',
      payload: { val: value, id, regType: 'pedigree' }
    });
    this.setState({
      supportingDocType: value,
      isDocSearching: true
    })
  }

  searchData = (val) => {
    const { dispatch } = this.props;
    const data = val;
    const pendingVal = true;
    const { searchVal } = this.state
    if ((data.length === 0 || data.length > 2) && searchVal === "Owner") {
      dispatch({
        type: 'list/searchByOwner',
        payload: { val: data, pending: pendingVal }
      });
    }
    if ((data.length === 0 || data.length > 2) && searchVal === "Dog") {
      dispatch({
        type: 'list/searchByDog',
        payload: { val: data, pending: pendingVal }
      });
    }
    if ((data.length === 0 || data.length > 2) && searchVal === "SubmittedBy") {
      dispatch({
        type: 'list/searchBySubmitted',
        payload: { val: data, pending: pendingVal }
      });
    }
  }

  selectRepresentative = (e, item) => {
    const { dispatch } = this.props;
    const pendingVal = true;
    this.setState({
      currentRepId: item.id,
      pendingRegistration: item.pendingRegistrationCount
    })
    dispatch({
      type: 'list/filterByRep',
      payload: { val: item.id, pending: pendingVal }
    });
  }

  approveAllByRep = () => {
    const { dispatch } = this.props;
    const { currentRepId } = this.state;
    dispatch({
      type: 'list/approveAllByRep',
      payload: { val: currentRepId }
    });
  }

  changeViewMoreState = () => {
    this.setState(prevState => ({
      viewMore: !prevState.viewMore,
    }), () => this.myHandleLoadEvent());
  }

  changeViewMoreStateBack = () => {
    const { dispatch, list } = this.props;
    const role = localStorage.getItem('user-role')
    if (role === 'ABKCOffice' || role === 'Administrators') {
      dispatch({
        type: 'list/filterPending'
      });
    }
    else if (role === 'Representatives' || role === 'Owners') {
      dispatch({
        type: 'list/filterPendingById',
        payload: list.currentUsers.id,
      });
    }
    this.setState(prevState => ({
      viewMore: !prevState.viewMore,
    }));

  }

  onRejectRegistration = () => {
    this.setState(prevState => ({
      showRejection: !prevState.showRejection,
      showApproval: false,
      showRequestInfo: false,
      showRequestError: false
    }));
  }

  onApproveRegistration = () => {
    this.setState(prevState => ({
      showApproval: !prevState.showApproval,
      showRequestInfo: false,
      showRejection: false,
      showRejectError: false,
      showRequestError: false
    }));
  }

  requestRegistrationInfo = () => {
    this.setState(prevState => ({
      showRequestInfo: !prevState.showRequestInfo,
      showApproval: false,
      showRejection: false,
      showRejectError: false
    }));
  }

  rejectReason = (value, id) => {
    const { dispatch } = this.props;
    if (value === '') {
      this.setState({
        showRejectError: true,
        showRejection: true
      })
    }
    else {
      this.setState({
        showRejection: false,
        showRejectError: false,
        updated: true,
      })
      dispatch({
        type: 'list/rejectRegistration',
        payload: { val: value, id, type: 'pedigree' }
      });
    }
  }

  approveReason = (value, id) => {
    const { dispatch } = this.props;
    this.setState({
      showApproval: false,
      updated: true,
    })
    dispatch({
      type: 'list/approveRegistration',
      payload: { val: value, id, type: 'pedigree' }
    });
  }

  requestInfo = (value, id) => {
    const { dispatch } = this.props;
    if (value === '') {
      this.setState({
        showRequestError: true,
        showRequestInfo: true
      })
    }
    else {
      this.setState({
        showRequestInfo: false,
        showRequestError: false,
        updated: true,
      })
      dispatch({
        type: 'list/requestRegistrationInfo',
        payload: { val: value, id, type: 'pedigree' }
      });
    }
  }

  filterByDate = (date, dateString) => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/filterByDates',
      payload: { dateString }
    });
  }

  showModal = () => {
    // let newWindow = window.open('/');
    // newWindow.onload = () => {
    //   newWindow.location = url;
    // };
    // // window.open(url, '_blank');
    this.setState({
      visible: true,
    });
  }

  handleCancel = () => {
    this.setState({
      visible: false,
    });
  }

  handleOk = () => {
    this.setState({
      visible: false,
    });
  }

  myHandleLoadEvent = () => {
    const rowHeight = document.getElementById('detail-content-row').clientHeight
    this.setState({ contentMinHeight: rowHeight })
  }

  myHandleImageLoadEvent() {
    const rowHeight = document.getElementById('content-row').clientHeight;
    this.setState({ conentWrapperMinHeight: rowHeight })
  }

  render() {
    const {
      loading,
      list
    } = this.props;
    const listDetails = list;
    const role = localStorage.getItem('user-role');
    let listPendingDatas = [];
    if (role === 'ABKCOffice' || role === 'Administrators') {
      listPendingDatas = listDetails.listData
    }
    else {
      listPendingDatas = listDetails.listPendingData
    }
    let { sidePanelDatas } = this.state;
    const { supportingDocType, filterVal, viewMore, pendingRegistration, conentWrapperMinHeight, showRejection, contentMinHeight, showRejectError, showApproval, showRequestInfo, showRequestError, visible } = this.state;
    if (listDetails.sidePanelDetails.length) {
      if (role === 'ABKCOffice' || role === 'Administrators') {
        // eslint-disable-next-line 
        sidePanelDatas = listDetails.sidePanelDetails[0]
      }
      else {
        // eslint-disable-next-line 
        sidePanelDatas = listDetails.listPendingData && listDetails.listPendingData[0]
      }
    }
    else {
      sidePanelDatas = ''
    }

    const extraContent = (
      <div>
        <div className="search-cover">
          <Row>
            <Col sm={8} xs={24}>
              <Select defaultValue={formatMessage({ id: 'menu.list.owner' })} style={{ width: 120 }} onChange={this.selectSearchField}>
                <Option value="Owner"><FormattedMessage id="menu.list.owner" /> </Option>
                <Option value="Dog"><FormattedMessage id="menu.list.dog" /> </Option>
                <Option value="SubmittedBy"><FormattedMessage id="menu.list.submittedBy" /> </Option>
              </Select>
            </Col>

            <Col sm={16} xs={24}>
              <Search className={styles.extraContentSearch} placeholder={formatMessage({ id: 'menu.list.search.placeholder' })} enterButton onSearch={value => this.searchData(value)} />
            </Col>
          </Row>
        </div>
        <div className="filter-cover">
          <Row>
            <Col sm={3} xs={24}>
              <span className="label-text" id="filter"><FormattedMessage id="menu.list.filterBy" />{' '}</span>
            </Col>
            <Col sm={12} xs={24}>
              <Select
                defaultValue={formatMessage({ id: 'menu.list.filterAll' })}
                style={{ width: 200 }}
                showSearch
                onChange={this.handleChangeFilter}
              >
                <OptGroup label={formatMessage({ id: 'menu.list.filter.registrations.label' })}>
                  <Option value="All"><FormattedMessage id="menu.list.filterAll" /></Option>
                </OptGroup>
                <OptGroup label={formatMessage({ id: 'menu.list.filter.priority.label' })}>
                  <Option value="night"><FormattedMessage id="menu.list.filter.night" /></Option>
                </OptGroup>
                {(role === 'ABKCOffice' || role === 'Administrators') && listDetails.representatives.length &&
                  <OptGroup label={formatMessage({ id: 'menu.list.filter.representatives.label' })}>
                    {listDetails.representatives.map(item => (
                      <Option key={item.id} id={item.id} value={item.loginName} onClick={e => this.selectRepresentative(e, item)}><div data-tip={item.loginName}>{item.loginName}  <ReactTooltip /></div></Option>
                    ))}
                  </OptGroup>
                }
              </Select>
            </Col>
            <Col sm={9} xs={24}>
              <RangePicker locale={locale} onChange={this.filterByDate} />
            </Col>
          </Row>
        </div>
      </div>
    );

    const paginationProps = {
      pageSize: 5,
    };

    const ListContent = ({ data: { dogInfo } }) => (
      <div className={styles.listContent}>
        {dogInfo.owner &&
          <div className={styles.listContentItem}>
            <p data-tip={dogInfo.owner.firstName}><Icon type="user" /> {dogInfo.owner.firstName}</p>
            <ReactTooltip />
            <p data-tip={dogInfo.owner.email}><Icon type="mail" /> {dogInfo.owner.email}</p>
          </div>
        }
      </div>
    );

    return (
      <div>
        {!viewMore ?
          <PageHeaderWrapper className={styles.formHeader} title={<FormattedMessage id="form.view.dog.registration" />}>
            <div className={styles.standardList}>
              <Row id="content-row" onLoad={(event) => this.myHandleImageLoadEvent(event)} ref={(input) => { this.myElement = input; }}>
                <Col sm={sidePanelDatas ? 16 : 24} xs={24} id="content-left-col">
                  <Card
                    className={`${styles.listCard} all-reg-content`}
                    bordered={false}
                    style={{ marginTop: 24, borderRight: '12px solid #f0f2f6', minHeight: conentWrapperMinHeight }}
                    bodyStyle={{ padding: '0 32px 40px 32px' }}
                    extra={extraContent}
                  >
                    <div className="item-header">
                      {/* eslint-disable-next-line  */}
                      {(filterVal === "All" || filterVal === "night") ? '' : (pendingRegistration > 0) ?
                        <Button type="primary" className="reg-approve-btn" onClick={this.approveAllByRep}><FormattedMessage id="menu.list.approve.all" /></Button> : <Button type="primary" className="reg-approve-btn"><FormattedMessage id="menu.list.approve.all" /></Button>}
                    </div>
                    <List
                      className="list-content"
                      size="large"
                      rowKey="id"
                      // loading={loading}
                      pagination={paginationProps}
                      dataSource={listPendingDatas}
                      renderItem={item => (
                        <List.Item className={item.submittedBy && item.submittedBy.roles[0] && item.submittedBy.roles[0].name !== "Owner" ? "item-by-rep" : "item-by-owner"} id={(item.overnightRequested && item.rushRequested) ? "immediate" : item.overnightRequested ? 'overnight' : item.rushRequested ? 'rushnight' : ''}>
                          <List.Item.Meta
                            avatar={<div><Avatar src={item.registrationThumbnailBase64} shape="square" size="large" /></div>}
                            title={<a href="#" onClick={e => this.displayPanelData(e, item)}>{item.dogInfo.dogName}</a>}
                            description={<div><span>{item.dogInfo.gender} . {item.dogInfo.breed}</span><p className={styles.regType}><FormattedMessage id="menu.list.label.dob" /><span>: {moment(`${item.dogInfo.dateOfBirth}`).format('MMM-Do YYYY')}</span></p><p className={styles.regType}><FormattedMessage id="menu.list.label.submittedOn" /><span>: {item.dateSubmitted && moment(`${item.dateSubmitted}`).format('MMM-Do YYYY')}</span></p><p className={styles.regType}><FormattedMessage id="menu.list.label.type" />: <span className={`span-type ${item.registrationType}-type`}>{item.registrationType}</span></p></div>}
                          />
                          <ListContent data={item} />
                          <span className="registerd-person"><FormattedMessage id="menu.list.label.registeredBy" /> {item.submittedBy && item.submittedBy.roles[0] && item.submittedBy.roles[0].name}</span>
                        </List.Item>
                      )}
                    />
                  </Card>
                </Col>

                <Col sm={8} xs={24} id="content-right-col">
                  {sidePanelDatas &&
                    <Card
                      className={`${styles.listCard} detail-info-cover`}
                      bordered={false}
                      style={{ marginTop: 24, minHeight: conentWrapperMinHeight }}
                      bodyStyle={{ padding: '0 20px 40px 20px' }}
                    >
                      <div>
                        <div className="dog-info"><h4>{sidePanelDatas.dogInfo.dogName}</h4><span>{sidePanelDatas.dogInfo.gender} . {sidePanelDatas.dogInfo.breed}</span><p className={styles.regType}><FormattedMessage id="menu.list.label.dob" /><span>: {moment(`${sidePanelDatas.dogInfo.dateOfBirth}`).format('MMM-Do YYYY')}</span></p><p className={styles.regType}><FormattedMessage id="menu.list.label.type" /><span>: {sidePanelDatas.registrationType}</span></p></div>
                        {sidePanelDatas.registrationThumbnailBase64 &&
                          <Carousel autoplay>
                            <div>
                              <img
                                src={sidePanelDatas && sidePanelDatas.registrationThumbnailBase64}
                                style={{
                                  height: '140px', width: '100%'
                                }}
                                alt=""
                              />
                            </div>
                          </Carousel>
                        }
                        <div className={styles.docContainerWrapper}>
                          <Select
                            onChange={(value) => this.selectSupportingDocType(value, sidePanelDatas.id)}
                            showSearch
                            defaultValue={supportingDocType ? supportingDocType : <FormattedMessage id="menu.list.documents.placeholder" />}
                            style={{ width: 200, marginBottom: 10 }}
                            loading={this.state.isDocSearching}
                          >
                            <Option value="frontPedigree"><FormattedMessage id="menu.list.documents.front.pedigree.label" /> </Option>
                            <Option value="backPedigree"><FormattedMessage id="menu.list.documents.back.pedigree.label" /> </Option>
                            <Option value="frontPhoto"><FormattedMessage id="menu.list.documents.front.photo.label" /> </Option>
                            <Option value="sidePhoto"><FormattedMessage id="menu.list.documents.side.photo.label" /> </Option>
                            <Option value="ownerSignature"><FormattedMessage id="menu.list.documents.owner.sign.label" /> </Option>
                            <Option value="coOwnerSignature"><FormattedMessage id="menu.list.documents.coowner.sign.label" /> </Option>
                          </Select>

                          {supportingDocType && listDetails.docsStatus &&

                            <div className="btn-document-view">
                              {listDetails.docsData.fileType !== "application/pdf" &&
                                <Button onClick={() => this.showModal()}>
                                  <FormattedMessage id="menu.list.btn.view.file" />
                                </Button>
                              }

                              <Modal
                                title={supportingDocType}
                                visible={visible}
                                onOk={this.handleOk}
                                onCancel={this.handleCancel}
                                footer={[
                                  <Button key="submit" type="primary" loading={loading} onClick={this.handleOk}>
                                    <FormattedMessage id="menu.btn.ok" />
                                  </Button>,
                                ]}
                              >
                                <div>
                                  <img id="photo" alt="img" src={listDetails.docsData.fileUrl} style={{ 'width': '100%' }} />
                                </div>
                              </Modal>
                              <div className={styles.tags}>
                                <Button>
                                  <a href={listDetails.docsData.fileUrl} download={`${listDetails.docsData.fileName}`}><FormattedMessage id="menu.list.btn.download.file" /></a>
                                </Button>
                              </div>
                            </div>
                          }
                        </div>

                        <div className="detail-cover">
                          <div className="detail-row">
                            <div className="detail-col">
                              <FormattedMessage id="menu.list.color.label" />
                              <p>{sidePanelDatas.dogInfo.color}</p>
                            </div>
                            <div className="detail-col">
                              <FormattedMessage id="menu.list.microchip.label" />
                              <p>{sidePanelDatas.dogInfo.microchipNumber}</p>
                            </div>
                          </div>
                          <div className="detail-row">
                            <div className="detail-col">
                              <FormattedMessage id="menu.list.sire.label" />
                              <p>{sidePanelDatas.dogInfo.sire}</p>
                            </div>
                            <div className="detail-col">
                              <FormattedMessage id="menu.list.dam.label" />
                              <p>{sidePanelDatas.dogInfo.dam}</p>
                            </div>
                          </div>
                        </div>
                        <div className={styles.listContentItem}>
                          <p><Icon type="user" /> {sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.firstName}</p>
                          <p><Icon type="mail" /> {sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.email}</p>
                        </div>
                        <Button type="primary" onClick={this.changeViewMoreState}><FormattedMessage id="menu.list.btn.view.more" /></Button>
                      </div>
                    </Card>}
                </Col>
              </Row>
            </div>
          </PageHeaderWrapper> :
          <div className="inner-detail-page">
            <Row>
              <Col sm={24} xs={24}>
                <Card>
                  <Button type="default" onClick={this.changeViewMoreStateBack}><FormattedMessage id="menu.list.label.back" /></Button>
                </Card>
              </Col>
            </Row>
            <GridContent className={styles.standardList}>
              <div className={styles.standardList}>
                <Row gutter={24} className="detailed-info full-detail-cover"
                  id="detail-content-row" onLoad={(event) => this.myHandleLoadEvent(event)}>
                  <Col lg={12} md={24}>
                    <Card
                      className={styles.tabsCard}
                      style={{ minHeight: contentMinHeight }}
                    >
                      <div className="dog-info">
                        <h4>{sidePanelDatas.dogInfo.dogName}</h4>
                        <h6> {sidePanelDatas.dogInfo.gender} . {sidePanelDatas.dogInfo.breed}</h6>
                        <Row>
                          <Col sm={8} xs={24}>
                            <p className="info"><FormattedMessage id="menu.list.label.dob" /> {moment(`${sidePanelDatas.dogInfo.dateOfBirth}`).format('MMM-Do YYYY')}</p>
                            <p className="info"><FormattedMessage id="menu.list.color.label" />{sidePanelDatas.dogInfo.color}</p>
                          </Col>
                          <Col sm={8} xs={24}>
                            <p className="info"><FormattedMessage id="menu.list.sire.label" />{sidePanelDatas.dogInfo.sire}</p>
                            <p className="info"><FormattedMessage id="menu.list.dam.label" />{sidePanelDatas.dogInfo.dam}</p>
                          </Col>
                          <Col sm={8} xs={24}>
                            <p className="info"><span><FormattedMessage id="menu.list.microchip.label" /></span>{sidePanelDatas.dogInfo.microchipNumber}</p>
                            <p className="info"><span><FormattedMessage id="menu.list.status.label" /> </span>{sidePanelDatas.registrationStatus}</p>
                          </Col>
                        </Row>
                      </div>

                      <Divider dashed />
                      <div className={styles.containerWrapper}>
                        <Select
                          onChange={(value) => this.selectSupportingDocType(value, sidePanelDatas.id)}
                          showSearch
                          style={{ width: 200, marginBottom: 10 }}
                          defaultValue={supportingDocType ? supportingDocType : <FormattedMessage id="menu.list.documents.placeholder" />}
                          loading={this.state.isDocSearching}
                        >
                          <Option value="frontPedigree"><FormattedMessage id="menu.list.documents.front.pedigree.label" /> </Option>
                          <Option value="backPedigree"><FormattedMessage id="menu.list.documents.back.pedigree.label" /> </Option>
                          <Option value="frontPhoto"><FormattedMessage id="menu.list.documents.front.photo.label" /> </Option>
                          <Option value="sidePhoto"><FormattedMessage id="menu.list.documents.side.photo.label" /> </Option>
                          <Option value="ownerSignature"><FormattedMessage id="menu.list.documents.owner.sign.label" /> </Option>
                          <Option value="coOwnerSignature"><FormattedMessage id="menu.list.documents.coowner.sign.label" /> </Option>
                        </Select>

                        {supportingDocType && listDetails.docsStatus &&
                          <div className="btn-document-view">
                            {listDetails.docsData.fileType !== "application/pdf" &&
                              <Button onClick={this.showModal}>
                                <FormattedMessage id="menu.list.btn.view.file" />
                              </Button>
                            }

                            <Modal
                              title={supportingDocType}
                              visible={visible}
                              onOk={this.handleOk}
                              onCancel={this.handleCancel}
                              footer={[
                                <Button key="submit" type="primary" loading={loading} onClick={this.handleOk}>
                                  <FormattedMessage id="menu.btn.ok" />
                                </Button>,
                              ]}
                            >
                              <div>
                                <img id="photo" alt="img" src={listDetails.docsData.fileUrl} style={{ 'width': '100%' }} />
                              </div>
                            </Modal>


                            <div className={styles.tags}>
                              <Button>
                                <a href={listDetails.docsData.fileUrl} download={`${listDetails.docsData.fileName}`}><FormattedMessage id="menu.list.btn.download.file" /></a>
                              </Button>
                            </div>
                          </div>
                        }
                      </div>
                      <Divider dashed />
                      {(role === 'ABKCOffice' || role === 'Administrators') && sidePanelDatas.registrationStatus === "Pending" ?
                        <div className="btn-actions">
                          <Button type="primary" onClick={this.onApproveRegistration}><FormattedMessage id="menu.table.approve" /></Button>
                          <Button type="primary" onClick={this.onRejectRegistration}><FormattedMessage id="menu.table.reject" /></Button>
                          <Button type="primary" onClick={this.requestRegistrationInfo}><FormattedMessage id="menu.list.request.info" /></Button>
                        </div> : ''}
                      <div>
                        {showRejection && <Search
                          placeholder="Reason for rejection"
                          enterButton="Submit"
                          size="default"
                          onSearch={(value) => this.rejectReason(value, sidePanelDatas.id)}
                          style={{ marginTop: 10 }}
                        />}
                        {showRejectError && <p className="show-error"><FormattedMessage id="menu.list.reject.reason" /></p>}
                      </div>
                      <div>
                        {showApproval && <Search
                          placeholder="Reason for approval"
                          enterButton="Submit"
                          size="default"
                          onSearch={(value) => this.approveReason(value, sidePanelDatas.id)}
                          style={{ marginTop: 10 }}
                        />}
                      </div>
                      <div>
                        {showRequestInfo && <Search
                          placeholder="Request more data needed"
                          enterButton="Submit"
                          size="default"
                          onSearch={(value) => this.requestInfo(value, sidePanelDatas.id)}
                          style={{ marginTop: 10 }}
                        />}
                        {showRequestError && <p className="show-error"><FormattedMessage id="menu.list.info.required" /></p>}
                      </div>
                    </Card>
                  </Col>
                  <Col lg={12} xs={24}>
                    <div className="detail-outer-cover">
                      <Card style={{ minHeight: contentMinHeight }}>
                        <Tabs defaultActiveKey="1">
                          <TabPane tab={formatMessage({ id: 'menu.list.owner' })} key="1">
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.firstName.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.firstName}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.lastName.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.lastName}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.address1.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.address1}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}><FormattedMessage id="form.address2.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.address2}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.address3.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.address3}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.city.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.city}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}><FormattedMessage id="form.state.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.state}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.zip.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.zip}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.country.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.country}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}><FormattedMessage id="form.international.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.international ? 'Yes' : ' No'}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.email.placeholder" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.email}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.phone-number.label" />
                                <p>{sidePanelDatas.dogInfo.owner && sidePanelDatas.dogInfo.owner.phone}</p>
                              </Col>
                            </Row>
                          </TabPane>
                          <TabPane tab={formatMessage({ id: 'menu.list.coowner' })} key="2">
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}>  <FormattedMessage id="form.firstName.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.firstName}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.lastName.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.lastName}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.address1.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.address1}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}><FormattedMessage id="form.address2.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.address2}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.address3.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.address3}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.city.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.city}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.state.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.state}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.zip.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.zip}</p>
                              </Col>
                              <Col className="detail-column" span={8}> <FormattedMessage id="form.country.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.country}</p>
                              </Col>
                            </Row>
                            <Row className="detail-row">
                              <Col className="detail-column" span={8}><FormattedMessage id="form.international.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.international ? 'Yes' : 'No'}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.email.placeholder" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.email}</p>
                              </Col>
                              <Col className="detail-column" span={8}><FormattedMessage id="form.phone-number.label" />
                                <p>{sidePanelDatas.dogInfo.coOwner && sidePanelDatas.dogInfo.coOwner.phone}</p>
                              </Col>
                            </Row>
                          </TabPane>
                        </Tabs>
                      </Card>
                    </div>
                  </Col>
                </Row>
              </div>
            </GridContent>
          </div>
        }
      </div>
    );
  }
}

export default PendingRegistrationList;
