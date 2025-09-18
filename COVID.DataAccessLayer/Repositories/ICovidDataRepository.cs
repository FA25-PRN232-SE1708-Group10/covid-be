using COVID.DataAccessLayer.Models;

namespace COVID.DataAccessLayer.Repositories
{
    public interface ICovidDataRepository
    {
        IQueryable<CovidDataPoint> GetAll();
    }
}
