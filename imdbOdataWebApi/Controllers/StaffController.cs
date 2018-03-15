using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.OData;
using Microsoft.AspNet.OData.Extensions;

using DataAccess;
using Microsoft.AspNet.OData;

namespace imdbOdataWebApi.Controllers
{
    public class StaffController : /*Controller*/ ODataController
    {

        private readonly imdbContext _appDbContext;
        public StaffController(imdbContext sampleODataDbContext)
        {
            _appDbContext = sampleODataDbContext;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            //The following line is the incredibly important piece that does all of the magic.
            //Notice that an IQueryable is being returned. You might actually be able to pull odata off 
            //with other objects that support the IQueryable interface!
            return Ok(_appDbContext.Staff.AsQueryable());
        }

        //Sample Queries: Find all actors/actresses with a name like 'Denzel'. List 4 movies
        //that they are known for.
        //http://localhost:5000/odata/Staff?$filter=contains(PrimaryName, 'Denzel')&$expand=KnownForTitles($expand=AssociatedMedia)
    }
}
