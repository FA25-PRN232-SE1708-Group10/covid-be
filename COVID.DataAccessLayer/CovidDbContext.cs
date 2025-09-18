using COVID.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace COVID.DataAccessLayer
{
    public class CovidDbContext : DbContext
    {
        public CovidDbContext(DbContextOptions<CovidDbContext> options)
            : base(options) { }

        public DbSet<CovidDataPoint> CovidDataPoints { get; set; }
    }
}
