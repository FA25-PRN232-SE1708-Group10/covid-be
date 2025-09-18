using COVID.DataAccessLayer.Models;
using COVID.DataAccessLayer.Repositories;

namespace COVID.BusinessLogicLayer.Services
{
    public class CovidDataService : ICovidDataService
    {
        private readonly ICovidDataRepository _repository;

        public CovidDataService(ICovidDataRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<CovidDataPoint> GetAllData()
        {
            return _repository.GetAll();
        }
    }
}
