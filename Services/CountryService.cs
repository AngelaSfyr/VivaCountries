using Azure;
using Microsoft.EntityFrameworkCore;
using VivaApiProject.Data.Migrations;
using VivaApiProject.Handlers;
using VivaApiProject.Models;
using VivaApiProject.Mapping;

namespace VivaApiProject.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        //TODO: in memory cache h ConcurrentDictionary
        private readonly ICountryCacheHandler _cacheHandler;
        private readonly CountryMapper countryMapper;

        public CountryService(HttpClient httpClient, DatabaseContext context, ICountryCacheHandler cacheHandler, CountryMapper countryMapper)
        {
            _httpClient = httpClient;
            _context = context;
            _cacheHandler = cacheHandler;
            this.countryMapper = countryMapper;
        }

        public async Task<List<CountryResponse>> GetAllCountriesAsync()
        {
            List<CountryResponse> result = new List<CountryResponse>();

            // 1. Check cache
            var countriesCashed = _cacheHandler.GetCache();
            result = countryMapper.CountryToDto(countriesCashed);
            if(result.Any())
            {
                return result;
            }

            // 2. Check database
            var dbCountries = await _context.Countries
                .Include(c => c.Borders)
                .ToListAsync();

            if (dbCountries.Any())
            {
                _cacheHandler.SetCache(dbCountries);
                result = countryMapper.CountryToDto(dbCountries);
                return result;
            }


            // 3. Αν δεν υπάρχει ούτε στη βάση, πάρε από 3rd party API

            var response = await GetCountriesCallAsync();
            if(response != null && response.Any())
            {
                // Αποθήκευση στη βάση
                await SaveCountriesToDatabaseAsync(response);

                // Cache κάθε χώρα ξεχωριστά και όλη τη λίστα
                _cacheHandler.SetCache(response);
                result = countryMapper.CountryToDto(response);
            }

            return result;
        }
        private async Task<List<Country>> GetCountriesCallAsync()
        {
            //example: {"name":{"common":"Turkey","official":"Republic of Turkey","nativeName":{"tur":{"official":"Türkiye Cumhuriyeti","common":"Türkiye"}}},"capital":["Ankara"],"borders":["ARM","AZE","BGR","GEO","GRC","IRN","IRQ","SYR"]}
            var raw = await _httpClient.GetFromJsonAsync<List<RestCountry>>("https://restcountries.com/v3.1/all?fields=name,capital,borders");
            //Console.WriteLine(raw);
            var response = raw.Select(c => new Country
            {
                Name = c.Name.Common,
                Capital = string.IsNullOrEmpty(c.Capital?.FirstOrDefault()) ? "N/A" : c.Capital.First(),
                Borders = c.Borders?.Select(b => new Border
                {
                    Code = b
                }).ToList() ?? new List<Border>()
            }).ToList();

            return response;
        }
        public async Task SaveCountriesToDatabaseAsync(List<Country> result)
        {
            if (result == null || !result.Any())
                return;

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                foreach (var country in result)
                {
                    if (await _context.Countries.AnyAsync(c => c.Name == country.Name))
                        continue;

                    var entity = new Country
                    {
                        Name = country.Name,
                        Capital = country.Capital,
                        Borders = country.Borders.ToList()
                    };

                    _context.Countries.Add(entity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

    }
}
