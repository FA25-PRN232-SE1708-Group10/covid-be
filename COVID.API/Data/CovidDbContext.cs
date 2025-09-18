using COVID.API.Models;
using Microsoft.EntityFrameworkCore;

namespace COVID.API.Data
{
    public class CovidDbContext : DbContext
    {
        public CovidDbContext(DbContextOptions<CovidDbContext> options)
            : base(options) { }

        public DbSet<CovidDataPoint> CovidDataPoints { get; set; }
    }
}
