import React, { createContext, useState } from 'react';

export const GlobalAlertContext = createContext();

const GlobalAlertProvider = ({ children }) => {
  const [alert, setAlert] = useState(null);

  return <GlobalAlertContext.Provider value={{ alert, setAlert }}>{children}</GlobalAlertContext.Provider>;
};

export const GlobalAlertConsumer = GlobalAlertContext.Consumer;

export default GlobalAlertProvider;
