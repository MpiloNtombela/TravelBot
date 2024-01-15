import React, { useState, useEffect, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { InputGroup, Input, Button, Container, Spinner } from 'reactstrap';
import useFetch from '../useFetch';
import { Sunrise, Stars, Sunset, CashStack, CarFront, PeopleFill, Geo, ArrowReturnLeft, Search, Map } from 'react-bootstrap-icons';
import './TravelBot.css';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import { Link, useNavigate } from 'react-router-dom';
import L from 'leaflet';

import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';
import FeelingLucky from './FeelingLucky';

// For some reason, leaflet doesn't load the default icon images, so we have to manually specify them
let DefaultIcon = L.icon({
  iconUrl: icon,
  shadowUrl: iconShadow,
  iconAnchor: [12, 41],
});

L.Marker.prototype.options.icon = DefaultIcon;

const StatsCard = ({ title, value, icon }) => {
  return (
    <div className='card'>
      <div className='card-body'>
        <div className='row'>
          <div className='col-12'>
            <div className='icon-cont'>{icon}</div>
          </div>
          <div className='col-12'>
            <div className='d-flex justify-content-center'>
              <div className='text-center'>
                <h4 className='text-capitalize'>{value}</h4>
                <p className='text-muted text-uppercase'>{title}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const Summary = () => {
  const { countryName } = useParams();
  const { data: summary, loading } = useFetch(`api/countries/${countryName}`);
  const navigate = useNavigate();
  const [search, setSearch] = useState(countryName || '');

  const formatter = useMemo(() => new Intl.DateTimeFormat('default', { hour: '2-digit', minute: '2-digit', hourCycle: 'h23' }), []);
  const numberFormatter = useMemo(() => new Intl.NumberFormat('default', { maximumFractionDigits: 2 }), []);

  const handleOnChange = (e) => {
    setSearch(e.target.value);
  };

  const handleOnSubmit = () => {
    if (search.trim() !== '') {
      navigate(`/summary/${search}`);
    }
  };

  useEffect(() => {
    setSearch(countryName);
  }, [countryName]);

  return (
    <div className='position-relative'>
      <div className='hero'>
        <Container>
          <InputGroup className='mt-3'>
            <Input placeholder='search a country' bsSize='lg' disabled={loading} value={search} onChange={handleOnChange} />
            <Button className='btn btn-danger text-nowrap' disabled={loading} onClick={handleOnSubmit}>
              <Search /> Search
            </Button>
          </InputGroup>
          <div className='mt-1 text-end'>
            <FeelingLucky />
          </div>
          <h1 className='mt-1 mb-3 display-2 text-center text-white'>{countryName}</h1>
        </Container>
      </div>

      <Container className='main-content card shadow-lg p-4 position-absolute'>
        <div className='mb-3 d-flex justify-content-between align-items-center'>
          {summary ? (
            <div className='d-flex align-items-center'>
              <img src={summary?.flag} alt='flag' className='img-fluid profile' />
              <div>
                <p className='ms-3 header-text mb-0'>
                  Capital: <strong>{summary?.capital}</strong>
                </p>
                <p className='ms-3 text-muted d-flex gap-1 align-items-center gx-2 text-nowrap'>
                  <PeopleFill /> <strong className=''> {numberFormatter.format(summary?.population)}</strong>
                </p>
              </div>
            </div>
          ) : (
            <div className='d-flex align-items-center'>
              <div className='profile' />
              <div>
                <p className='ms-3 header-text mb-0'>
                  Capital: <strong>...</strong>
                </p>
                <p className='ms-3 text-muted d-flex align-items-center gx-2'>
                  <PeopleFill /> <strong> ...</strong>
                </p>
              </div>
            </div>
          )}
          <div>
            <Link to='/' className='btn btn-outline-danger float-end'>
              <ArrowReturnLeft /> Back
            </Link>
          </div>
        </div>
        <div className='row'>
          <div className='col-md-12'>
            {loading ? (
              <div className='text-center'>
                <Spinner
                  color='danger'
                  style={{
                    height: '3rem',
                    width: '3rem',
                  }}
                >
                  Loading...
                </Spinner>
              </div>
            ) : summary ? (
              <div>
                <div>
                  <p className='text-muted text-end mt-3 small'>Date and time are based on your current location (timezone)</p>
                  <div className='row g-2'>
                    <div className='col-md-6 h-100'>
                      <StatsCard title='Sunrise' value={formatter.format(new Date(summary.sunrise))} icon={<Sunrise size={70} />} />
                    </div>
                    <div className='col-md-6 h-100'>
                      <StatsCard title='Sunset' value={formatter.format(new Date(summary.sunset))} icon={<Sunset size={70} />} />
                    </div>
                  </div>
                </div>
                <div className='row g-2 mt-2'>
                  <div className='col-md-4 h-100'>
                    <StatsCard title='Currency' value={`${summary.currency} (${summary.currencyCode})`} icon={<CashStack size={70} />} />
                  </div>
                  <div className='col-md-4 h-100'>
                    <StatsCard title='Total Languages' value={`${summary.totalLanguages} Languages`} icon={<PeopleFill size={70} />} />
                  </div>
                  <div className='col-md-4 h-100'>
                    <StatsCard title='Drive Side' value={`${summary.driveSide} Side`} icon={<CarFront size={70} />} />
                  </div>
                </div>
                <div className='my-3'>
                  <MapContainer center={[summary.latitude, summary.longitude]} zoom={8} scrollWheelZoom={false} style={{ height: '400px', width: '100%', borderRadius: '.5rem' }}>
                    <TileLayer
                      attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                      url='https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png'
                    />
                    <Marker position={[summary.latitude, summary.longitude]}>
                      <Popup>
                        <b>{summary.name}</b>
                        <p>
                          <Geo /> Latitude: {summary.latitude}
                        </p>
                        <p>
                          <Geo /> Longitude: {summary.longitude}
                        </p>
                      </Popup>
                    </Marker>
                  </MapContainer>
                </div>
                <StatsCard title='Distance from KAHA office' value={`${numberFormatter.format(summary.distance)} km`} icon={<Map size={70} />} />
              </div>
            ) : (
              <div className='text-center'>
                <h4>No data found for {countryName}</h4>
              </div>
            )}
          </div>
        </div>
      </Container>
    </div>
  );
};

export default Summary;
