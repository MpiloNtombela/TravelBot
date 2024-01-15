using System;
using KAHA.TravelBot.NETCoreReactApp.Models;
using Newtonsoft.Json.Linq;

namespace KAHA.TravelBot.NETCoreReactApp.Services
{
  public class TravelBotService : ITravelBotService
  {

    // HttpClient is intended to be instantiated once and reused throughout the life of an application. (Singleton)
    // Instantiating an HttpClient class for every request will exhaust the number of sockets available under heavy loads.
    private static readonly HttpClient httpClient = new();
    private readonly ILogger<TravelBotService> _logger;

    public TravelBotService(ILogger<TravelBotService> logger)
    {
      _logger = logger;
    }

    // Caching the countries to avoid making unnecessary API calls.
    private static List<CountryModel>? cachedCountries = null; // countryName -> CountryModel

    public async Task<List<CountryModel>> GetAllCountries()
    {
      // If the countries have already been retrieved, return the cached countries.
      if (cachedCountries != null && cachedCountries.Count > 0)
      {
        return cachedCountries;
      }

      /*
        Moved the API URL to a variable for easier readability and maintainability.
        Now only retrieving the fields that are needed (name, capital, population, latlng), this will improve performance.
      */
      string apiUrl = "https://restcountries.com/v3.1/all?fields=name,capital,population,latlng";

      try
      {
        // Using the shared HttpClient instance to send the request.
        string response = await httpClient.GetStringAsync(apiUrl);
        JArray parsedResponse = JArray.Parse(response);
        List<CountryModel> countries = new();

        // Removed the unused 'Place' variable and the unnecessary 'if (true)' statement.
        foreach (JToken? x in parsedResponse)
        {
          try
          {
            CountryModel country = new()
            {
              Name = x?["name"]?["common"]?.ToString() ?? string.Empty,
              Capital = (x?["capital"] != null && x["capital"]?.Count() > 0) ? x["capital"]?[0]?.ToString() ?? string.Empty : string.Empty,
              Population = x?["population"]?.ToObject<int>() ?? 0,
              Latitude = (x?["latlng"] != null && x["latlng"]?.Count() > 0) ? x["latlng"]?[0]?.ToObject<float>() ?? 0 : 0,
              Longitude = (x?["latlng"] != null && x["latlng"]?.Count() > 1) ? x["latlng"]?[1]?.ToObject<float>() ?? 0 : 0
            };
            countries.Add(country);
          }
          catch (Exception ex)
          {
            // Instead of swallowing the exception, we're now throwing it to be handled by the caller.
            _logger.LogError(ex, "An error occurred while retrieving the countries");
            throw;
          }
        }

        cachedCountries = countries;
        return countries;
      }
      catch (Exception ex)
      {
        // Log the exception
        _logger.LogError(ex, "An error occurred while retrieving the countries");
        throw;
      }
    }

    /*
      Get the top 5 countries by population in the southern hemisphere
      The southern hemisphere is defined as having a latitude less than 0
     */
    public async Task<List<CountryModel>> GetTopFiveCountries()
    {
      List<CountryModel> countries = await GetAllCountries();
      return countries
          .Where(x => x.Latitude < 0)
          .OrderByDescending(x => x.Population)
          .Take(5)
          .ToList();
    }


    // Caching the country summaries to avoid making unnecessary API calls.
    private static readonly Dictionary<string, CountrySummaryModel>? cachedCountrySummaries = new(); // countryName -> CountrySummaryModel

    public async Task<CountrySummaryModel> GetCountrySummaryAsync(string countryName)
    {

      if (cachedCountrySummaries != null && cachedCountrySummaries.ContainsKey(countryName))
      {
        return cachedCountrySummaries[countryName];
      }

      string apiUrl = $"https://restcountries.com/v3.1/name/{countryName}?fields=name,capital,population,latlng,flags,currencies,languages,car,maps,capitalInfo";
      (string sunrise, string sunset) = await GetSunriseSunsetTimes(countryName);

      double klat = -33.9759724;
      double klng = 18.4592032;

      try
      {
        string response = httpClient.GetStringAsync(apiUrl).Result;
        JArray parsedResponse = JArray.Parse(response);
        CountrySummaryModel countrySummary = new()
        {
          Name = parsedResponse[0]?["name"]?["common"]?.ToString() ?? string.Empty,
          Capital = parsedResponse[0]?["capital"]?[0]?.ToString() ?? string.Empty,
          Population = parsedResponse[0]?["population"]?.ToObject<int>() ?? 0,
          Latitude = parsedResponse[0]?["capitalInfo"]?["latlng"]?[0]?.ToObject<float>() ?? 0,
          Longitude = parsedResponse[0]?["capitalInfo"]?["latlng"]?[1]?.ToObject<float>() ?? 0,
          Flag = parsedResponse[0]?["flags"]?["png"]?.ToString() ?? string.Empty,
          Currency = parsedResponse[0]?["currencies"]?.First?.First?["name"]?.ToString() ?? string.Empty,
          CurrencyCode = parsedResponse[0]?["currencies"]?.First?.First?["symbol"]?.ToString() ?? string.Empty,
          TotalLanguages = parsedResponse[0]?["languages"]?.Count() ?? 0,
          DriveSide = parsedResponse[0]?["car"]?["side"]?.ToString() ?? string.Empty,
          Map = parsedResponse[0]?["maps"]?["googleMaps"]?.ToString() ?? string.Empty,
          Sunrise = sunrise,
          Sunset = sunset,
          Distance = Distance.CalculateDistance(klat, klng, parsedResponse[0]?["capitalInfo"]?["latlng"]?[0]?.ToObject<float>() ?? 0, parsedResponse[0]?["capitalInfo"]?["latlng"]?[1]?.ToObject<float>() ?? 0)
        };

        if (cachedCountrySummaries != null)
        {
          // Using the country name as the key for the cached country summaries
          cachedCountrySummaries[countryName] = countrySummary;
        }
        return countrySummary;
      }
      catch (Exception ex)
      {
        // Log the exception
        _logger.LogError(ex, "An error occurred while retrieving the country summary");
        throw;
      }
    }

