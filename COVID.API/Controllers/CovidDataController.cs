using COVID.API.Data;
using COVID.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace COVID.API.Controllers
{
    public class CovidDataController : ODataController
    {
        private readonly CovidDbContext _context;

        public CovidDataController(CovidDbContext context)
        {
            _context = context;
        }

        [EnableQuery(PageSize = 1000)]
        public IQueryable<CovidDataPoint> Get()
        {
            return _context.CovidDataPoints;
        }
    }
}
