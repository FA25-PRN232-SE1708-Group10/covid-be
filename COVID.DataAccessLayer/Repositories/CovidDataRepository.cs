using COVID.DataAccessLayer.Models;

namespace COVID.DataAccessLayer.Repositories
{
    public class CovidDataRepository : ICovidDataRepository
    {
        private readonly CovidDbContext _context;

        public CovidDataRepository(CovidDbContext context)
        {
            _context = context;
        }

        public IQueryable<CovidDataPoint> GetAll()
        {
            return _context.CovidDataPoints;
        }
    }
}
