import React from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';
import GlobalAlertProvider from './components/GlobalAlert';
import AlertToast from './components/Alert';

export const App = () => {
  return (
    <GlobalAlertProvider>
      <Layout>
        <AlertToast />
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
      </Layout>
    </GlobalAlertProvider>
  );
};
