using COVID.BusinessLogicLayer.Services;
using COVID.DataAccessLayer.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace COVID.API.Controllers
{
    public class CovidDataController : ODataController
    {
        private readonly ICovidDataService _service;

        public CovidDataController(ICovidDataService service)
        {
            _service = service;
        }

        [EnableQuery(PageSize = 1000)]
        public IQueryable<CovidDataPoint> Get()
        {
            return _service.GetAllData();
        }
    }
}
