using Microsoft.Extensions.Caching.Memory;
using VivaApiProject.Models;

namespace VivaApiProject.Handlers
{
    public class CountryCacheHandler : ICountryCacheHandler
    {
        private readonly MemoryCache _cache;

        public CountryCacheHandler(IMemoryCache cache)
        {
            _cache = cache as MemoryCache ?? throw new InvalidOperationException("MemoryCache must be of concrete type.");
        }

        public List<Country> GetCache()
        {
            var result = new List<Country>();
            foreach (object key in _cache.Keys)
            {
                if (_cache.TryGetValue(key, out Country cachedCountry))
                    result.Add(cachedCountry);
            }
            return result;
        }

        public void SetCache(List<Country> countries)
        {
            foreach (var country in countries)
            {
                var key = $"Country_{country.Name.ToLower()}";
                _cache.Set(key, country);
            }
        }
    }
}