    public async Task<CountrySummaryModel> GetRandomCountryInSouthernHemisphere()
    {

      // Fixed latitude comparison to be less than 0 instead of greater or equal to 0
      List<CountryModel> countries = await GetAllCountries();
      IEnumerable<CountryModel> countriesInSouthernHemisphere = countries.Where(x => x.Latitude < 0);
      Random random = new();
      int randomIndex = random.Next(0, countriesInSouthernHemisphere.Count());
      CountryModel randomCountry = countriesInSouthernHemisphere.ElementAt(randomIndex);
      return await GetCountrySummaryAsync(randomCountry.Name);
    }

    /*
      Get the sunrise and sunset times for the capital city of the given country
      The sunrise and sunset times are in UTC
     */
    public async Task<(string, string)> GetSunriseSunsetTimes(string countryName)
    {
      try
      {
        (float lat, float lng) = await GetCapitalLatLng(countryName);
        return await GetSunriseSunsetTimes(lat, lng);
      }
      catch (Exception ex)
      {
        // Log the exception
        _logger.LogError(ex, "An error occurred while retrieving the sunrise and sunset times");
        throw;
      }
    }

    /*
      Get the latitude and longitude of the capital city of the given country
     */

    private static readonly Dictionary<string, (float, float)>? cachedCapitalLatLng = new(); // countryName -> (lat, lng)

    private static async Task<(float, float)> GetCapitalLatLng(string countryName)
    {
      if (cachedCapitalLatLng != null && cachedCapitalLatLng.ContainsKey(countryName))
      {
        return cachedCapitalLatLng[countryName];
      }

      string countryApiUrl = $"https://restcountries.com/v3.1/name/{countryName}?fields=capitalInfo";
      string countryResponse = await httpClient.GetStringAsync(countryApiUrl);
      JArray parsedCountryResponse = JArray.Parse(countryResponse);
      float lat = parsedCountryResponse[0]?["capitalInfo"]?["latlng"]?[0]?.ToObject<float>() ?? 0;
      float lng = parsedCountryResponse[0]?["capitalInfo"]?["latlng"]?[1]?.ToObject<float>() ?? 0;

      if (cachedCapitalLatLng != null)
      {
        cachedCapitalLatLng[countryName] = (lat, lng);
      }
      return (lat, lng);
    }
    /*
      private method to get the sunrise and sunset times for a given latitude and longitude
     */

    private static readonly Dictionary<(float, float), (string, string)>? cachedSunriseSunsetTimes = new(); // (lat, lng) -> (sunrise, sunset)
    private static async Task<(string, string)> GetSunriseSunsetTimes(float lat, float lng)
    {

      if (cachedSunriseSunsetTimes != null && cachedSunriseSunsetTimes.ContainsKey((lat, lng)))
      {
        return cachedSunriseSunsetTimes[(lat, lng)];
      }


      // Construct the URL for the sunrise-sunset API
      string sunriseSunsetApiUrl = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&formatted=0&date=today";

      // Get the sunrise and sunset times for the given country
      string sunriseSunsetResponse = await httpClient.GetStringAsync(sunriseSunsetApiUrl);
      JObject parsedSunriseSunsetResponse = JObject.Parse(sunriseSunsetResponse);
      string sunrise = parsedSunriseSunsetResponse?["results"]?["sunrise"]?.ToString() ?? string.Empty;
      string sunset = parsedSunriseSunsetResponse?["results"]?["sunset"]?.ToString() ?? string.Empty;

      if (cachedSunriseSunsetTimes != null)
      {
        cachedSunriseSunsetTimes[(lat, lng)] = (sunrise, sunset);
      }
      return (sunrise, sunset);
    }
  }
}
