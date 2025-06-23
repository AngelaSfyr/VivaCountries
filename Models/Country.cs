namespace VivaApiProject.Models
{
    public class Country
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Capital { get; set; }
        public ICollection<Border> Borders { get; set; } = new List<Border>();
    }
}
