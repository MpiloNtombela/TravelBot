using System.Net;
using KAHA.TravelBot.NETCoreReactApp.Models;
using KAHA.TravelBot.NETCoreReactApp.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KAHA.TravelBot.NETCoreReactApp.Controllers
{
  // Declare a private readonly field for the ITravelBotService

  [Route("api/[controller]")]
  [ApiController]
  public class CountriesController : ControllerBase
  {
    /*
      * The ITravelBotService is injected into the constructor of the CountriesController.
      * This is called Dependency Injection.
      * This way, you can use the same instance of TravelBotService across different methods in the controller
     */
    private readonly ITravelBotService _travelBotService;

    // Inject the ITravelBotService into the constructor
    public CountriesController(ITravelBotService travelBotService)
    {
      _travelBotService = travelBotService;
    }

    // Get all countries
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
      try
      {
        var countries = await _travelBotService.GetAllCountries();
        return Ok(countries);
      }
      catch (HttpRequestException ex)
      {
        if (ex.StatusCode.HasValue)
        {
          return StatusCode((int)ex.StatusCode.Value);
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception)
      {
        return StatusCode(500);
      }
    }

    // GET: api/<CountriesController>
    [HttpGet("top5")]
    public async Task<IActionResult> GetTopFive()
    {
      try
      {
        var countries = await _travelBotService.GetTopFiveCountries();
        return Ok(countries);
      }
      catch (HttpRequestException ex)
      {
        if (ex.StatusCode.HasValue)
        {
          return StatusCode((int)ex.StatusCode.Value);
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception)
      {
        return StatusCode(500);
      }
    }

    // GET api/<CountriesController>/Zimbabwe
    [HttpGet("{countryName}")]
    public async Task<IActionResult> GetSummary(string countryName)
    {
      try
      {
        CountrySummaryModel countrySummary = await _travelBotService.GetCountrySummaryAsync(countryName);
        if (countrySummary == null)
        {
          return NotFound();
        }
        return Ok(countrySummary);
      }
      catch (HttpRequestException ex)
      {
        if (ex.StatusCode.HasValue)
        {
          return StatusCode((int)ex.StatusCode.Value);
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception)
      {
        return StatusCode(500);
      }
    }

    // POST api/<CountriesController>
    [HttpGet("random")]
    public async Task<IActionResult> GetRandom()
    {
      try
      {
        CountrySummaryModel countrySummary = await _travelBotService.GetRandomCountryInSouthernHemisphere();
        if (countrySummary == null)
        {
          return NotFound();
        }
        return Ok(countrySummary);
      }
      catch (HttpRequestException ex)
      {
        if (ex.StatusCode.HasValue)
        {
          return StatusCode((int)ex.StatusCode.Value);
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception)
      {
        return StatusCode(500);
      }
    }
  }
}
