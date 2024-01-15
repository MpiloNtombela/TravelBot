import React, { useMemo } from 'react';
import { Table, Spinner, Container } from 'reactstrap';
import { BoxArrowUpRight } from 'react-bootstrap-icons';
import useFetch from '../useFetch';
import { useNavigate } from 'react-router-dom';
import './TravelBot.css';

const TopFive = () => {
  const navigate = useNavigate();
  const { data: topFive, loading } = useFetch('api/countries/top5');
  const formater = useMemo(() => new Intl.NumberFormat('default', { maximumFractionDigits: 2, style: 'decimal' }), []);

  const navigateToSummary = (countryName) => {
    navigate(`/summary/${countryName}`);
  };

  return (
    <Container className='mt-3'>
      <div className='top-five shadow p-3 rounded'>
        <h4 className='mb-3'>Top Populated Countries</h4>
        <Table bordered responsive hover>
          <thead>
            <tr>
              <th>Name</th>
              <th>Capital</th>
              <th>Population</th>
              <th>Latitude</th>
              <th>Longitude</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan='5' className='text-center'>
                  <Spinner
                    color='danger'
                    style={{
                      height: '3rem',
                      width: '3rem',
                    }}
                  >
                    Loading...
                  </Spinner>
                </td>
              </tr>
            ) : topFive ? (
              topFive.map((country) => (
                <tr key={country.name} className='text-right cursor-pointer' onClick={() => navigateToSummary(country.name)}>
                  <td>
                    {country.name} <BoxArrowUpRight />
                  </td>
                  <td>{country.capital}</td>
                  <td>{formater.format(country.population)}</td>
                  <td>{country.latitude}</td>
                  <td>{country.longitude}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan='5' className='text-center'>
                  No data found
                </td>
              </tr>
            )}
          </tbody>
          <caption className='text-right'>
            <small>Top 5 populated countries in Southern Hemisphere</small>
          </caption>
        </Table>
      </div>
    </Container>
  );
};

export default TopFive;
