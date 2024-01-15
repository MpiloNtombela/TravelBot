import React, { useEffect, useContext } from 'react';
import { Toast, ToastHeader, ToastBody } from 'reactstrap';
import { GlobalAlertContext } from './GlobalAlert';

const AlertToast = () => {
  const { alert, setAlert } = useContext(GlobalAlertContext);

  useEffect(() => {
    if (alert != null) {
      setTimeout(() => {
        setAlert(null);
      }, 3000);
    }
  }, [alert, setAlert]);

  return (
    <Toast isOpen={alert != null} delay={3000} hidden={alert === null}>
      {alert?.title && (
        <ToastHeader icon={alert?.type}>
          <strong className='mr-auto'>{alert?.title}</strong>
        </ToastHeader>
      )}
      <ToastBody>{alert?.message}</ToastBody>
    </Toast>
  );
};

export default AlertToast;
