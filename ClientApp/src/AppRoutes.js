import TopFive from './components/TravelBot/TopFive';
import Summary from './components/TravelBot/Summary';

const AppRoutes = [
  {
    index: true,
    element: <TopFive />,
  },

  // path with parameters
  {
    path: '/summary/:countryName',
    element: <Summary />,
  },
];

export default AppRoutes;
