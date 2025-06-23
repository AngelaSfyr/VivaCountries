namespace VivaApiProject.Models
{
    public class Border
    {
        public int Id { get; set; } // Primary key
        public string Code { get; set; } // e.g., "FRA", "ESP"

        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
