import { notification } from "antd";

export const showNotification = (title: string, message: string) => {
  notification.info({
    message: title,
    description: message,
    style: {
      width: 400
    }
  });
};
