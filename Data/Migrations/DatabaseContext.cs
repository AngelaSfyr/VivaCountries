using VivaApiProject.Models;
using Microsoft.EntityFrameworkCore;

namespace VivaApiProject.Data.Migrations
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Border> Borders { get; set; }

    }
}