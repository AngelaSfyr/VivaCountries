namespace VivaApiProject.Models
{
    public class CountryResponse
    {
        public string? Name { get; set; }
        public string? Capital { get; set; }
        public List<Border>? Borders { get; set; }
    }
}
