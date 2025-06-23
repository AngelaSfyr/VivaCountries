namespace VivaApiProject.Models
{
    public class CountryResponse
    {
        public string? Name { get; set; }
        public string? Capital { get; set; }
        public List<string>? Borders { get; set; }
    }

    public class RestCountry
    {
        public NameInfo Name { get; set; }
        public List<string> Capital { get; set; }
        public List<string> Borders { get; set; }
    }

    public class NameInfo
    {
        public string Common { get; set; }
    }

}
