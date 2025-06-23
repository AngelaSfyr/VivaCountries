using Microsoft.Extensions.Caching.Memory;
using VivaApiProject.Models;

namespace VivaApiProject.Handlers
{
    public interface ICountryCacheHandler
    {
        List<Country> GetCache();
        void SetCache(List<Country> countries);
    }
}
