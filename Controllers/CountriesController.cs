using Microsoft.AspNetCore.Mvc;
using VivaApiProject.Services;

namespace VivaApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _countryService.GetAllCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching countries"+ex);
                return StatusCode(500, ex);
            }

        }
    }
}
