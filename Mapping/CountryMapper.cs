using VivaApiProject.Models;

namespace VivaApiProject.Mapping
{
    public class CountryMapper
    {
        public List<CountryResponse> CountryToDto(List<Country>? cachedCountries)
        {
            var result = new List<CountryResponse>();
            if (cachedCountries != null)
            {
                result = cachedCountries
                    .Select(c => new CountryResponse
                    {
                        Name = c.Name,
                        Capital = c.Capital,
                        Borders = c.Borders.Select(b => b.Code).ToList()
                    }).ToList();
            }
            return result;
        }
    }
}
