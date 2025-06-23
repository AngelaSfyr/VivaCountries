using VivaApiProject.Models;

namespace VivaApiProject.Services
{
    public interface ICountryService
    {
        Task<List<Country>> GetAllCountriesAsync();
    }
}
