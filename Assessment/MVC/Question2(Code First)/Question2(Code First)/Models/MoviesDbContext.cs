using System.Data.Entity;

namespace Question2_Code_First_.Models
{
    public class MoviesDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }
}