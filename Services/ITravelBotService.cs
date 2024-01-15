using KAHA.TravelBot.NETCoreReactApp.Models;

namespace KAHA.TravelBot.NETCoreReactApp.Services
{
  // Declare a public interface for the ITravelBotService
  public interface ITravelBotService
  {
    public Task<List<CountryModel>> GetAllCountries();
    public Task<List<CountryModel>> GetTopFiveCountries();
    public Task<CountrySummaryModel> GetCountrySummaryAsync(string countryName);
    public Task<CountrySummaryModel> GetRandomCountryInSouthernHemisphere();
    public Task<(string, string)> GetSunriseSunsetTimes(string countryName);
  }
}
