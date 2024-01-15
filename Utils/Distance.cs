using System;

namespace KAHA.TravelBot.NETCoreReactApp.Services
{
  public class Distance
  {
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
      // If the coordinates are the same, return 0
      if ((lat1 == lat2) && (lon1 == lon2))
      {
        return 0;
      }
      else
      {
        // Convert latitude and longitude from degrees to radians
        lat1 = Deg2Rad(lat1);
        lon1 = Deg2Rad(lon1);
        lat2 = Deg2Rad(lat2);
        lon2 = Deg2Rad(lon2);

        // Compute the differences between the coordinates
        double dlon = lon2 - lon1;
        double dlat = lat2 - lat1;

        // Apply the Haversine formula
        double a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2);
        double c = 2 * Math.Asin(Math.Sqrt(a));

        // Convert the distance from radians to kilometers
        double distanceInKilometers = Rad2Deg(c) * 60 * 1.1515 * 1.609344;

        return distanceInKilometers;
      }
    }

    // Converts degrees to radians
    private static double Deg2Rad(double deg)
    {
      return deg * Math.PI / 180;
    }

    // Converts radians to degrees
    private static double Rad2Deg(double rad)
    {
      return rad * 180 / Math.PI;
    }
  }
}
