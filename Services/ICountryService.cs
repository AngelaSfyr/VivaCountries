using VivaApiProject.Models;

namespace VivaApiProject.Services
{
    public interface ICountryService
    {
        Task<List<CountryResponse>> GetAllCountriesAsync();
    }
}
