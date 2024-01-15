namespace KAHA.TravelBot.NETCoreReactApp.Models
{
  /*
  Complete the TravelBotService.GetCountrySummary method, returning an interesting country summary, for a given Country Name.
This can consist of some key information that would be interesting for a potential traveller.
Include the Sunrise and Sunset times for the capital city of the given country.
Some key stats that we would like to see: Total number of official languages, the side of the road inhabitants drive on.
   */
  public class CountrySummaryModel : CountryModel
  {
    public string? Sunrise { get; set; }
    public string? Sunset { get; set; }
    public int TotalLanguages { get; set; }
    public string? DriveSide { get; set; }

    public string? Currency { get; set; }
    public string? CurrencyCode { get; set; }

    public string? Flag { get; set; }

    public string? Map { get; set; }

    public double? Distance { get; set; }

  }
}
