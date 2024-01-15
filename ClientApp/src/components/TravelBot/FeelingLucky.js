import React, { useEffect } from 'react';
import useFetch from '../useFetch';
import { Button, Spinner } from 'reactstrap';
import { Stars } from 'react-bootstrap-icons';
import { useNavigate } from 'react-router-dom';

const FeelingLucky = () => {
  const navigate = useNavigate();
  const { data, loading, triggerFetch } = useFetch('api/countries/random', false);

  const handleOnLucky = () => {
    triggerFetch();
  };

  useEffect(() => {
    if (data) {
      navigate(`/summary/${data.name}`);
    }
  }, [data]);

  return (
    <Button className='btn btn-primary text-nowrap' disabled={loading} onClick={handleOnLucky}>
      {loading ? <Spinner color='light' size='sm' /> : <Stars />} I'm Feeling Lucky
    </Button>
  );
};

export default FeelingLucky;
