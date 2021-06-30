import React, { Component } from 'react';
import { Button, Modal, Spin } from 'antd';

export interface IProgressDialogProps {
  title: string;
  caption: string;
  isProcessing: boolean;
  showDialog: boolean;
  handleCloseFn: () => void;
}

// export interface IProgressDialogState {
// }

export default class ProgressDialog extends Component<IProgressDialogProps, any> {
  public state: any = {};

  public constructor(props: IProgressDialogProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { title, caption, handleCloseFn, showDialog, isProcessing } = this.props;
    return (
      <Modal
        title={title}
        visible={showDialog}
        centered={true}
        closable={false}
        destroyOnClose={true}
        afterClose={handleCloseFn}
        okButtonProps={{ disabled: true }}
        cancelButtonProps={{ disabled: true }}
        footer={this.getFooter()}
      >
        <p style={{ textAlign: 'center' }}>{caption}</p>
        {isProcessing && (
          <div style={{ textAlign: 'center' }}>
            <Spin size="large" />
          </div>
        )}
      </Modal>
    );
  }
  private getFooter = () => {
    const { handleCloseFn, isProcessing } = this.props;
    const rtn = [null];
    if (!isProcessing) {
      rtn.push(
        <Button key="submit" type="primary" onClick={() => handleCloseFn()}>
          Ok
        </Button>
      );
    } else {
      rtn.push(null);
    }
    return rtn;
  };
}
