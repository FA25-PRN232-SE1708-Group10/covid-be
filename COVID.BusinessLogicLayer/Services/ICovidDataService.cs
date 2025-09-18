using COVID.DataAccessLayer.Models;

namespace COVID.BusinessLogicLayer.Services
{
    public interface ICovidDataService
    {
        IQueryable<CovidDataPoint> GetAllData();
    }
}
