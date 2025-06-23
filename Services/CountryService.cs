using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VivaApiProject.Data.Migrations;
using VivaApiProject.Models;

namespace VivaApiProject.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        //TODO: in memory cache h ConcurrentDictionary
        private readonly MemoryCache _cache;

        public CountryService(HttpClient httpClient, DatabaseContext context, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _context = context;
            _cache = cache as MemoryCache ?? throw new InvalidOperationException("MemoryCache must be of concrete type.");
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            List<Country> result = new List<Country>();
            // 1. Check cache
            foreach (object key in _cache.Keys)
            {
                if (_cache.TryGetValue(key, out Country cachedCountry))
                    result.Add(cachedCountry);
            }
            if (result.Any())
                return result;


            // 2. Check database

            var dbCountries = await _context.Countries
                .Include(c => c.Borders)
                .ToListAsync();

            if (dbCountries.Any())
            {
                foreach (var country in dbCountries)
                {
                    var key = $"Country_{country.Name.ToLower()}";
                    //TODO: if I set timelimit then I need to verify what is in cach and what is in database
                    _cache.Set(key, country);
                    result.Add(country);
                }
                return result;
            }


            // 3. Αν δεν υπάρχει ούτε στη βάση, πάρε από 3rd party API
            //example: {"name":{"common":"Turkey","official":"Republic of Turkey","nativeName":{"tur":{"official":"Türkiye Cumhuriyeti","common":"Türkiye"}}},"capital":["Ankara"],"borders":["ARM","AZE","BGR","GEO","GRC","IRN","IRQ","SYR"]}
            var raw = await _httpClient.GetFromJsonAsync<List<RestCountry>>("https://restcountries.com/v3.1/all?fields=name,capital,borders");
            //Console.WriteLine(raw);
            result = raw.Select(c => new Country
            {
                Name = c.Name.Common,
                Capital = string.IsNullOrEmpty(c.Capital?.FirstOrDefault()) ? "N/A" : c.Capital.First(),
                Borders = c.Borders?.Select(b => new Border
                {
                    Code = b
                }).ToList() ?? new List<Border>()
            }).ToList();

            // Αποθήκευση στη βάση
            await SaveCountriesToDatabaseAsync(result);

            // Cache κάθε χώρα ξεχωριστά και όλη τη λίστα
            foreach (var country in result)
            {
                var key = $"Country_{country.Name.ToLower()}";
                //TODO: if I set timelimit then I need to verify what is in cach and what is in database
                _cache.Set(key, country);
            }

            return result;

        }
        private class RestCountry
        {
            public NameInfo Name { get; set; }
            public List<string> Capital { get; set; }
            public List<string> Borders { get; set; }
        }

        private class NameInfo
        {
            public string Common { get; set; }
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
