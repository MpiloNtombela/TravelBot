import { useState, useEffect, useContext, useCallback } from 'react';
import { GlobalAlertContext } from './GlobalAlert';

const useFetch = (url, autoFetch = true) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { setAlert } = useContext(GlobalAlertContext);

  const fetchData = useCallback(() => {
    setLoading(true);
    setError(null);

    fetch(url)
      .then((response) => {
        if (response.ok) {
          return response.json();
        }
        throw new Error(response.statusText);
      })
      .then((data) => {
        setData(data);
      })
      .catch((error) => {
        setData(null);
        setError(error);
        setAlert({
          title: 'Error',
          message: error.message,
          type: 'danger',
        });
      })
      .finally(() => {
        setLoading(false);
      });
  }, [url, setAlert]);

  useEffect(() => {
    if (autoFetch) {
      fetchData();
    }
  }, [autoFetch, fetchData, url]);

  const triggerFetch = () => {
    fetchData();
  };

  const refetch = () => {
    fetchData();
  };

  return { data, loading, error, refetch, triggerFetch };
};

export default useFetch;
